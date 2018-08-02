Imports System.Data.SqlClient
Imports System.Text
Imports IBM.Data.DB2.iSeries
Imports Talent.Common
Imports Talent.Common.Utilities

<Serializable()>
Public Class DBProduct
    Inherits DBAccess

#Region "Constants"

    Private Const ProductDetails As String = "ProductDetails"
    Private Const ProductPricingDetails As String = "ProductPricingDetails"
    Private Const AvailableStandsWithoutDescriptions As String = "AvailableStandsWithoutDescriptions"
    Private Const ProductList As String = "ProductList"
    Private Const ProductListReturnAll As String = "ProductListReturnAll"
    Private Const StandDescriptions As String = "StandDescriptions"
    Private Const ProductStadiumAvailability As String = "ProductStadiumAvailability"
    Private Const ProductSeatAvailability As String = "ProductSeatAvailability"
    Private Const ProductSeatNumbers As String = "ProductSeatNumbers"
    Private Const EligibleTicketingCustomers As String = "RetrieveEligibleTicketingCustomers"
    Private Const ProductSeatRestrictions As String = "ProductSeatRestrictions"
    Private Const ProductSeatDetails As String = "ProductSeatDetails"
    Private Const ProductNavigationLoad As String = "ProductNavigationLoad"
    Private Const ProductStockLoad As String = "ProductStockLoad"
    Private Const ProductLoad As String = "ProductLoad"
    Private Const ProductOptionsLoad As String = "ProductOptionsLoad"
    Private Const ProductRelationsLoad As String = "ProductRelationsLoad"
    Private Const ProductPriceLoad As String = "ProductPriceLoad"
    Private Const ProductHospitality As String = "ProductHospitality"
    Private Const CourseAvailabilityPreCheck As String = "CourseAvailabilityPreCheck"
    Private Const ProductSubTypesList As String = "ProductSubTypesList"
    Private Const ProductReservationCodes As String = "RetrieveProductReservationCodes"
    Private Const RetrieveSeatHistory As String = "RetrieveSeatHistory"
    Private Const RetrieveSeatPrintHistory As String = "RetrieveSeatPrintHistory"
    Private Const ProductAdditionalInformation As String = "ProductAdditionalInformation"
    Private Const ProductAdditionalInformationOption As String = "ProductAdditionalInformationOption"
    Private Const RetrieveProductQuestionAnswers As String = "RetrieveProductQuestionAnswers"
    Private Const CreateLinkedProductPackage As String = "CreateLinkedProductPackage"
    Private Const UpdateLinkedProductPackage As String = "UpdateLinkedProductPackage"
    Private Const DeleteLinkedProductPackage As String = "DeleteLinkedProductPackage"
    Private Const RetrieveProductsFiltered As String = "RetrieveProductsFiltered"
    Private Const RetrieveStadiumStands As String = "RetrieveStadiumStands"
    Private Const RetrieveStadiums As String = "RetrieveStadiums"
    Private Const RetrieveStadiumStandAreas As String = "RetrieveStadiumStandAreas"
    Private Const AwayProductAvailability As String = "AwayProductAvailability"
    Private Const TravelProductAvailability As String = "TravelProductAvailability"
    Private Const EventProductAvailability As String = "EventProductAvailability"
    Private Const ProductPriceBreaks As String = "ProductPriceBreaks"
    Private Const PriceBreakSeatDetails As String = "PriceBreakSeatDetails"
    Private Const RetrieveProductsByDescription As String = "RetrieveProductsByDescription"
    Private Const RetrieveCorporatePackagesByDescription As String = "RetrieveCorporatePackagesByDescription"
    Private Const RetrieveProductGroups As String = "RetrieveProductGroups"
    Private Const RetrieveProductGroupFixtures As String = "RetrieveProductGroupFixtures"
    Private Const RetrieveProductGroupPackages As String = "RetrieveProductGroupPackages"

#End Region

#Region "Class Level Fields"

    Private _de As New DEProductDetails
    Private _deProductQuestionsAnswers As New List(Of DEProductQuestionsAnswers)
    Private _deStock As New DEStock
    Private sStadia() As String
    Private PARAMOUT2 As String = String.Empty
    Private PARAMOUT3 As String = String.Empty
    Private PARAMOUT4 As String = String.Empty
    Private ws016rLastPriceCode As String = String.Empty
    Private groupFields As New Collection
    Private _deProductGroupHierarchies As New DEProductGroupHierarchies
    Private _deProductGroups As New DEProductGroups
    Private _deProductRelations As New DEProductRelations
    Private _dep As New DEProduct
    Private _depo As DEProductOptions
    Private _productCollection As Collection
    Private _DeproductPriceLoad As DEProductPriceLoad
    Private _cmdAdapter As iDB2DataAdapter = Nothing
    Private _cmd As iDB2Command = Nothing
    Private Class attributeDetails
        Public attributeValue As Decimal
        Public attributeBoolean As Boolean
        Public attributeDescription As String
        Public attributeDate As Date
    End Class

#End Region

#Region "Public Properties"

    Public Property De() As DEProductDetails
        Get
            Return _de
        End Get
        Set(ByVal value As DEProductDetails)
            _de = value
        End Set
    End Property
    Public Property DeProductQuestionsAnswers() As List(Of DEProductQuestionsAnswers)
        Get
            Return _deProductQuestionsAnswers
        End Get
        Set(ByVal value As List(Of DEProductQuestionsAnswers))
            _deProductQuestionsAnswers = value
        End Set
    End Property
    Public Property DeProductGroupHierarchies() As DEProductGroupHierarchies
        Get
            Return _deProductGroupHierarchies
        End Get
        Set(ByVal value As DEProductGroupHierarchies)
            _deProductGroupHierarchies = value
        End Set
    End Property
    Public Property DeProductGroups() As DEProductGroups
        Get
            Return _deProductGroups
        End Get
        Set(ByVal value As DEProductGroups)
            _deProductGroups = value
        End Set
    End Property
    Public Property DeProductRelations() As DEProductRelations
        Get
            Return _deProductRelations
        End Get
        Set(ByVal value As DEProductRelations)
            _deProductRelations = value
        End Set
    End Property
    Public Property Dep() As DEProduct
        Get
            Return _dep
        End Get
        Set(ByVal value As DEProduct)
            _dep = value
        End Set
    End Property
    Public Property Depo() As DEProductOptions
        Get
            Return _depo
        End Get
        Set(ByVal value As DEProductOptions)
            _depo = value
        End Set
    End Property
    Public Property ProductCollection() As Collection
        Get
            Return _productCollection
        End Get
        Set(ByVal value As Collection)
            _productCollection = value
        End Set
    End Property
    Public Property Stock() As DEStock
        Get
            Return _deStock
        End Get
        Set(ByVal value As DEStock)
            _deStock = value
        End Set
    End Property
    Public Property DeProductPriceLoad() As DEProductPriceLoad
        Get
            Return _DeproductPriceLoad
        End Get
        Set(ByVal value As DEProductPriceLoad)
            _DeproductPriceLoad = value
        End Set
    End Property
    Public Property AgentLevelCacheForProductStadiumAvailability() As Boolean = False
#End Region

#Region "TALENTTKT"

    Protected Overrides Function AccessDataBaseTALENTTKT() As ErrorObj
        Dim err As New ErrorObj
        Select Case _settings.ModuleName
            'Basic product details
            Case Is = ProductPricingDetails : err = AccessDatabaseWS117R()
            Case Is = ProductDetails : err = AccessDatabaseWS007R()
            Case Is = ProductList
                Select Case _de.ProductType
                    Case Is = GlobalConstants.MEMBERSHIPPRODUCTTYPE, GlobalConstants.PPSPRODUCTTYPE : err = AccessDatabaseWS006R()
                    Case Is = GlobalConstants.TRAVELPRODUCTTYPE, GlobalConstants.EVENTPRODUCTTYPE : err = AccessDatabaseWS015R()
                    Case Else : err = AccessDatabaseWS016R()
                End Select
            Case Is = ProductListReturnAll : err = AccessDatabaseWS609R()
            Case Is = ProductSubTypesList : err = AccessDatabaseWS070R()
            Case Is = RetrieveProductsFiltered : err = AccessDatabaseMD008S()
            Case Is = RetrieveProductsByDescription : err = AccessDatabaseMD074S()
            Case Is = RetrieveCorporatePackagesByDescription : err = AccessDatabaseMD075S()

                'Availability and Seating relatated
            Case Is = AvailableStandsWithoutDescriptions : err = AccessDatabaseWS017R()
            Case Is = StandDescriptions : err = AccessDatabaseWS118R()
            Case Is = ProductStadiumAvailability : err = AccessDatabaseWS011R()
            Case Is = ProductSeatAvailability : err = AccessDatabaseWS151R()
            Case Is = ProductSeatNumbers : err = AccessDatabaseWS152R()
            Case Is = ProductSeatDetails : err = AccessDatabaseWS024R()
            Case Is = RetrieveStadiumStands : err = AccessDatabaseSD002S()
            Case Is = RetrieveStadiums : err = AccessDatabaseSD005S()
            Case Is = RetrieveStadiumStandAreas : err = AccessDatabaseSD021S()
            Case Is = AwayProductAvailability, TravelProductAvailability, EventProductAvailability : err = AccessDatabaseMD011S()
            Case Is = ProductSeatRestrictions : err = AccessDatabaseWS153R()

                'Hospitality and Customer/Purchase related
            Case Is = EligibleTicketingCustomers : err = AccessDatabaseWS045R()
            Case Is = ProductHospitality : err = AccessDatabaseWS002R()
            Case Is = CourseAvailabilityPreCheck : err = AccessDatabaseWS049R()
            Case Is = ProductReservationCodes : err = AccessDatabaseWS154R()
            Case Is = RetrieveSeatHistory : err = AccessDatabaseMD188R()
            Case Is = RetrieveSeatPrintHistory : err = PerformRetrieveSeatPrintHistory()

                'Additional Information and Linked Products
            Case Is = ProductAdditionalInformationOption : err = AccessDatabaseWS145R()
            Case Is = ProductAdditionalInformation : err = AccessDatabaseWS144R()
            Case Is = RetrieveProductQuestionAnswers : err = AccessDatabaseWS146R()
            Case Is = CreateLinkedProductPackage, UpdateLinkedProductPackage, DeleteLinkedProductPackage : err = AccessDatabaseLP001S()

                'Price Breaks
            Case Is = ProductPriceBreaks : err = AccessDatabaseMD141S()
            Case Is = PriceBreakSeatDetails : err = AccessDatabaseMD143S()

                'Product Groups
            Case Is = RetrieveProductGroups : err = AccessDatabasePG001S()
            Case Is = RetrieveProductGroupFixtures : err = AccessDatabasePG002S()
            Case Is = RetrieveProductGroupPackages : err = AccessDatabaseWS670R()
        End Select
        Return err
    End Function
#End Region

    Private Function AccessDatabaseWS049R() As ErrorObj

        Dim err As New ErrorObj
        ResultDataSet = New DataSet
        Dim dRow As DataRow = Nothing
        Dim PARAMOUT As String = String.Empty


        'Create the Status data table
        Dim DtStatusResults As New DataTable
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        'Create the product list data table
        Dim DtCourseAvailabilityPreCheck As New DataTable
        ResultDataSet.Tables.Add(DtCourseAvailabilityPreCheck)
        With DtCourseAvailabilityPreCheck.Columns
            .Add("ProductCode", GetType(String))
            .Add("Availability", GetType(String))
        End With

        Try

            'Call WS049R
            PARAMOUT = CallWS049R()

            dRow = Nothing
            dRow = DtStatusResults.NewRow
            If PARAMOUT.Substring(1023, 1) = "E" Or PARAMOUT.Substring(1021, 2).Trim <> "" Then
                dRow("ErrorOccurred") = "E"
                dRow("ReturnCode") = PARAMOUT.Substring(1021, 2)
            Else
                dRow("ErrorOccurred") = ""
                dRow("ReturnCode") = ""
            End If
            DtStatusResults.Rows.Add(dRow)

            'No errors 
            If PARAMOUT.Substring(1023, 1) <> "E" And PARAMOUT.Substring(1021, 2).Trim = "" Then

                Dim sProductCode As String = PARAMOUT.Substring(0, 6)
                Dim sAvailability As String = PARAMOUT.Substring(6, 7)

                'Create a new row
                dRow = Nothing
                dRow = DtCourseAvailabilityPreCheck.NewRow
                dRow("ProductCode") = sProductCode.Trim()
                dRow("Availability") = sAvailability.TrimStart("0")
                DtCourseAvailabilityPreCheck.Rows.Add(dRow)

            End If

        Catch ex As Exception
            ResultDataSet = Nothing
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBPD-WS049R"
                .HasError = True
            End With
        End Try

        Return err

    End Function

    Private Function CallWS049R() As String

        'Create command object
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strProgram As String = "WS049R"
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim &
                                    "/" & strProgram & "(@PARAM1)"
        Dim parmIO As iDB2Parameter
        Dim PARAMOUT As String = String.Empty

        'Set the connection string
        cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)

        'Populate the parameter
        parmIO = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 1024)
        parmIO.Value = WS049RParm()
        parmIO.Direction = ParameterDirection.InputOutput

        'Execute
        TalentCommonLog("CallWS049R", "", "Backend Request: strHEADER=" & strHEADER & ", parmIO.Value=" & parmIO.Value)

        cmdSELECT.ExecuteNonQuery()
        PARAMOUT = cmdSELECT.Parameters(Param1).Value.ToString

        TalentCommonLog("CallWS049R", "", "Backend Response: PARAMOUT=" & PARAMOUT)

        Return PARAMOUT

    End Function

    Private Function WS049RParm() As String

        Dim myString As String

        'Construct the parameter
        myString = Utilities.FixStringLength(_de.ProductCode, 6) &
                   Utilities.FixStringLength("", 1018)

        Return myString

    End Function

    Private Function AccessDatabaseWS070R() As ErrorObj

        Dim err As New ErrorObj
        ResultDataSet = New DataSet
        Dim dRow As DataRow = Nothing
        Dim PARAMOUT As String = String.Empty
        Dim moreRecords As Boolean = True
        Dim lastProductType As String = String.Empty
        Dim lastProductSubType As String = String.Empty

        'Create the Status data table
        Dim dtStatusResults As New DataTable
        dtStatusResults.TableName = "Status"
        ResultDataSet.Tables.Add(dtStatusResults)
        With dtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        'Create the product sub type list data table
        Dim dtProductSubTypeList As New DataTable
        dtProductSubTypeList.TableName = "ProductSubTypes"
        ResultDataSet.Tables.Add(dtProductSubTypeList)
        With dtProductSubTypeList.Columns
            .Add("ProductType", GetType(String))
            .Add("ProductSubType", GetType(String))
            .Add("SubTypeDescription", GetType(String))
            .Add("ProductMDTE08", GetType(String))
            .Add("StartDate", GetType(Date))
            .Add("EndDate", GetType(Date))
            .Add("CategoryId", GetType(Integer))
            .Add("LocationIds", GetType(String))
        End With

        'Create the location code desc list data table
        Dim dtLocations As New DataTable
        dtLocations.TableName = "LocationDetails"
        ResultDataSet.Tables.Add(dtLocations)
        With dtLocations.Columns
            .Add("LocationId", GetType(String))
            .Add("Location", GetType(String))
        End With

        Try

            'Loop until no more products available
            Do While moreRecords = True

                'Call WS070R
                PARAMOUT = CallWS070R(lastProductType, lastProductSubType, PARAMOUT2)

                dRow = Nothing
                dRow = dtStatusResults.NewRow
                If PARAMOUT.Substring(10239, 1) = "E" Or PARAMOUT.Substring(10237, 2).Trim <> "" Then
                    dRow("ErrorOccurred") = "E"
                    dRow("ReturnCode") = PARAMOUT.Substring(10237, 2)
                Else
                    dRow("ErrorOccurred") = ""
                    dRow("ReturnCode") = ""
                End If
                dtStatusResults.Rows.Add(dRow)

                'No errors 
                If PARAMOUT.Substring(10239, 1) <> "E" And PARAMOUT.Substring(10237, 2).Trim = "" Then

                    'Extract the data from the parameter
                    Dim position As Integer = 0
                    Dim exitLoop As Boolean = False
                    Do While Not exitLoop

                        ' Determine the start point for the next set of information is
                        If PARAMOUT.Substring(position, 6).Trim = "" Then
                            exitLoop = True
                        Else

                            'Create a new row
                            dRow = Nothing
                            dRow = dtProductSubTypeList.NewRow
                            dRow("ProductType") = PARAMOUT.Substring(position, 1)
                            dRow("ProductSubType") = PARAMOUT.Substring(position + 1, 4)
                            dRow("ProductMDTE08") = PARAMOUT.Substring(position + 5, 7)
                            dRow("StartDate") = Utilities.ISeriesDate(PARAMOUT.Substring(position + 5, 7))
                            dRow("EndDate") = Utilities.ISeriesDate(PARAMOUT.Substring(position + 12, 7))
                            If String.IsNullOrWhiteSpace(PARAMOUT.Substring(position + 19, 15)) Then
                                dRow("CategoryId") = 0
                            Else
                                dRow("CategoryId") = PARAMOUT.Substring(position + 19, 15)
                            End If
                            Dim endPos As Integer = PARAMOUT.IndexOf("stad=", position)
                            dRow("SubTypeDescription") = PARAMOUT.Substring(position + 39, (endPos - position - 39))
                            position = endPos + 5
                            endPos = PARAMOUT.IndexOf("@#@", position)
                            dRow("LocationIds") = PARAMOUT.Substring(position, (endPos - position))
                            dtProductSubTypeList.Rows.Add(dRow)

                            'Loop again
                            position = endPos + 3
                        End If
                    Loop

                    'Extract the stadium information on the first call
                    If String.IsNullOrEmpty(lastProductType) Then

                        'Extract the data from the parameter
                        position = 0
                        exitLoop = False
                        Do While Not exitLoop

                            ' Determine the start point for the next set of information is
                            If PARAMOUT2.Substring(position, 10).Trim = "" Then
                                exitLoop = True
                            Else

                                'Create a new row
                                dRow = Nothing
                                dRow = dtLocations.NewRow
                                dRow("LocationId") = PARAMOUT2.Substring(position, 10)
                                dRow("Location") = PARAMOUT2.Substring(position + 10, 40).Trim()
                                Dim addThisRow As Boolean = True
                                For Each row As DataRow In dtLocations.Rows
                                    If row("LocationId").ToString() = PARAMOUT2.Substring(position, 10) Then
                                        addThisRow = False
                                        Exit For
                                    End If
                                Next
                                If addThisRow Then dtLocations.Rows.Add(dRow)

                                'Loop again
                                position = position + 50
                            End If
                        Loop
                    End If

                    'Extract the last used product sub type
                    lastProductType = PARAMOUT.Substring(10231, 1)
                    lastProductSubType = PARAMOUT.Substring(10232, 4)
                    If String.IsNullOrEmpty(lastProductType.Trim) OrElse String.IsNullOrEmpty(lastProductSubType.Trim) Then
                        moreRecords = False
                    End If
                Else
                    moreRecords = False
                End If

            Loop

        Catch ex As Exception
            ResultDataSet = Nothing
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBPD-WS070R"
                .HasError = True
            End With
        End Try

        Return err

    End Function

    Private Function CallWS070R(ByVal lastProductType As String, ByVal lastProductSubType As String, ByRef PARAMOUT2 As String) As String

        'Create command object
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strProgram As String = "WS070R"
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim &
                                    "/" & strProgram & "(@PARAM1, @PARAM2)"
        Dim parmIO As iDB2Parameter
        Dim parmIO2 As iDB2Parameter
        Dim PARAMOUT As String = String.Empty
        PARAMOUT2 = String.Empty

        'Set the connection string
        cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)

        'Populate the parameter
        parmIO = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 10240)
        parmIO.Value = WS070RParm(lastProductType, lastProductSubType)
        parmIO.Direction = ParameterDirection.InputOutput

        'Populate the parameter
        parmIO2 = cmdSELECT.Parameters.Add(Param2, iDB2DbType.iDB2Char, 10240)
        parmIO2.Value = Utilities.FixStringLength("", 10240)
        parmIO2.Direction = ParameterDirection.InputOutput

        'Execute
        TalentCommonLog("CallWS070R", "", "Backend Request: strHEADER=" & strHEADER & ", parmIO.Value=" & parmIO.Value)

        cmdSELECT.ExecuteNonQuery()
        PARAMOUT = cmdSELECT.Parameters(Param1).Value.ToString
        PARAMOUT2 = cmdSELECT.Parameters(Param2).Value.ToString

        TalentCommonLog("CallWS070R", "", "Backend Response: PARAMOUT=" & PARAMOUT)

        Return PARAMOUT

    End Function

    Private Function WS070RParm(ByVal lastProductType As String, ByVal lastProductSubType As String) As String

        Dim myString As String

        Dim Stadia() As String = De.StadiumCode.Split(",")
        Dim count As Integer = 0
        Dim stadiumList As New StringBuilder

        Do While count < Stadia.Length
            stadiumList.Append(Stadia(count))
            count = count + 1
        Loop

        Dim searchDate As String = String.Empty
        If Not String.IsNullOrEmpty(De.ProductDate.Trim) Then
            searchDate = DateToIseriesFormat(CType(De.ProductDate, Date))
        End If

        'Construct the parameter
        myString = Utilities.FixStringLength("", 10202) &
                    Utilities.FixStringLength(Settings.OriginatingSource, 10) &
                    Utilities.PadLeadingZeros(searchDate, 7) &
                    Utilities.FixStringLength(stadiumList.ToString, 12) &
                    Utilities.FixStringLength(lastProductType, 1) &
                    Utilities.FixStringLength(lastProductSubType, 4) &
                    Utilities.FixStringLength(Settings.OriginatingSourceCode, 1) &
                   Utilities.FixStringLength("", 3)

        Return myString

    End Function

    Private Function AccessDatabaseWS007R() As ErrorObj
        Dim err As New ErrorObj
        Dim dRow As DataRow = Nothing
        Dim PARAMOUT As String = String.Empty
        Dim PARAMOUT2 As String = String.Empty
        Dim DtStatusResults As New DataTable("StatusResults")
        Dim dtPriceBandResults As New DataTable("ProductPriceBands")
        Dim DtProdDetailsResults As New DataTable("ProductDetails")
        Dim dtPriceCodeResults As New DataTable("PriceCodes")
        Dim dtCampaignCodeResults As New DataTable("CampaignCodes")
        ResultDataSet = New DataSet
        CreateProductDetailsResultDataSet(DtStatusResults, dtPriceBandResults, DtProdDetailsResults, dtPriceCodeResults, dtCampaignCodeResults)

        Try

            'Call WS007R
            PARAMOUT = CallWS007R(PARAMOUT2)

            'Set the response data
            dRow = Nothing
            dRow = DtStatusResults.NewRow
            If PARAMOUT.Substring(3071, 1) = "E" Or PARAMOUT.Substring(3069, 2).Trim <> "" Then
                dRow("ErrorOccurred") = "E"
                dRow("ReturnCode") = PARAMOUT.Substring(3069, 2)
            Else
                dRow("ErrorOccurred") = ""
                dRow("ReturnCode") = ""
            End If
            DtStatusResults.Rows.Add(dRow)

            'No errors 
            If PARAMOUT.Substring(3071, 1) <> "E" And PARAMOUT.Substring(3069, 2).Trim = "" Then

                'Extract the prioce band data from the parameter
                Dim iPosBand As Integer = 0
                Dim iPosDescription As Integer = 36
                Dim iPosPriority As Integer = 1548
                Dim iPosPriceBandValue As Integer = 1584
                Dim iPosPriceBandMinQuantity As Integer = 2088
                Dim iPosPriceBandMaxQuantity As Integer = 1944
                Dim iPosPriceBandIsFamilyType As Integer = 2232

                Dim iCounter As Integer = 1
                Do While PARAMOUT2.Substring(iPosBand, 1).Trim <> String.Empty AndAlso iCounter < 37
                    'Create a new row
                    dRow = Nothing
                    dRow = dtPriceBandResults.NewRow
                    dRow("PriceBand") = PARAMOUT2.Substring(iPosBand, 1).Trim()
                    dRow("PriceBandDescription") = PARAMOUT2.Substring(iPosDescription, 40).Trim()
                    If PARAMOUT2.Substring(iPosPriority, 1) <> "Y" Then
                        dRow("PriceBandPriority") = PARAMOUT2.Substring(iPosPriority, 1).Trim()
                    Else
                        dRow("PriceBandPriority") = String.Empty
                    End If
                    dRow("PriceBandValue") = PARAMOUT2.Substring(iPosPriceBandValue, 10).Trim()
                    dRow("PriceBandMinQuantity") = PARAMOUT2.Substring(iPosPriceBandMinQuantity, 4).Trim()
                    dRow("PriceBandMaxQuantity") = PARAMOUT2.Substring(iPosPriceBandMaxQuantity, 4).Trim()
                    dRow("PriceBandIsFamilyType") = PARAMOUT2.Substring(iPosPriceBandIsFamilyType, 1).Trim()
                    dtPriceBandResults.Rows.Add(dRow)

                    'Increment
                    iPosBand = iPosBand + 1
                    iPosDescription = iPosDescription + 40
                    iPosPriority = iPosPriority + 1
                    iPosPriceBandValue = iPosPriceBandValue + 10
                    iPosPriceBandMinQuantity = iPosPriceBandMinQuantity + 4
                    iPosPriceBandMaxQuantity = iPosPriceBandMaxQuantity + 4
                    iPosPriceBandIsFamilyType = iPosPriceBandIsFamilyType + 1
                    iCounter = iCounter + 1
                Loop

                'Extract the Product Details from the parameter
                Dim iPosition As Integer = 0
                dRow = Nothing
                dRow = DtProdDetailsResults.NewRow
                dRow("ProductCode") = PARAMOUT.Substring(iPosition, 6).Trim()
                dRow("ProductText1") = PARAMOUT.Substring(iPosition + 6, 40)
                dRow("ProductText2") = PARAMOUT.Substring(iPosition + 46, 40)
                dRow("ProductText3") = PARAMOUT.Substring(iPosition + 86, 40)
                dRow("ProductText4") = PARAMOUT.Substring(iPosition + 126, 40)
                dRow("ProductText5") = PARAMOUT.Substring(iPosition + 166, 40)
                dRow("ProductDetail1") = PARAMOUT.Substring(iPosition + 206, 60)
                dRow("ProductDetail2") = PARAMOUT.Substring(iPosition + 266, 60)
                dRow("ProductDetail3") = PARAMOUT.Substring(iPosition + 326, 60)
                dRow("ProductDetail4") = PARAMOUT.Substring(iPosition + 386, 60)
                dRow("ProductDetail5") = PARAMOUT.Substring(iPosition + 446, 60)
                dRow("ProductDescription") = PARAMOUT.Substring(iPosition + 506, 40).Trim()
                dRow("ProductDate") = PARAMOUT.Substring(iPosition + 546, 19).Trim()
                dRow("ProductTime") = PARAMOUT.Substring(iPosition + 565, 7).Trim()
                dRow("ProductType") = PARAMOUT.Substring(iPosition + 572, 1).Trim()
                dRow("LoyalityOnOff") = PARAMOUT.Substring(iPosition + 573, 1).Trim()
                dRow("ProductReqdLoyalityPoints") = PARAMOUT.Substring(iPosition + 574, 5).Trim()
                dRow("MembLoyalityPointsTotal") = PARAMOUT.Substring(iPosition + 579, 5).Trim()
                dRow("MembFavStand") = PARAMOUT.Substring(iPosition + 584, 3).Trim()
                dRow("MembFavArea") = PARAMOUT.Substring(iPosition + 587, 4).Trim()
                dRow("UseVisualSeatLevelSelection") = convertToBool(PARAMOUT.Substring(iPosition + 1738, 1))
                dRow("CoReqProductGroup") = PARAMOUT.Substring(iPosition + 1740, 30).Trim()
                dRow("IsProductBundle") = (PARAMOUT.Substring(iPosition + 2960, 1) = "B")
                dRow("TemplateID") = PARAMOUT.Substring(iPosition + 2961, 13).Trim()
                dRow("IsSoldOut") = Utilities.convertToBool(PARAMOUT.Substring(iPosition + 2974, 1).Trim())
                dRow("HomeTeamCode") = PARAMOUT.Substring(iPosition + 2975, 4).Trim()
                dRow("AlternativeSeatSelection") = convertToBool(PARAMOUT.Substring(iPosition + 2981, 1))
                dRow("AlternativeSeatSelectionAcrossStands") = convertToBool(PARAMOUT.Substring(iPosition + 2982, 1))
                dRow("OppositionCode") = PARAMOUT.Substring(iPosition + 2984, 4).Trim()
                dRow("DefaultPriceBand") = PARAMOUT.Substring(iPosition + 2988, 1).Trim()
                dRow("HomeAsAway") = PARAMOUT.Substring(iPosition + 2989, 1).Trim()
                dRow("AvailableOnline") = Utilities.convertToBool(PARAMOUT.Substring(iPosition + 3010, 1))
                dRow("DefaultPriceCode") = PARAMOUT.Substring(iPosition + 3012, 2).Trim()
                dRow("ProductStadium") = PARAMOUT.Substring(iPosition + 3014, 2).Trim()
                dRow("CanStadiumUseFavouriteSeat") = Utilities.convertToBool(PARAMOUT.Substring(iPosition + 2979, 1))
                dRow("ProductSubType") = (PARAMOUT.Substring(iPosition + 3016, 4)).Trim
                dRow("AllowComments") = Utilities.convertToBool(PARAMOUT.Substring(iPosition + 3011, 1))
                dRow("RestrictGraphical") = (PARAMOUT.Substring(iPosition + 2322, 1) = "Y")
                dRow("LowPrice1") = PARAMOUT.Substring(iPosition + 2304, 9).Trim
                dRow("LowPrice2") = PARAMOUT.Substring(iPosition + 2313, 9).Trim
                dRow("LowPriceBand1") = PARAMOUT.Substring(iPosition + 2323, 1).Trim
                dRow("LowPriceBand2") = PARAMOUT.Substring(iPosition + 2324, 1).Trim
                If PARAMOUT.Substring(iPosition + 2325, 7).Trim() <> "0000000" Then
                    dRow("MDTE08Date") = PARAMOUT.Substring(iPosition + 2325, 7).Trim
                End If

                dRow("Sponsor") = PARAMOUT.Substring(iPosition + 2332, 4).Trim
                dRow("Location") = PARAMOUT.Substring(iPosition + 2336, 40).Trim
                dRow("Duration") = PARAMOUT.Substring(iPosition + 2376, 40).Trim
                dRow("FromAge") = PARAMOUT.Substring(iPosition + 2416, 3).Trim
                dRow("ToAge") = PARAMOUT.Substring(iPosition + 2419, 3).Trim
                dRow("Competition") = PARAMOUT.Substring(iPosition + 2422, 4).Trim
                dRow("isAdHocRefund") = convertToBool(PARAMOUT.Substring(iPosition + 2426, 1))
                dRow("TicketExchangeEnabled") = convertToBool(PARAMOUT.Substring(iPosition + 1769, 1))
                dRow("HideDate") = convertToBool(PARAMOUT.Substring(iPosition + 1770, 1))
                dRow("HideTime") = convertToBool(PARAMOUT.Substring(iPosition + 1771, 1))
                dRow("isDDRefundProduct") = convertToBool(PARAMOUT.Substring(iPosition + 1772, 1))
                dRow("AllowPriceBandAlterations") = PARAMOUT.Substring(iPosition + 1773, 1).Trim
                dRow("DefaultPriceBandForBasket") = PARAMOUT.Substring(iPosition + 1774, 1).Trim
                dRow("BundleStartDate") = PARAMOUT.Substring(iPosition + 2946, 7).Trim
                dRow("BundleEndDate") = PARAMOUT.Substring(iPosition + 2953, 7).Trim
                dRow("SVGStadiumDescription") = PARAMOUT.Substring(iPosition + 2447, 50).Trim
                If Not String.IsNullOrEmpty(PARAMOUT.Substring(iPosition + 2447, 50).Trim()) Then
                    De.SVGStadiumDescriptionAvailable = True
                Else
                    De.SVGStadiumDescriptionAvailable = False
                End If

                DtProdDetailsResults.Rows.Add(dRow)
                'Price Codes
                Dim totalCodesLength As Integer = PARAMOUT.Substring(iPosition + 2223, 40).Length - 2
                Dim tempCodes As String = PARAMOUT.Substring(iPosition + 2223, 40)
                Dim tempCodesFoc As String = PARAMOUT.Substring(iPosition + 2427, 20)
                If tempCodes.Length > 0 Then
                    For strIndex As Integer = 0 To (totalCodesLength / 2) Step 1
                        If tempCodes.Substring(strIndex * 2, 2).Trim.Length > 0 Then
                            dRow = Nothing
                            dRow = dtPriceCodeResults.NewRow
                            dRow("PriceCode") = tempCodes.Substring(strIndex * 2, 2).Trim
                            dRow("FreeOfCharge") = convertToBool(tempCodesFoc.Substring(strIndex, 1).Trim)
                            dtPriceCodeResults.Rows.Add(dRow)
                        End If
                    Next
                End If
                'Campaign Codes
                totalCodesLength = PARAMOUT.Substring(iPosition + 2263, 40).Length - 2
                tempCodes = PARAMOUT.Substring(iPosition + 2263, 40)
                If tempCodes.Length > 0 Then
                    For strIndex As Integer = 0 To totalCodesLength Step 2
                        If tempCodes.Substring(strIndex, 2).Trim.Length > 0 Then
                            dRow = Nothing
                            dRow = dtCampaignCodeResults.NewRow
                            dRow("CampaignCode") = tempCodes.Substring(strIndex, 2)
                            dtCampaignCodeResults.Rows.Add(dRow)
                        End If
                    Next
                End If
            End If

        Catch ex As Exception
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBPD-WS007R"
                .HasError = True
            End With
        End Try

        Return err

    End Function

    Private Function CallWS007R(ByRef PARAMOUT2 As String) As String
        'Create command object
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strProgram As String = "WS007R"
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & "/" & strProgram & "(@PARAM1,@PARAM2)"
        Dim parmIO As iDB2Parameter
        Dim parmIO2 As iDB2Parameter
        Dim PARAMOUT As String = String.Empty

        'Set the connection string
        cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)

        'Populate the parameter
        parmIO = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 3072)
        parmIO.Value = WS007Parm()
        parmIO.Direction = ParameterDirection.InputOutput

        parmIO2 = cmdSELECT.Parameters.Add(Param2, iDB2DbType.iDB2Char, 3072)
        parmIO2.Value = String.Empty
        parmIO2.Direction = ParameterDirection.InputOutput

        'Execute
        TalentCommonLog("CallWS007R", "", "Backend Request: strHEADER=" & strHEADER & ", parmIO.Value=" & parmIO.Value)
        cmdSELECT.ExecuteNonQuery()
        PARAMOUT = cmdSELECT.Parameters(Param1).Value.ToString
        PARAMOUT2 = cmdSELECT.Parameters(Param2).Value.ToString

        TalentCommonLog("CallWS007R", "", "Backend Response: PARAMOUT=" & PARAMOUT)
        Return PARAMOUT
    End Function

    Private Function WS007Parm() As String
        Dim myString As New StringBuilder
        Dim ats As String = String.Empty
        Dim getLowPrices As String = String.Empty
        If De.GetLowPrices Then
            getLowPrices = "Y"
        End If
        If De.Src = "S" Then ats = "Y"

        myString.Append(Utilities.FixStringLength(De.ProductCode, 6))
        myString.Append(Utilities.FixStringLength("", 1705))
        myString.Append(Utilities.FixStringLength(ats, 1))
        myString.Append(Utilities.FixStringLength("", 591))
        myString.Append(Utilities.FixStringLength(getLowPrices, 1))
        myString.Append(Utilities.FixStringLength("", 679))
        'myString.Append(Utilities.FixStringLength("", 1271))
        myString.Append(Utilities.FixStringLength(Utilities.ConvertToYN(De.AllowPriceException, True), 1)) 'allow price exception
        myString.Append(Utilities.FixStringLength("", 4)) 'opposition code
        myString.Append(Utilities.FixStringLength("", 1)) 'default price band
        myString.Append(Utilities.FixStringLength("", 1)) 'Home as away Y/N
        myString.Append(Utilities.FixStringLength(Settings.AuthorityUserProfile, 10))
        myString.Append(Utilities.FixStringLength(Settings.OriginatingSource, 10))
        myString.Append(Utilities.FixStringLength("", 1)) 'Availble online Y/N
        myString.Append(Utilities.FixStringLength("", 1)) 'Allow comments
        myString.Append(Utilities.FixStringLength(De.PriceCode, 2))
        myString.Append(Utilities.FixStringLength("", 54)) 'Stadium code, sub type, session id, customer number
        myString.Append(Utilities.FixStringLength(De.Src, 1))
        myString.Append("   ") 'Error code and error flag

        Return myString.ToString()
    End Function

    Private Sub CreateProductDetailsResultDataSet(ByRef DtStatusResults As DataTable, ByRef DtPriceBandResults As DataTable, ByRef DtProdDetailsResults As DataTable, ByRef dtPriceCodeResults As DataTable, ByRef dtCampaignCodeResults As DataTable)
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        'Create the product Price Band Details data table
        ResultDataSet.Tables.Add(DtPriceBandResults)
        With DtPriceBandResults.Columns
            .Add("PriceBand", GetType(String))
            .Add("PriceBandDescription", GetType(String))
            .Add("PriceBandPriority", GetType(String))
            .Add("PriceBandValue", GetType(String))
            .Add("PriceBandMinQuantity", GetType(String))
            .Add("PriceBandMaxQuantity", GetType(String))
            .Add("PriceBandIsFamilyType", GetType(String))
        End With

        'Create the product  Details data table
        ResultDataSet.Tables.Add(DtProdDetailsResults)
        With DtProdDetailsResults.Columns
            .Add("ProductCode", GetType(String))
            .Add("ProductText1", GetType(String))
            .Add("ProductText2", GetType(String))
            .Add("ProductText3", GetType(String))
            .Add("ProductText4", GetType(String))
            .Add("ProductText5", GetType(String))
            .Add("ProductDetail1", GetType(String))
            .Add("ProductDetail2", GetType(String))
            .Add("ProductDetail3", GetType(String))
            .Add("ProductDetail4", GetType(String))
            .Add("ProductDetail5", GetType(String))
            .Add("ProductDescription", GetType(String))
            .Add("ProductDate", GetType(String))
            .Add("ProductTime", GetType(String))
            .Add("ProductType", GetType(String))
            .Add("ProductSubType", GetType(String))
            .Add("LoyalityOnOff", GetType(String))
            .Add("ProductReqdLoyalityPoints", GetType(String))
            .Add("MembLoyalityPointsTotal", GetType(String))
            .Add("MembFavStand", GetType(String))
            .Add("MembFavArea", GetType(String))
            .Add("UseVisualSeatLevelSelection", GetType(Boolean))
            .Add("CoReqProductGroup", GetType(String))
            .Add("AlternativeSeatSelection", GetType(Boolean))
            .Add("AlternativeSeatSelectionAcrossStands", GetType(Boolean))
            .Add("HomeTeamCode", GetType(String))
            .Add("OppositionCode", GetType(String))
            .Add("DefaultPriceBand", GetType(String))
            .Add("HomeAsAway", GetType(String))
            .Add("AvailableOnline", GetType(Boolean))
            .Add("DefaultPriceCode", GetType(String))
            .Add("LoyaltyRequirementMet", GetType(String))
            .Add("LimitRequirementMet", GetType(String))
            .Add("ProductStadium", GetType(String))
            .Add("CanStadiumUseFavouriteSeat", GetType(Boolean))
            .Add("AllowComments", GetType(Boolean))
            .Add("IsSoldOut", GetType(Boolean))
            .Add("TemplateID", GetType(String))
            .Add("IsProductBundle", GetType(Boolean))
            .Add("RestrictGraphical", GetType(Boolean))
            .Add("LowPrice1", GetType(String))
            .Add("LowPrice2", GetType(String))
            .Add("LowPriceBand1", GetType(String))
            .Add("LowPriceBand2", GetType(String))
            .Add("MDTE08Date", GetType(String))
            .Add("Sponsor", GetType(String))
            .Add("Opposition", GetType(String))
            .Add("Location", GetType(String))
            .Add("Duration", GetType(String))
            .Add("FromAge", GetType(Integer))
            .Add("ToAge", GetType(Integer))
            .Add("Competition", GetType(String))
            .Add("isAdHocRefund", GetType(Boolean))
            .Add("TicketExchangeEnabled", GetType(Boolean))
            .Add("HideDate", GetType(Boolean))
            .Add("HideTime", GetType(Boolean))
            .Add("BundleStartDate", GetType(String))
            .Add("BundleEndDate", GetType(String))
            .Add("isDDRefundProduct", GetType(Boolean))
            .Add("AllowPriceBandAlterations", GetType(String))
            .Add("DefaultPriceBandForBasket", GetType(String))
            .Add("SVGStadiumDescription", GetType(String))

        End With

        ResultDataSet.Tables.Add(dtPriceCodeResults)
        With dtPriceCodeResults.Columns
            .Add("PriceCode", GetType(String))
            .Add("FreeOfCharge", GetType(Boolean))
        End With

        ResultDataSet.Tables.Add(dtCampaignCodeResults)
        With dtCampaignCodeResults.Columns
            .Add("CampaignCode", GetType(String))
        End With
    End Sub

    Private Function AccessDatabaseWS015R() As ErrorObj
        Dim err As New ErrorObj
        ResultDataSet = New DataSet
        ' Dim sLastRecord As String = "000"
        ' Dim sRecordTotal As String = "000"
        Dim bMoreRecords As Boolean = True
        Dim sLastRRN As String = String.Empty
        Dim sLastProduct As String = String.Empty
        Dim sLastDate As String = String.Empty
        Dim dRow As DataRow = Nothing
        Dim PARAMOUT As String = String.Empty

        'Create the Status data table
        Dim DtStatusResults As New DataTable("StatusResults")
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        'Create the product list data table
        Dim DtProductListResults As New DataTable("ProductListResults")
        ResultDataSet.Tables.Add(DtProductListResults)
        ProductListDataTable(DtProductListResults)

        Try
            'Loop until no more products available
            Do While bMoreRecords = True

                'Call WS015R
                PARAMOUT = CallWS015R(sLastProduct, sLastDate, sLastRRN, bMoreRecords)

                'Set the response data on the first call to WS015R
                If sLastProduct = String.Empty Then
                    dRow = Nothing
                    dRow = DtStatusResults.NewRow
                    If PARAMOUT.Substring(5119, 1) = "E" Or PARAMOUT.Substring(5117, 2).Trim <> "" Then
                        dRow("ErrorOccurred") = "E"
                        dRow("ReturnCode") = PARAMOUT.Substring(5117, 2)
                        bMoreRecords = False
                    Else
                        dRow("ErrorOccurred") = ""
                        dRow("ReturnCode") = ""
                    End If
                    DtStatusResults.Rows.Add(dRow)
                End If

                'No errors 
                If PARAMOUT.Substring(5119, 1) <> "E" And PARAMOUT.Substring(5117, 2).Trim = "" Then

                    'Extract the data from the parameter
                    Dim iPosition As Integer = 0
                    Dim iCounter As Integer = 1
                    Do While iCounter <= 12
                        'Has a product been returned
                        If PARAMOUT.Substring(iPosition, 6).Trim = "" Then
                            Exit Do
                        Else
                            'Create a new row
                            dRow = Nothing
                            dRow = DtProductListResults.NewRow
                            dRow("ProductCode") = PARAMOUT.Substring(iPosition, 6)
                            dRow("ProductDescription") = PARAMOUT.Substring(iPosition + 6, 40)
                            dRow("ProductType") = PARAMOUT.Substring(iPosition + 46, 1)
                            dRow("ProductSubType") = PARAMOUT.Substring(iPosition + 47, 4).Trim
                            dRow("ProductMDTE08") = PARAMOUT.Substring(iPosition + 51, 7)
                            dRow("ProductTime") = PARAMOUT.Substring(iPosition + 58, 7)
                            dRow("ProductOnSale") = PARAMOUT.Substring(iPosition + 65, 1)
                            dRow("ProductAvailForSale") = PARAMOUT.Substring(iPosition + 66, 1)
                            dRow("ProductPriceBand") = PARAMOUT.Substring(iPosition + 67, 1)
                            dRow("ProductTicketLimit") = PARAMOUT.Substring(iPosition + 68, 5)
                            dRow("ProductEntryTime") = PARAMOUT.Substring(iPosition + 73, 10)
                            dRow("ProductStadium") = PARAMOUT.Substring(iPosition + 83, 2)
                            dRow("ProductOppositionCode") = PARAMOUT.Substring(iPosition + 85, 4)
                            dRow("ProductHomeCode") = PARAMOUT.Substring(iPosition + 89, 4)
                            dRow("ProductCompetitionCode") = PARAMOUT.Substring(iPosition + 93, 4)
                            dRow("ProductSponsorCode") = PARAMOUT.Substring(iPosition + 97, 4)
                            dRow("ProductDetailCode") = PARAMOUT.Substring(iPosition + 101, 6)
                            dRow("ProductDescription2") = PARAMOUT.Substring(iPosition + 107, 40)
                            dRow("capacityCnt") = PARAMOUT.Substring(iPosition + 147, 7)
                            dRow("unavailableCnt") = PARAMOUT.Substring(iPosition + 154, 7)
                            dRow("reservationsCnt") = PARAMOUT.Substring(iPosition + 161, 7)
                            dRow("bookingsCnt") = PARAMOUT.Substring(iPosition + 168, 7)
                            dRow("salesCnt") = PARAMOUT.Substring(iPosition + 175, 7)
                            dRow("returnsCnt") = PARAMOUT.Substring(iPosition + 182, 7)
                            dRow("location") = PARAMOUT.Substring(iPosition + 189, 40)
                            dRow("ageRangeFrm") = PARAMOUT.Substring(iPosition + 229, 2)
                            dRow("ageRangeTo") = PARAMOUT.Substring(iPosition + 231, 2)
                            dRow("duration") = PARAMOUT.Substring(iPosition + 233, 40)
                            dRow("ProductDate") = PARAMOUT.Substring(iPosition + 273, 18)
                            dRow("ProductYear") = PARAMOUT.Substring(iPosition + 292, 4)
                            dRow("ProductReqdMem") = PARAMOUT.Substring(iPosition + 296, 6)
                            dRow("ProductReqdMemDesc") = PARAMOUT.Substring(iPosition + 302, 40)
                            dRow("ProductReqdMemOK") = PARAMOUT.Substring(iPosition + 342, 1)
                            dRow("LimitRequirementMet") = PARAMOUT.Substring(iPosition + 343, 1)
                            dRow("IsSoldOut") = AvailabilityTotal(dRow("capacityCnt").ToString.Trim, dRow("returnsCnt").ToString.Trim,
                                        dRow("salesCnt").ToString.Trim, dRow("unavailableCnt").ToString.Trim,
                                        dRow("bookingsCnt").ToString.Trim, dRow("reservationsCnt").ToString.Trim)
                            dRow("ProductReqdLoyalityPoints") = ""
                            dRow("CampaignCode") = ""
                            dRow("UseVisualSeatLevelSelection") = False
                            dRow("ProductHomeAsAway") = "N" 'Ensure that something exists in this column
                            dRow("PPSSchemeType") = String.Empty
                            dRow("HasAssociatedTravelProduct") = False
                            dRow("IsProductBundle") = False
                            dRow("RestrictGraphical") = False
                            dRow("ExcludeProductFromWebSales") = convertToBool(PARAMOUT.Substring(iPosition + 344, 1))
                            dRow("HideDate") = convertToBool(PARAMOUT.Substring(iPosition + 345, 1))
                            dRow("HideTime") = convertToBool(PARAMOUT.Substring(iPosition + 346, 1))
                            dRow("ProductSetAsSoldOut") = convertToBool(PARAMOUT.Substring(iPosition + 347, 1))
                            dRow("CorporateHospitalityProductCode") = String.Empty
                            DtProductListResults.Rows.Add(dRow)
                            'Increment
                            iPosition = iPosition + 375
                            iCounter = iCounter + 1
                        End If
                    Loop

                    'Extract the footer information
                    bMoreRecords = PARAMOUT.Substring(5115, 1)
                    sLastDate = PARAMOUT.Substring(5075, 7)
                    sLastProduct = PARAMOUT.Substring(5069, 6)
                    sLastRRN = PARAMOUT.Substring(5054, 15)
                End If

            Loop
        Catch ex As Exception
            ResultDataSet = Nothing
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBPD-WS015R"
                .HasError = True
            End With
        End Try
        Return err
    End Function

    Private Function AvailabilityTotal(ByVal capacityCount As String, ByVal returnsCount As String, ByVal salesCount As String,
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
        If total > 0 Then
            Return False
        Else
            Return True
        End If
    End Function

    Private Function CallWS015R(ByVal sLastProduct As String, ByVal sLastDate As String, ByVal slastRRN As String, ByVal sMore As String) As String

        'Create command object
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strProgram As String = "WS015R"
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & "/" & strProgram & "(@PARAM1)"
        Dim parmIO As iDB2Parameter
        Dim PARAMOUT As String = String.Empty

        'Set the connection string
        cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)

        'Populate the parameter
        parmIO = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 5120)
        parmIO.Value = WS015RParm(sLastProduct, sLastDate, slastRRN, sMore)
        parmIO.Direction = ParameterDirection.InputOutput

        'Execute
        TalentCommonLog("CallWS015R", "", "Backend Request: strHEADER=" & strHEADER & ", parmIO.Value=" & parmIO.Value)
        cmdSELECT.ExecuteNonQuery()
        PARAMOUT = cmdSELECT.Parameters(Param1).Value.ToString
        TalentCommonLog("CallWS015R", "", "Backend Response: PARAMOUT=" & PARAMOUT)

        Return PARAMOUT
    End Function

    Private Function WS015RParm(ByVal sLastProduct As String, ByVal slastDate As String, ByVal slastRRN As String, ByVal sMore As String) As String
        Dim myString As String
        Dim stadium0 As String = String.Empty
        Dim stadium1 As String = String.Empty
        Dim stadium2 As String = String.Empty
        Dim stadium3 As String = String.Empty
        Dim stadium4 As String = String.Empty
        Dim stadium5 As String = String.Empty
        Dim sStadia() As String = De.StadiumCode.Split(",")
        Dim count As Integer = 0

        Do While count < sStadia.Length
            Select Case count
                Case Is = 0
                    stadium0 = sStadia(0)
                Case Is = 1
                    stadium1 = sStadia(1)
                Case Is = 2
                    stadium2 = sStadia(2)
                Case Is = 3
                    stadium3 = sStadia(3)
                Case Is = 4
                    stadium4 = sStadia(4)
                Case Is = 5
                    stadium5 = sStadia(5)
            End Select
            count = count + 1
        Loop

        'Construct the parameter
        myString = Utilities.FixStringLength("", 4500) &
                    Utilities.FixStringLength(Settings.AuthorityUserProfile, 10) &
                    Utilities.FixStringLength("", 9) &
                    Utilities.FixStringLength(Settings.IsAgent.ConvertToISeriesYesNo, 1) &
                    Utilities.FixStringLength("", 35) &
                    Utilities.FixStringLength("", 498) &
                    Utilities.FixStringLength(De.ProductType, 1) &
                    Utilities.PadLeadingZeros(slastRRN, 15) &
                    Utilities.FixStringLength(sLastProduct, 6) &
                    Utilities.FixStringLength(slastDate, 7) &
                    Utilities.FixStringLength(De.ProductSubtype, 4) &
                    Utilities.FixStringLength(stadium0, 2) &
                    Utilities.FixStringLength(stadium1, 2) &
                    Utilities.FixStringLength(stadium2, 2) &
                    Utilities.FixStringLength(stadium3, 2) &
                    Utilities.FixStringLength(stadium4, 2) &
                    Utilities.FixStringLength(stadium5, 2) &
                    Utilities.FixStringLength(De.CustomerNumber, 12) &
                     Utilities.FixStringLength("", 5) &
                    Utilities.FixStringLength(sMore, 1) &
                    Utilities.FixStringLength(De.Src, 1) &
                    "   "
        Return myString
    End Function

    Private Function AccessDatabaseWS016R() As ErrorObj

        Dim err As New ErrorObj
        ResultDataSet = New DataSet
        Dim sLastRecord As String = "000"
        Dim sRecordTotal As String = "000"
        Dim bMoreRecords As Boolean = True
        Dim dRow As DataRow = Nothing
        Dim PARAMOUT As String = String.Empty

        'Create the Status data table
        Dim DtStatusResults As New DataTable("StatusResults")
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        'Create the product list data table
        Dim DtProductListResults As New DataTable("ProductListResults")
        ResultDataSet.Tables.Add(DtProductListResults)
        ProductListDataTable(DtProductListResults)

        Dim dtStadiumList As New DataTable("StadiumList")
        ResultDataSet.Tables.Add(dtStadiumList)
        With dtStadiumList.Columns
            .Add("StadiumCode", GetType(String))
            .Add("FavouriteSeatSelectionEnabled", GetType(Boolean))
        End With

        Try

            'Loop until no more products available
            Do While bMoreRecords = True

                'Call WS016R
                PARAMOUT = CallWS016R(sRecordTotal, sLastRecord)

                'Set the response data on the first call to WS016R
                If sLastRecord = "000" Then
                    dRow = Nothing
                    dRow = DtStatusResults.NewRow
                    If PARAMOUT.Substring(3071, 1) = "E" Or PARAMOUT.Substring(3069, 2).Trim <> "" Then
                        dRow("ErrorOccurred") = "E"
                        dRow("ReturnCode") = PARAMOUT.Substring(3069, 2)
                        bMoreRecords = False
                    Else
                        dRow("ErrorOccurred") = ""
                        dRow("ReturnCode") = ""
                    End If
                    DtStatusResults.Rows.Add(dRow)
                End If

                'No errors 
                If PARAMOUT.Substring(3071, 1) <> "E" And PARAMOUT.Substring(3069, 2).Trim = "" Then

                    'Extract the data from the parameter
                    Dim iPosition As Integer = 0
                    Dim iPosition2 As Integer = 1000
                    Dim iCounter As Integer = 1
                    Do While iCounter <= 12

                        ' Has a product been returned
                        If PARAMOUT.Substring(iPosition, 6).Trim() = "" Then
                            Exit Do
                        Else

                            'Create a new row
                            dRow = Nothing
                            dRow = DtProductListResults.NewRow
                            dRow("ProductCode") = PARAMOUT.Substring(iPosition, 6).Trim()
                            dRow("ProductDescription") = PARAMOUT.Substring(iPosition + 6, 40).Trim()
                            dRow("ProductDate") = PARAMOUT.Substring(iPosition + 46, 19).Trim()
                            dRow("ProductYear") = PARAMOUT.Substring(iPosition + 65, 4)
                            dRow("ProductTime") = PARAMOUT.Substring(iPosition + 69, 7).Trim()
                            dRow("ProductOnSale") = PARAMOUT.Substring(iPosition + 76, 1)
                            dRow("ProductAvailForSale") = PARAMOUT.Substring(iPosition + 77, 1)
                            dRow("ProductReqdMem") = PARAMOUT.Substring(iPosition + 78, 6).Trim()
                            dRow("ProductReqdMemDesc") = PARAMOUT.Substring(iPosition + 84, 40).Trim()
                            dRow("ProductReqdMemOK") = PARAMOUT.Substring(iPosition + 124, 1)
                            dRow("ProductType") = PARAMOUT.Substring(iPosition + 125, 1)
                            dRow("ProductReqdLoyalityPoints") = PARAMOUT.Substring(iPosition + 126, 5)
                            dRow("ProductMDTE08") = PARAMOUT.Substring(iPosition + 131, 7)
                            dRow("ProductPriceBand") = PARAMOUT.Substring(iPosition + 138, 1)
                            dRow("ProductTicketLimit") = PARAMOUT.Substring(iPosition + 139, 5)
                            dRow("ProductEntryTime") = PARAMOUT.Substring(iPosition + 144, 10).Trim()
                            dRow("ProductAssocTravelProd") = PARAMOUT.Substring(iPosition + 154, 6).Trim()
                            dRow("ProductOppositionCode") = PARAMOUT.Substring(iPosition + 160, 4).Trim()
                            dRow("ProductCompetitionCode") = PARAMOUT.Substring(iPosition + 214, 4).Trim()
                            dRow("ProductDescription2") = PARAMOUT.Substring(iPosition + 165, 40).Trim()
                            dRow("ProductStadium") = PARAMOUT.Substring(iPosition + 205, 2)
                            dRow("CampaignCode") = PARAMOUT.Substring(iPosition + 207, 2).Trim()
                            dRow("ProductSponsorCode") = PARAMOUT.Substring(iPosition + 209, 4).Trim()
                            dRow("UseVisualSeatLevelSelection") = convertToBool(PARAMOUT.Substring(iPosition + 213, 1))
                            dRow("PPSSchemeType") = String.Empty
                            dRow("LoyaltyRequirementMet") = PARAMOUT.Substring(iPosition + 218, 1)
                            dRow("LimitRequirementMet") = PARAMOUT.Substring(iPosition + 219, 1)
                            dRow("ProductHomeAsAway") = PARAMOUT.Substring(iPosition + 220, 1)
                            dRow("ProductSubType") = PARAMOUT.Substring(iPosition + 221, 4).Trim()
                            dRow("HasAssociatedTravelProduct") = convertToBool(PARAMOUT.Substring(iPosition + 225, 1).Trim())
                            dRow("IsSoldOut") = convertToBool(PARAMOUT.Substring(iPosition + 226, 1).Trim())
                            dRow("PriceCode") = PARAMOUT.Substring(iPosition + 227, 2).Trim()
                            dRow("AlternativeSeatSelection") = convertToBool(PARAMOUT.Substring(iPosition + 229, 1))
                            dRow("AlternativeSeatSelectionAcrossStands") = convertToBool(PARAMOUT.Substring(iPosition + 230, 1))
                            dRow("IsProductBundle") = (PARAMOUT.Substring(iPosition + 231, 1) = "B")
                            dRow("RestrictGraphical") = (PARAMOUT.Substring(iPosition + 232, 1) = "Y")
                            If Not String.IsNullOrWhiteSpace(PARAMOUT2.Substring(iPosition2, 30)) Then
                                dRow("ProductStadiumDescription") = PARAMOUT2.Substring(iPosition2, 30).Trim()
                                dRow("location") = PARAMOUT2.Substring(iPosition2 + 30, 40).Trim()
                                dRow("BundleStartDate") = PARAMOUT2.Substring(iPosition2 + 70, 7).Trim()
                                dRow("BundleEndDate") = PARAMOUT2.Substring(iPosition2 + 77, 7).Trim()
                                dRow("ProductCompetitionDesc") = PARAMOUT2.Substring(iPosition2 + 84, 20).Trim()
                                dRow("ProductSubTypeDesc") = PARAMOUT2.Substring(iPosition2 + 104, 30).Trim()
                                dRow("ProductOppositionDesc") = PARAMOUT2.Substring(iPosition2 + 134, 30).Trim()
                                dRow("CorporateHospitalityProductCode") = PARAMOUT2.Substring(iPosition2 + 164, 6).Trim()
                                dRow("ProductLinkedToHospitality") = PARAMOUT2.Substring(iPosition2 + 170, 1).Trim()
                            End If
                            dRow("ExcludeProductFromWebSales") = convertToBool(PARAMOUT.Substring(iPosition + 233, 1))
                            dRow("HideDate") = convertToBool(PARAMOUT.Substring(iPosition + 234, 1))
                            dRow("HideTime") = convertToBool(PARAMOUT.Substring(iPosition + 235, 1))
                            DtProductListResults.Rows.Add(dRow)

                            'Increment
                            iPosition = iPosition + 250
                            iPosition2 = iPosition2 + 250
                            iCounter = iCounter + 1

                        End If
                    Loop

                    'Extract the footer information
                    sLastRecord = PARAMOUT.Substring(3065, 3)
                    sRecordTotal = PARAMOUT.Substring(3062, 3)
                    If CInt(sLastRecord) >= CInt(sRecordTotal) Then
                        bMoreRecords = False
                    End If
                Else
                    bMoreRecords = False
                End If

            Loop

            If Not String.IsNullOrWhiteSpace(PARAMOUT2) Then
                Dim counter As Integer = 0
                Do While counter < 5 And counter < sStadia.Length
                    dRow = Nothing
                    dRow = dtStadiumList.NewRow
                    dRow("StadiumCode") = sStadia(counter)
                    dRow("FavouriteSeatSelectionEnabled") = convertToBool(PARAMOUT2.Substring(23 + counter, 1))
                    dtStadiumList.Rows.Add(dRow)
                    counter += 1
                Loop
            End If

        Catch ex As Exception
            ResultDataSet = Nothing
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBPD-WS016R"
                .HasError = True
            End With
        End Try

        Return err

    End Function

    Private Function AccessDatabaseWS609R() As ErrorObj

        Dim err As New ErrorObj
        ResultDataSet = New DataSet
        Dim sLastRecord As String = "000"
        Dim sRecordTotal As String = "000"
        Dim bMoreRecords As Boolean = True
        Dim dRow As DataRow = Nothing
        Dim PARAMOUT As String = String.Empty

        'Create the Status data table
        Dim DtStatusResults As New DataTable
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        'Create the product list data table
        Dim DtProductListResults As New DataTable
        Dim DtPreReqResults As New DataTable
        Dim DtLoyaltyResults As New DataTable
        Dim DtLoyaltySchedule As New DataTable
        Dim DtPreReqGroupData As New DataTable
        ResultDataSet.Tables.Add(DtProductListResults)
        ResultDataSet.Tables.Add(DtLoyaltySchedule)
        ResultDataSet.Tables.Add(DtPreReqGroupData)
        ProductListDataTable(DtProductListResults)
        LoyaltyScheduleTable(DtLoyaltySchedule)
        PreReqGroupDataTable(DtPreReqGroupData)

        Try

            'Loop until no more products available
            Do While bMoreRecords = True

                'Call WS609R
                PARAMOUT = CallWS609R(sRecordTotal, sLastRecord)

                'Set the response data on the first call to WS609R
                If sLastRecord = "000" Then
                    dRow = Nothing
                    dRow = DtStatusResults.NewRow
                    If PARAMOUT.Substring(30071, 1) = "E" Or PARAMOUT.Substring(30069, 2).Trim <> "" Then
                        dRow("ErrorOccurred") = "E"
                        dRow("ReturnCode") = PARAMOUT.Substring(30069, 2)
                        bMoreRecords = False
                    Else
                        dRow("ErrorOccurred") = ""
                        dRow("ReturnCode") = ""
                    End If
                    DtStatusResults.Rows.Add(dRow)
                End If

                'No errors 
                If PARAMOUT.Substring(30071, 1) <> "E" And PARAMOUT.Substring(30069, 2).Trim = "" Then

                    'Extract the data from the parameter
                    Dim iPosition As Integer = 0
                    Dim iCounter As Integer = 1
                    Do While iCounter <= 12

                        ' Has a product been returned
                        If PARAMOUT.Substring(iPosition, 6).Trim = "" Then
                            Exit Do
                        Else

                            'Create a new row
                            dRow = Nothing
                            dRow = DtProductListResults.NewRow
                            dRow("ProductCode") = PARAMOUT.Substring(iPosition, 6)
                            dRow("ProductDescription") = PARAMOUT.Substring(iPosition + 6, 40)
                            dRow("ProductDate") = PARAMOUT.Substring(iPosition + 46, 19)
                            dRow("ProductYear") = PARAMOUT.Substring(iPosition + 65, 4)
                            dRow("ProductTime") = PARAMOUT.Substring(iPosition + 69, 7)
                            dRow("ProductOnSale") = PARAMOUT.Substring(iPosition + 76, 1)
                            dRow("ProductAvailForSale") = PARAMOUT.Substring(iPosition + 77, 1)
                            dRow("ProductReqdMem") = PARAMOUT.Substring(iPosition + 78, 6)
                            dRow("ProductReqdMemDesc") = PARAMOUT.Substring(iPosition + 84, 40)
                            dRow("ProductReqdMemOK") = PARAMOUT.Substring(iPosition + 124, 1)
                            dRow("ProductType") = PARAMOUT.Substring(iPosition + 125, 1)
                            dRow("ProductReqdLoyalityPoints") = PARAMOUT.Substring(iPosition + 126, 5)
                            dRow("ProductMDTE08") = PARAMOUT.Substring(iPosition + 131, 7)
                            dRow("ProductPriceBand") = PARAMOUT.Substring(iPosition + 138, 1)
                            dRow("ProductTicketLimit") = PARAMOUT.Substring(iPosition + 139, 5)
                            dRow("ProductEntryTime") = PARAMOUT.Substring(iPosition + 144, 10)
                            dRow("ProductAssocTravelProd") = PARAMOUT.Substring(iPosition + 154, 6)
                            dRow("HasAssociatedTravelProduct") = False
                            dRow("ProductOppositionCode") = PARAMOUT.Substring(iPosition + 160, 4)
                            'dRow("ProductCompetitionCode") = PARAMOUT.Substring(iPosition + 164, 1)
                            dRow("ProductCompetitionCode") = PARAMOUT.Substring(iPosition + 214, 4)
                            dRow("ProductDescription2") = PARAMOUT.Substring(iPosition + 165, 40)
                            dRow("ProductStadium") = PARAMOUT.Substring(iPosition + 205, 2)
                            dRow("CampaignCode") = PARAMOUT.Substring(iPosition + 207, 2)
                            dRow("ProductSponsorCode") = PARAMOUT.Substring(iPosition + 209, 4)
                            dRow("UseVisualSeatLevelSelection") = convertToBool(PARAMOUT.Substring(iPosition + 213, 1))
                            dRow("PPSSchemeType") = String.Empty
                            dRow("LoyaltyRequirementMet") = PARAMOUT.Substring(iPosition + 218, 1)
                            dRow("LimitRequirementMet") = PARAMOUT.Substring(iPosition + 219, 1)
                            dRow("IsProductBundle") = False

                            'prerequisite data
                            dRow("PreReqProductGroup") = PARAMOUT.Substring(iPosition + 499, 6)
                            dRow("PreReqDescription") = PARAMOUT.Substring(iPosition + 505, 40)
                            dRow("PreReqMultiGroup") = PARAMOUT.Substring(iPosition + 545, 1)
                            dRow("PreReqStadium") = PARAMOUT.Substring(iPosition + 546, 2)
                            dRow("PreReqValidationRule") = PARAMOUT.Substring(iPosition + 548, 4)
                            dRow("PreReqComments") = PARAMOUT.Substring(iPosition + 552, 47)
                            'dRow("PreReqComments") = String.Empty

                            Dim iPosition2 = iPosition
                            For iCounter2 As Integer = 1 To 15
                                If PARAMOUT.Substring(iPosition2 + 599, 6).Trim = "" Then
                                    Exit For
                                Else
                                    Dim dRow2 As DataRow
                                    dRow2 = DtPreReqGroupData.NewRow
                                    dRow2("ProductCode") = PARAMOUT.Substring(iPosition, 6)
                                    dRow2("PreReqProductCode") = PARAMOUT.Substring(iPosition2 + 599, 6)
                                    dRow2("PreReqProductDescription") = PARAMOUT.Substring(iPosition2 + 605, 40)
                                    dRow2("PreReqProductType") = PARAMOUT.Substring(iPosition2 + 645, 1)
                                    dRow2("PreReqProductDate") = PARAMOUT.Substring(iPosition2 + 646, 7)
                                    DtPreReqGroupData.Rows.Add(dRow2)
                                    iPosition2 = iPosition2 + 54
                                End If
                            Next iCounter2

                            'loyalty data
                            dRow("LoyaltyDetailsApplyRestriction") = PARAMOUT.Substring(iPosition + 1499, 1)
                            dRow("LoyaltyDetailsNoOfPointsAwarded") = PARAMOUT.Substring(iPosition + 1500, 5)
                            dRow("LoyaltyDetailsUpdatePreviouslyAwardedPoints") = PARAMOUT.Substring(iPosition + 1505, 1)
                            dRow("LoyaltyDetailsUpdateFromDate") = PARAMOUT.Substring(iPosition + 1506, 7)
                            dRow("LoyaltyDetailsUpdateToDate") = PARAMOUT.Substring(iPosition + 1513, 7)
                            dRow("LoyaltyDetailsNoOfPurchasePointsAwarded") = PARAMOUT.Substring(iPosition + 1520, 5)
                            dRow("LoyaltyDetailsAwardToSeasonTicketHolders") = PARAMOUT.Substring(iPosition + 1525, 1)
                            dRow("LoyaltyDetailsSeasonTicketID") = PARAMOUT.Substring(iPosition + 1526, 5)

                            Dim iPosition3 = iPosition
                            For iCount3 As Integer = 1 To 10
                                If PARAMOUT.Substring(iPosition3 + 1999, 7).Trim = "" Then
                                    Exit For
                                Else
                                    Dim dRow3 As DataRow
                                    dRow3 = DtLoyaltySchedule.NewRow
                                    dRow3("ProductCode") = PARAMOUT.Substring(iPosition, 6)
                                    dRow3("From") = PARAMOUT.Substring(iPosition3 + 1999, 7)
                                    dRow3("RequiredPoints") = PARAMOUT.Substring(iPosition3 + 2006, 5)
                                    dRow3("Time") = PARAMOUT.Substring(iPosition3 + 2011, 4)
                                    iPosition3 += 16
                                    DtLoyaltySchedule.Rows.Add(dRow3)
                                End If
                            Next iCount3
                            dRow("CorporateHospitalityProductCode") = String.Empty
                            DtProductListResults.Rows.Add(dRow)

                            'Increment
                            iPosition = iPosition + 2500
                            iCounter = iCounter + 1

                        End If
                    Loop

                    'Extract the footer information
                    sLastRecord = PARAMOUT.Substring(30065, 3)
                    sRecordTotal = PARAMOUT.Substring(30062, 3)
                    If CInt(sLastRecord) >= CInt(sRecordTotal) Then
                        bMoreRecords = False
                    End If
                Else
                    bMoreRecords = False
                End If

            Loop

        Catch ex As Exception
            ResultDataSet = Nothing
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBPD-WS609R"
                .HasError = True
            End With
        End Try

        Return err

    End Function

    Private Function CallWS016R(ByVal sRecordTotal As String, ByVal sLastRecord As String) As String

        'Create command object
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strProgram As String = "WS016R"
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim &
                                    "/" & strProgram & "(@PARAM1,@PARAM2)"
        Dim parmIO, parmIO2 As iDB2Parameter
        Dim PARAMOUT As String = String.Empty

        'Set the connection string
        cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)

        'Populate the parameter
        parmIO = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 3072)
        parmIO.Value = WS016Parm(sRecordTotal, sLastRecord)
        parmIO.Direction = ParameterDirection.InputOutput

        parmIO2 = cmdSELECT.Parameters.Add(Param2, iDB2DbType.iDB2Char, 5120)
        parmIO2.Value = WS016Parm2(sRecordTotal, sLastRecord)
        parmIO2.Direction = ParameterDirection.InputOutput

        'Execute
        TalentCommonLog("CallWS016R", "", "Backend Request: strHEADER=" & strHEADER & ", parmIO.Value=" & parmIO.Value & ", parmIO2.Value=" & parmIO2.Value)

        cmdSELECT.ExecuteNonQuery()
        PARAMOUT = cmdSELECT.Parameters(Param1).Value.ToString
        PARAMOUT2 = cmdSELECT.Parameters(Param2).Value.ToString
        ws016rLastPriceCode = PARAMOUT2.Substring(20, 3)

        TalentCommonLog("CallWS016R", "", "Backend Response: PARAMOUT=" & PARAMOUT)

        Return PARAMOUT

    End Function

    Private Function CallWS609R(ByVal sRecordTotal As String, ByVal sLastRecord As String) As String

        'Create command object
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strProgram As String = "WS609R"
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim &
                                    "/" & strProgram & "(@PARAM1)"
        Dim parmIO As iDB2Parameter
        Dim PARAMOUT As String = String.Empty

        'Set the connection string
        cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)

        'Populate the parameter
        parmIO = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 30074)
        parmIO.Value = WS609Parm(sRecordTotal, sLastRecord)
        parmIO.Direction = ParameterDirection.InputOutput

        'Execute
        TalentCommonLog("CallWS609R", "", "Backend Request: strHEADER=" & strHEADER & ", parmIO.Value=" & parmIO.Value)

        cmdSELECT.ExecuteNonQuery()
        PARAMOUT = cmdSELECT.Parameters(Param1).Value.ToString

        TalentCommonLog("CallWS609R", "", "Backend Response: PARAMOUT=" & PARAMOUT)

        Return PARAMOUT

    End Function

    Private Function WS016Parm(ByVal sRecordTotal As String, ByVal sLastRecord As String) As String
        Dim myString As New StringBuilder
        Dim stadium0 As String = String.Empty
        Dim stadium1 As String = String.Empty
        Dim stadium2 As String = String.Empty
        Dim stadium3 As String = String.Empty
        Dim stadium4 As String = String.Empty
        Dim stadium5 As String = String.Empty

        sStadia = De.StadiumCode.Split(",")
        Dim count As Integer = 0
        Do While count < sStadia.Length
            Select Case count
                Case Is = 0 : stadium0 = sStadia(0)
                Case Is = 1 : stadium1 = sStadia(1)
                Case Is = 2 : stadium2 = sStadia(2)
                Case Is = 3 : stadium3 = sStadia(3)
                Case Is = 4 : stadium4 = sStadia(4)
                Case Is = 5 : stadium5 = sStadia(5)
            End Select
            count = count + 1
        Loop

        'Construct the parameter
        myString.Append(Utilities.FixStringLength("", 3038))
        myString.Append(Utilities.FixStringLength(stadium0, 2))
        myString.Append(Utilities.FixStringLength(stadium1, 2))
        myString.Append(Utilities.FixStringLength(stadium2, 2))
        myString.Append(Utilities.FixStringLength(stadium3, 2))
        myString.Append(Utilities.FixStringLength(stadium4, 2))
        myString.Append(Utilities.FixStringLength(stadium5, 2))
        myString.Append(Utilities.FixStringLength(De.CustomerNumber, 12))
        myString.Append(Utilities.FixStringLength(sRecordTotal, 3))
        myString.Append(Utilities.FixStringLength(sLastRecord, 3))
        myString.Append(Utilities.FixStringLength(De.Src, 1))
        myString.Append(Utilities.FixStringLength(String.Empty, 3))

        Return myString.ToString()
    End Function

    Private Function WS016Parm2(ByVal sRecordTotal As String, ByVal sLastRecord As String) As String
        Dim myString As New StringBuilder
        myString.Append(Utilities.FixStringLength(Settings.AuthorityUserProfile, 10))
        myString.Append(Utilities.FixStringLength(Settings.IsAgent.ConvertToISeriesYesNo, 1))
        myString.Append(Utilities.FixStringLength("", 9))
        myString.Append(Utilities.FixStringLength(ws016rLastPriceCode, 3))
        myString.Append(Utilities.FixStringLength(" ", 7))
        myString.Append(Utilities.FixStringLength(ConvertToYN(De.PriceAndAreaSelection), 1))
        myString.Append(Utilities.FixStringLength(De.ProductType, 1))
        myString.Append(Utilities.FixStringLength(De.ProductSubtype, 4))
        myString.Append(Utilities.FixStringLength(" ", 5092))
        Return myString.ToString()
    End Function

    Private Function WS609Parm(ByVal sRecordTotal As String, ByVal sLastRecord As String) As String
        Dim myString As New StringBuilder
        Dim stadium0 As String = String.Empty
        Dim stadium1 As String = String.Empty
        Dim stadium2 As String = String.Empty
        Dim stadium3 As String = String.Empty
        Dim stadium4 As String = String.Empty
        Dim stadium5 As String = String.Empty
        Dim sStadia() As String = De.StadiumCode.Split(",")
        Dim count As Integer = 0

        Do While count < sStadia.Length
            Select Case count
                Case Is = 0 : stadium0 = sStadia(0)
                Case Is = 1 : stadium1 = sStadia(1)
                Case Is = 2 : stadium2 = sStadia(2)
                Case Is = 3 : stadium3 = sStadia(3)
                Case Is = 4 : stadium4 = sStadia(4)
                Case Is = 5 : stadium5 = sStadia(5)
            End Select
            count = count + 1
        Loop

        'Construct the parameter
        myString.Append(Utilities.FixStringLength(String.Empty, 30038))
        myString.Append(Utilities.FixStringLength(stadium0, 2))
        myString.Append(Utilities.FixStringLength(stadium1, 2))
        myString.Append(Utilities.FixStringLength(stadium2, 2))
        myString.Append(Utilities.FixStringLength(stadium3, 2))
        myString.Append(Utilities.FixStringLength(stadium4, 2))
        myString.Append(Utilities.FixStringLength(stadium5, 2))
        myString.Append(Utilities.FixStringLength(De.CustomerNumber, 12))
        myString.Append(Utilities.PadLeadingZeros(sRecordTotal, 3))
        myString.Append(Utilities.PadLeadingZeros(sLastRecord, 3))
        myString.Append(Utilities.FixStringLength(De.Src, 1))
        myString.Append(Utilities.FixStringLength(String.Empty, 2))
        myString.Append(Utilities.FixStringLength(String.Empty, 1))
        myString.Append(Utilities.FixStringLength("Y", 1))
        myString.Append(Utilities.FixStringLength("Y", 1))

        Return myString.ToString()
    End Function

    Private Function AccessDatabaseWS017R() As ErrorObj

        Dim err As New ErrorObj
        ResultDataSet = New DataSet
        Dim sLastRecord As String = "000"
        Dim sRecordTotal As String = "000"
        Dim bMoreRecords As Boolean = True
        Dim dRow As DataRow = Nothing
        Dim PARAMOUT As String = String.Empty

        'Create the Status data table
        Dim DtStatusResults As New DataTable
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
            .Add("StadiumCode", GetType(String))
        End With

        'Create the stand details data table
        Dim DtStandDetailsResults As New DataTable
        ResultDataSet.Tables.Add(DtStandDetailsResults)
        With DtStandDetailsResults.Columns
            .Add("StandCode", GetType(String))
            .Add("StandDescription", GetType(String))
            .Add("AreaCode", GetType(String))
            .Add("AreaDescription", GetType(String))
            .Add("Roving", GetType(String))
            .Add("TicketExchangeAllowPurchase", GetType(Boolean))
            '            .Add("LowestPrice", GetType(String))
            '            .Add("HighestPrice", GetType(String))
        End With

        Try

            'Loop until no more products available
            Do While bMoreRecords = True

                'Call WS017R
                PARAMOUT = CallWS017R(sRecordTotal, sLastRecord)

                'Set the response data on the first call to WS016R
                If sLastRecord = "000" Then
                    dRow = Nothing
                    dRow = DtStatusResults.NewRow
                    If PARAMOUT.Substring(3071, 1) = "E" Or PARAMOUT.Substring(3069, 2).Trim <> "" Then
                        dRow("ErrorOccurred") = "E"
                        dRow("ReturnCode") = PARAMOUT.Substring(3069, 2)
                        dRow("StadiumCode") = ""
                        bMoreRecords = False
                    Else
                        dRow("ErrorOccurred") = ""
                        dRow("ReturnCode") = ""
                        dRow("StadiumCode") = PARAMOUT.Substring(3018, 2)
                    End If
                    DtStatusResults.Rows.Add(dRow)
                End If

                'No errors 
                If PARAMOUT.Substring(3071, 1) <> "E" And PARAMOUT.Substring(3069, 2).Trim = "" Then

                    'Extract the data from the parameter
                    Dim iPosition As Integer = 0
                    Dim iCounter As Integer = 1
                    Do While iCounter <= 294

                        ' Has a product been returned
                        If PARAMOUT.Substring(iPosition, 3).Trim = "" Then
                            Exit Do
                        Else
                            'Create a new row
                            dRow = Nothing
                            dRow = DtStandDetailsResults.NewRow
                            dRow("StandCode") = PARAMOUT.Substring(iPosition, 3).Trim
                            dRow("StandDescription") = ""
                            dRow("AreaCode") = PARAMOUT.Substring(iPosition + 3, 4).Trim
                            dRow("AreaDescription") = ""
                            dRow("Roving") = PARAMOUT.Substring(iPosition + 7, 1).Trim
                            dRow("TicketExchangeAllowPurchase") = convertToBool(PARAMOUT.Substring(iPosition + 8, 1).Trim)
                            DtStandDetailsResults.Rows.Add(dRow)

                            'Increment
                            iPosition = iPosition + 10
                            iCounter = iCounter + 1
                        End If
                    Loop

                    'Extract the footer information
                    sLastRecord = PARAMOUT.Substring(3065, 3)
                    sRecordTotal = PARAMOUT.Substring(3062, 3)
                    If CInt(sLastRecord) >= CInt(sRecordTotal) Then
                        bMoreRecords = False
                    End If
                Else
                    bMoreRecords = False
                End If
            Loop

        Catch ex As Exception
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBPD-WS017R"
                .HasError = True
            End With
        End Try

        Return err
    End Function

    Private Function CallWS017R(ByVal sRecordTotal As String, ByVal sLastRecord As String) As String
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & "/WS017R(@PARAM1)"
        Dim parmIO As iDB2Parameter
        Dim PARAMOUT As String = String.Empty

        cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)
        parmIO = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 3072)
        parmIO.Value = WS017Parm(sRecordTotal, sLastRecord)
        parmIO.Direction = ParameterDirection.InputOutput

        'Execute
        TalentCommonLog("CallWS017R", "", "Backend Request: strHEADER=" & strHEADER & ", parmIO.Value=" & parmIO.Value)
        cmdSELECT.ExecuteNonQuery()
        PARAMOUT = cmdSELECT.Parameters(Param1).Value.ToString
        TalentCommonLog("CallWS017R", "", "Backend Response: PARAMOUT=" & PARAMOUT)

        Return PARAMOUT
    End Function

    Private Function WS017Parm(ByVal sRecordTotal As String, ByVal sLastRecord As String) As String
        Dim myString As New StringBuilder
        Dim productHomeAsAway As String = De.ProductHomeAsAway

        myString.Append(Utilities.FixStringLength(String.Empty, 2940))
        myString.Append(Utilities.FixStringLength(De.CampaignCode, 2))
        myString.Append(Utilities.FixStringLength(productHomeAsAway, 1))
        myString.Append(Utilities.PadLeadingZeros(_de.PriceBreakId, 13))
        myString.Append(Utilities.FixStringLength(String.Empty, 45))
        myString.Append(Utilities.FixStringLength(ConvertToYN(Settings.IsAgent), 1))
        myString.Append(Utilities.PadLeadingZeros(De.ComponentID, 13))
        myString.Append(Utilities.FixStringLength(String.Empty, 1))
        myString.Append(Utilities.FixStringLength(ConvertToYN(De.AvailableToSell03), 1))
        myString.Append(Utilities.FixStringLength(ConvertToYN(De.AvailableToSellAvailableTickets), 1))
        myString.Append(Utilities.FixStringLength(_de.StadiumCode, 2))
        myString.Append(Utilities.FixStringLength(_de.SessionId, 36))
        myString.Append(Utilities.FixStringLength(_de.ProductCode, 6))
        myString.Append(Utilities.FixStringLength(sRecordTotal, 3))
        myString.Append(Utilities.FixStringLength(sLastRecord, 3))
        myString.Append(Utilities.FixStringLength(_de.Src, 1))
        myString.Append("   ")

        Return myString.ToString()
    End Function

    Private Function AccessDatabaseWS117R() As ErrorObj

        Dim err As New ErrorObj
        ResultDataSet = New DataSet
        Dim sNextStnd As String = String.Empty
        Dim sNextArea As String = String.Empty
        Dim bMoreRecords As Boolean = True
        Dim dRow As DataRow = Nothing
        Dim PARAMOUT As String = String.Empty
        Dim PARAMOUT2 As String = String.Empty

        Dim sbWholeWorkString As New StringBuilder

        'Create the Status data table
        Dim DtStatusResults As New DataTable
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
            .Add("StadiumCode", GetType(String))
            .Add("DefaultPriceBand", GetType(String))
        End With

        'Create the stand details data table
        Dim DtPriceDetailsResults As New DataTable
        ResultDataSet.Tables.Add(DtPriceDetailsResults)
        With DtPriceDetailsResults.Columns
            .Add("StandCode", GetType(String))
            .Add("StandDescription", GetType(String))
            .Add("AreaCode", GetType(String))
            .Add("AreaDescription", GetType(String))
            .Add("PriceBand", GetType(String))
            .Add("Price", GetType(Decimal))
            .Add("AreaCategory", GetType(String))
        End With

        Dim DtPriceBands As New DataTable
        ResultDataSet.Tables.Add(DtPriceBands)
        With DtPriceBands.Columns
            .Add("PriceBand", GetType(String))
        End With
        Try

            'Loop until no more products available
            Do While bMoreRecords = True

                'Call WS017R
                CallWS117R(sNextStnd, sNextArea, PARAMOUT, PARAMOUT2)

                'Set the response data if call in error or last call
                If (sNextStnd.Trim = "" And sNextArea.Trim = "") Or (PARAMOUT.Substring(3071, 1) = "E" Or PARAMOUT.Substring(3069, 2).Trim <> "") Then
                    dRow = Nothing
                    dRow = DtStatusResults.NewRow
                    If PARAMOUT.Substring(3071, 1) = "E" Or PARAMOUT.Substring(3069, 2).Trim <> "" Then
                        dRow("ErrorOccurred") = "E"
                        dRow("ReturnCode") = PARAMOUT.Substring(3069, 2)
                        dRow("StadiumCode") = ""
                        dRow("DefaultPriceBand") = ""
                        bMoreRecords = False
                    Else
                        dRow("ErrorOccurred") = ""
                        dRow("ReturnCode") = ""
                        dRow("StadiumCode") = PARAMOUT.Substring(3053, 2)
                        dRow("DefaultPriceBand") = PARAMOUT.Substring(3052, 1)
                    End If
                    DtStatusResults.Rows.Add(dRow)
                End If

                'No errors 
                If PARAMOUT.Substring(3071, 1) <> "E" And PARAMOUT.Substring(3069, 2).Trim = "" Then

                    sbWholeWorkString.Append(PARAMOUT.Substring(0, 3000).Trim)

                    'Extract the footer information
                    sNextStnd = PARAMOUT.Substring(3061, 3)
                    sNextArea = PARAMOUT.Substring(3064, 4)
                    If sNextStnd.Trim = "" And sNextArea.Trim = "" Then
                        bMoreRecords = False
                    End If

                Else
                    bMoreRecords = False
                End If
            Loop

            'now split string on stand 
            Dim standSeparators() As String = {";stand"}
            Dim standArray() As String = (";" & sbWholeWorkString.ToString()).Split(standSeparators, StringSplitOptions.RemoveEmptyEntries)
            Dim tempStandCode As String = String.Empty
            Dim tempAreaAndPrice As String = String.Empty
            Dim tempAreaCode As String = String.Empty
            Dim tempSeparator() As String = {"area"}
            Dim tempCommaSeparator() As String = {";"}
            For standArrayIndex As Integer = 0 To standArray.Length - 1
                tempStandCode = standArray(standArrayIndex).Substring(0, standArray(standArrayIndex).IndexOf(";")).Trim
                tempAreaAndPrice = standArray(standArrayIndex).Substring(standArray(standArrayIndex).IndexOf(";") + 1)
                Dim areaPriceWholeStringArray() As String = tempAreaAndPrice.ToString().Split(tempSeparator, StringSplitOptions.RemoveEmptyEntries)
                For areaPriceIndex As Integer = 0 To areaPriceWholeStringArray.Length - 1
                    Dim areaPriceArray() As String = areaPriceWholeStringArray(areaPriceIndex).Split(tempCommaSeparator, StringSplitOptions.RemoveEmptyEntries)
                    If areaPriceArray.Length > 2 Then
                        tempAreaCode = areaPriceArray(0).Trim
                        Dim tempPriceBand As String = ""
                        Dim tempPrice As String = ""
                        Dim tempPriceCategory As String = ""
                        If areaPriceArray(1).Trim.IndexOf("Price Category") > -1 Then
                            tempPriceBand = ""
                            tempPrice = ""
                            tempPriceCategory = ""
                            tempPriceCategory = areaPriceArray(1).Replace("Price Category", "").Trim
                            For priceIndex As Integer = 2 To areaPriceArray.Length - 1
                                If areaPriceArray(priceIndex).IndexOf("=") > 0 Then
                                    tempPriceBand = areaPriceArray(priceIndex).Substring(0, 1)
                                    tempPrice = areaPriceArray(priceIndex).Substring(2).Trim
                                    dRow = Nothing
                                    dRow = DtPriceDetailsResults.NewRow
                                    dRow("StandCode") = tempStandCode
                                    dRow("StandDescription") = ""
                                    dRow("AreaCode") = tempAreaCode
                                    dRow("AreaDescription") = ""
                                    dRow("PriceBand") = tempPriceBand
                                    dRow("Price") = tempPrice
                                    dRow("AreaCategory") = tempPriceCategory
                                    DtPriceDetailsResults.Rows.Add(dRow)
                                End If
                            Next
                        End If
                    End If
                Next
            Next

            If PARAMOUT2.Substring(0, 1) <> String.Empty Then
                Dim iPos As Integer = 0
                While Not String.IsNullOrWhiteSpace(PARAMOUT2.Substring(iPos, 1))
                    dRow = Nothing
                    dRow = DtPriceBands.NewRow
                    'Dim strrr As String = PARAMOUT2.Substring(iPos, 1)
                    dRow("PriceBand") = PARAMOUT2.Substring(iPos, 1)
                    DtPriceBands.Rows.Add(dRow)
                    iPos += 1
                End While
            End If
        Catch ex As Exception
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBPD-WS117R"
                .HasError = True
            End With
        End Try

        Return err

    End Function

    Private Sub CallWS117R(ByVal sNextStnd As String, ByVal sNextArea As String, ByRef PARAMOUT As String, ByRef PARAMOUT2 As String)

        'Create command object
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strProgram As String = "WS117R"
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & "/" & strProgram & "(@PARAM1, @PARAM2)"
        Dim parmIO, parmIO2 As iDB2Parameter

        cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)

        'Populate the parameter
        parmIO = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 3072)
        parmIO.Value = WS117Parm(sNextStnd, sNextArea)
        parmIO.Direction = ParameterDirection.InputOutput

        parmIO2 = cmdSELECT.Parameters.Add(Param2, iDB2DbType.iDB2Char, 5120)
        parmIO2.Value = Utilities.FixStringLength(String.Empty, 25) & ConvertToYN(Settings.IsAgent) & Utilities.FixStringLength(String.Empty, 5094)
        parmIO2.Direction = ParameterDirection.InputOutput

        'Execute
        TalentCommonLog("CallWS117R", "", "Backend Request: strHEADER=" & strHEADER & ", parmIO.Value=" & parmIO.Value)

        cmdSELECT.ExecuteNonQuery()
        PARAMOUT = cmdSELECT.Parameters(Param1).Value.ToString()
        PARAMOUT2 = cmdSELECT.Parameters(Param2).Value.ToString()

        TalentCommonLog("CallWS117R", "", "Backend Response: PARAMOUT=" & PARAMOUT)
    End Sub

    Private Function WS117Parm(ByVal sNextStnd As String, ByVal sNextArea As String) As String
        Dim myString As New StringBuilder

        'Construct the parameter
        myString.Append(Utilities.FixStringLength("", 3055))
        myString.Append(Utilities.FixStringLength(_de.ProductCode, 6))
        myString.Append(Utilities.FixStringLength(sNextStnd, 3))
        myString.Append(Utilities.FixStringLength(sNextArea, 4))
        myString.Append(Utilities.FixStringLength(_de.Src, 1))
        myString.Append("   ")

        Return myString.ToString()
    End Function

    Private Function AccessDatabaseWS118R() As ErrorObj

        Dim err As New ErrorObj
        ResultDataSet = New DataSet
        Dim sNextStnd As String = String.Empty
        Dim sNextArea As String = String.Empty
        Dim bMoreRecords As Boolean = True
        Dim dRow As DataRow = Nothing
        Dim PARAMOUT As String = String.Empty

        Dim sWorkString As String = String.Empty
        Dim sSubstring As String = String.Empty
        Dim sStandCode As String = String.Empty
        Dim sStandDescription As String = String.Empty
        Dim sAreaCode As String = String.Empty
        Dim sAreaDescription As String = String.Empty
        Dim intStartPos As Integer = 0
        Dim intEndPos As Integer = 0

        'Create the Status data table
        Dim DtStatusResults As New DataTable("StatusResults")
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
            .Add("StadiumCode", GetType(String))
        End With

        'Create the stand details data table
        Dim DtStadiumDetailsResults As New DataTable("StandAreas")
        ResultDataSet.Tables.Add(DtStadiumDetailsResults)
        With DtStadiumDetailsResults.Columns
            .Add("StandCode", GetType(String))
            .Add("StandDescription", GetType(String))
            .Add("AreaCode", GetType(String))
            .Add("AreaDescription", GetType(String))
        End With

        'Create the Season Ticket data table
        Dim DtSeasonTicketResults As New DataTable("DtSeasonTicketResults")
        ResultDataSet.Tables.Add(DtSeasonTicketResults)
        With DtSeasonTicketResults.Columns
            .Add("CurrentSeasonTicket", GetType(String))
            .Add("NextSeasonTicket", GetType(String))
        End With

        Try

            'Loop until no more products available
            Do While bMoreRecords = True

                'Call WS118R
                PARAMOUT = CallWS118R(sNextStnd, sNextArea)

                'Set the response data if call in error or last call
                If (sNextStnd.Trim = "" And sNextArea.Trim = "") Or (PARAMOUT.Substring(3071, 1) = "E" Or PARAMOUT.Substring(3069, 2).Trim <> "") Then
                    dRow = Nothing
                    dRow = DtStatusResults.NewRow
                    If PARAMOUT.Substring(3071, 1) = "E" Or PARAMOUT.Substring(3069, 2).Trim <> "" Then
                        dRow("ErrorOccurred") = "E"
                        dRow("ReturnCode") = PARAMOUT.Substring(3069, 2)
                        dRow("StadiumCode") = ""
                        bMoreRecords = False
                    Else
                        dRow("ErrorOccurred") = ""
                        dRow("ReturnCode") = ""
                        dRow("StadiumCode") = PARAMOUT.Substring(3059, 2)
                    End If
                    DtStatusResults.Rows.Add(dRow)
                End If

                'No errors 
                If PARAMOUT.Substring(3071, 1) <> "E" And PARAMOUT.Substring(3069, 2).Trim = "" Then

                    ' Extract each 'stand' string and store in array 
                    sWorkString = PARAMOUT.Substring(0, 3000)
                    Do While sWorkString.Trim <> ""
                        intEndPos = sWorkString.IndexOf("Stand=", 1)
                        If intEndPos < 0 Then
                            sSubstring = sWorkString.Substring(6)
                            sWorkString = ""
                        Else
                            sSubstring = sWorkString.Substring(6, intEndPos - 6)
                            sWorkString = sWorkString.Substring(intEndPos)
                        End If

                        ' Extract the stand code
                        intStartPos = sSubstring.IndexOf(";")
                        sStandCode = sSubstring.Substring(0, intStartPos)
                        sSubstring = sSubstring.Substring(intStartPos + 1)

                        ' Extract the stand description
                        intStartPos = sSubstring.IndexOf(";@")
                        sStandDescription = sSubstring.Substring(0, intStartPos)
                        sSubstring = sSubstring.Substring(intStartPos + 2)

                        ' Loop through the areas
                        Do While sSubstring.Trim <> ""

                            ' Extract the area code
                            intStartPos = sSubstring.IndexOf(";")
                            sAreaCode = sSubstring.Substring(0, intStartPos)
                            sSubstring = sSubstring.Substring(intStartPos + 1)

                            ' Extract the area description
                            intStartPos = sSubstring.IndexOf(";@")
                            sAreaDescription = sSubstring.Substring(0, intStartPos)
                            sSubstring = sSubstring.Substring(intStartPos + 2)

                            ' Add the row
                            dRow = Nothing
                            dRow = DtStadiumDetailsResults.NewRow
                            dRow("StandCode") = sStandCode.Trim()
                            dRow("StandDescription") = sStandDescription.Trim()
                            dRow("AreaCode") = sAreaCode.Trim()
                            dRow("AreaDescription") = sAreaDescription.Trim()
                            DtStadiumDetailsResults.Rows.Add(dRow)

                        Loop

                        ' next...
                    Loop

                    'Extrace the Season Ticket Info
                    Try
                        If Not String.IsNullOrEmpty(PARAMOUT.Substring(3047, 6).Trim) Then
                            Dim dr As DataRow = DtSeasonTicketResults.NewRow
                            dr("CurrentSeasonTicket") = PARAMOUT.Substring(3047, 6)
                            dr("NextSeasonTicket") = PARAMOUT.Substring(3053, 6)
                            DtSeasonTicketResults.Rows.Add(dr)
                        End If
                    Catch ex As Exception
                    End Try

                    'Extract the footer information
                    sNextStnd = PARAMOUT.Substring(3061, 3)
                    sNextArea = PARAMOUT.Substring(3064, 4)
                    If sNextStnd.Trim = "" And sNextArea.Trim = "" Then
                        bMoreRecords = False
                    End If

                Else
                    bMoreRecords = False
                End If
            Loop

        Catch ex As Exception
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBPD-WS118R"
                .HasError = True
            End With
        End Try

        Return err

    End Function

    Private Function CallWS118R(ByVal sNextStnd As String, ByVal sNextArea As String) As String

        'Create command object
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strProgram As String = "WS118R"
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim &
                                    "/" & strProgram & "(@PARAM1)"
        Dim parmIO As iDB2Parameter
        Dim PARAMOUT As String = String.Empty

        'Set the connection string
        cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)

        'Populate the parameter
        parmIO = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 3072)
        parmIO.Value = WS118Parm(sNextStnd, sNextArea)
        parmIO.Direction = ParameterDirection.InputOutput

        'Execute
        TalentCommonLog("CallWS118R", "", "Backend Request: strHEADER=" & strHEADER & ", parmIO.Value=" & parmIO.Value)

        cmdSELECT.ExecuteNonQuery()
        PARAMOUT = cmdSELECT.Parameters(Param1).Value.ToString

        TalentCommonLog("CallWS118R", "", "Backend Response: PARAMOUT=" & PARAMOUT)

        Return PARAMOUT

    End Function

    Private Function WS118Parm(ByVal sNextStnd As String, ByVal sNextArea As String) As String

        Dim myString As String

        Dim waitList As String = "N"
        If _de.WaitList Then
            waitList = "Y"
        End If

        'Construct the parameter
        myString = Utilities.FixStringLength("", 3040) &
            Utilities.FixStringLength(_de.ProductCode, 6) &
                 waitList & Utilities.FixStringLength("", 12) &
                 Utilities.FixStringLength(_de.StadiumCode, 2) &
                 Utilities.FixStringLength(sNextStnd, 3) &
                 Utilities.FixStringLength(sNextArea, 4) & "W   "

        Return myString

    End Function

    Private Function AccessDatabaseWS006R() As ErrorObj

        Dim err As New ErrorObj
        ResultDataSet = New DataSet
        Dim sLastRecord As String = "000"
        Dim sRecordTotal As String = "000"
        Dim bMoreRecords As Boolean = True
        Dim dRow As DataRow = Nothing
        Dim PARAMOUT As String = String.Empty

        'Create the Status data table
        Dim DtStatusResults As New DataTable("StatusResults")
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
            .Add("PPSFreeAreas", GetType(String))
        End With

        'Create the product list data table
        Dim DtProductListResults As New DataTable("ProductListResults")
        ResultDataSet.Tables.Add(DtProductListResults)
        ProductListDataTable(DtProductListResults)

        Try

            'Loop until no more products available
            Do While bMoreRecords = True

                'Call WS016R
                PARAMOUT = CallWS006R(sRecordTotal, sLastRecord)

                'Set the response data on the first call to WS016R
                If sLastRecord = "000" Then
                    dRow = Nothing
                    dRow = DtStatusResults.NewRow
                    If PARAMOUT.Substring(3071, 1) = "E" Or PARAMOUT.Substring(3069, 2).Trim <> "" Then
                        dRow("ErrorOccurred") = "E"
                        dRow("ReturnCode") = PARAMOUT.Substring(3069, 2)
                        dRow("PPSFreeAreas") = ""
                        bMoreRecords = False
                    Else
                        dRow("ErrorOccurred") = ""
                        dRow("ReturnCode") = ""
                        dRow("PPSFreeAreas") = PARAMOUT.Substring(2965, 35)
                    End If
                    DtStatusResults.Rows.Add(dRow)
                End If

                'No errors 
                If PARAMOUT.Substring(3071, 1) <> "E" And PARAMOUT.Substring(3069, 2).Trim = "" Then

                    'Extract the data from the parameter
                    Dim iPosition As Integer = 0
                    Dim iCounter As Integer = 1
                    Do While iCounter <= 12

                        ' Has a product been returned
                        If PARAMOUT.Substring(iPosition, 6).Trim = "" Then
                            Exit Do
                        Else

                            'Create a new row
                            dRow = Nothing
                            dRow = DtProductListResults.NewRow
                            dRow("ProductCode") = PARAMOUT.Substring(iPosition, 6)
                            dRow("ProductDescription") = PARAMOUT.Substring(iPosition + 6, 40).Trim()
                            dRow("ProductYear") = String.Empty
                            dRow("ProductDate") = String.Empty
                            dRow("ProductMDTE08") = String.Empty
                            ' dRow("ProductYear") = "0000"

                            If PARAMOUT.Substring(iPosition + 169, 7).Trim() <> "0000000" Then
                                dRow("ProductYear") = "20" + PARAMOUT.Substring(iPosition + 170, 2).Trim()
                                dRow("ProductDate") = ISeriesDate(PARAMOUT.Substring(iPosition + 170, 6).Trim())
                                dRow("ProductMDTE08") = PARAMOUT.Substring(iPosition + 169, 7).Trim()
                            End If

                            dRow("ProductTime") = ""
                            dRow("ProductOnSale") = ""
                            dRow("ProductAvailForSale") = PARAMOUT.Substring(iPosition + 155, 1)
                            dRow("ProductReqdMem") = PARAMOUT.Substring(iPosition + 109, 6).Trim()
                            dRow("ProductReqdMemDesc") = PARAMOUT.Substring(iPosition + 115, 40).Trim()
                            dRow("ProductReqdMemOK") = PARAMOUT.Substring(iPosition + 108, 1)
                            dRow("ProductType") = De.ProductType
                            dRow("ProductReqdLoyalityPoints") = ""

                            dRow("ProductPriceBand") = ""
                            dRow("ProductTicketLimit") = ""
                            dRow("ProductEntryTime") = ""
                            dRow("ProductAssocTravelProd") = ""
                            dRow("HasAssociatedTravelProduct") = False
                            dRow("ProductOppositionCode") = PARAMOUT.Substring(iPosition + 161, 4).Trim()
                            dRow("ProductCompetitionCode") = PARAMOUT.Substring(iPosition + 165, 4).Trim()
                            dRow("ProductDescription2") = PARAMOUT.Substring(iPosition + 57, 40).Trim()
                            dRow("ProductPrice") = Utilities.FormatPrice(PARAMOUT.Substring(iPosition + 46, 9))
                            dRow("PriceCode") = PARAMOUT.Substring(iPosition + 55, 2)
                            dRow("RelatingSeasonTicket") = PARAMOUT.Substring(iPosition + 97, 6).Trim()
                            dRow("ProductSponsorCode") = PARAMOUT.Substring(iPosition + 103, 4).Trim()
                            dRow("UseVisualSeatLevelSelection") = False
                            dRow("PPSSchemeType") = PARAMOUT.Substring(iPosition + 107, 1)
                            dRow("ProductHomeAsAway") = ""
                            dRow("IsSoldOut") = convertToBool(PARAMOUT.Substring(iPosition + 156, 1).Trim)
                            dRow("ProductSubType") = PARAMOUT.Substring(iPosition + 157, 4).Trim()
                            dRow("IsProductBundle") = False
                            dRow("RestrictGraphical") = False
                            dRow("location") = PARAMOUT.Substring(iPosition + 176, 40).Trim()
                            dRow("ExcludeProductFromWebSales") = convertToBool(PARAMOUT.Substring(iPosition + 216, 1))
                            dRow("HideDate") = convertToBool(PARAMOUT.Substring(iPosition + 217, 1))
                            dRow("HideTime") = convertToBool(PARAMOUT.Substring(iPosition + 218, 1))
                            dRow("CorporateHospitalityProductCode") = String.Empty
                            DtProductListResults.Rows.Add(dRow)

                            'Increment
                            iPosition = iPosition + 240
                            iCounter = iCounter + 1

                        End If
                    Loop

                    'Extract the footer information
                    sLastRecord = PARAMOUT.Substring(3065, 3)
                    sRecordTotal = PARAMOUT.Substring(3062, 3)
                    If PARAMOUT.Substring(3037, 1) <> "Y" Then
                        bMoreRecords = False
                    End If
                Else
                    bMoreRecords = False
                End If

            Loop

        Catch ex As Exception
            ResultDataSet = Nothing
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBPD-WS006R"
                .HasError = True
            End With
        End Try

        Return err

    End Function

    Private Function CallWS006R(ByVal sRecordTotal As String, ByVal sLastRecord As String) As String

        'Create command object
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strProgram As String = "WS006R"
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim &
                                    "/" & strProgram & "(@PARAM1)"
        Dim parmIO As iDB2Parameter
        Dim PARAMOUT As String = String.Empty

        'Set the connection string
        cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)

        'Populate the parameter
        parmIO = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 3072)
        parmIO.Value = WS006Parm(sRecordTotal, sLastRecord)
        parmIO.Direction = ParameterDirection.InputOutput

        'Execute
        TalentCommonLog("CallWS006R", "", "Backend Request: strHEADER=" & strHEADER & ", parmIO.Value=" & parmIO.Value)

        cmdSELECT.ExecuteNonQuery()
        PARAMOUT = cmdSELECT.Parameters(Param1).Value.ToString

        TalentCommonLog("CallWS006R", "", "Backend Response: PARAMOUT=" & PARAMOUT)

        Return PARAMOUT

    End Function

    Private Function WS006Parm(ByVal sRecordTotal As String, ByVal sLastRecord As String) As String

        Dim myString As String
        Dim callType As String = De.PPSType

        Dim stadium0 As String = String.Empty
        Dim stadium1 As String = String.Empty
        Dim stadium2 As String = String.Empty
        Dim stadium3 As String = String.Empty
        Dim stadium4 As String = String.Empty
        Dim stadium5 As String = String.Empty
        Dim sStadia() As String = De.StadiumCode.Split(",")
        Dim count As Integer = 0
        Do While count < sStadia.Length
            Select Case count
                Case Is = 0
                    stadium0 = sStadia(0)
                Case Is = 1
                    stadium1 = sStadia(1)
                Case Is = 2
                    stadium2 = sStadia(2)
                Case Is = 3
                    stadium3 = sStadia(3)
                Case Is = 4
                    stadium4 = sStadia(4)
                Case Is = 5
                    stadium5 = sStadia(5)
            End Select
            count = count + 1
        Loop
        'Construct the parameter
        myString = Utilities.FixStringLength("", 2951) &
             Utilities.FixStringLength(De.ProductSubtype, 4) &
                 Utilities.FixStringLength(Settings.IsAgent.ConvertToISeriesYesNo, 1) &
                 Utilities.FixStringLength("", 9) &
                 Utilities.FixStringLength("", 35) &
                 Utilities.FixStringLength(callType, 1) &
                 Utilities.FixStringLength("", 37) &
                 Utilities.FixStringLength(stadium0, 2) &
                 Utilities.FixStringLength(stadium1, 2) &
                 Utilities.FixStringLength(stadium2, 2) &
                 Utilities.FixStringLength(stadium3, 2) &
                 Utilities.FixStringLength(stadium4, 2) &
                 Utilities.FixStringLength(stadium5, 2) &
                 Utilities.FixStringLength("", 12) &
                 Utilities.FixStringLength(sRecordTotal, 3) &
                 Utilities.FixStringLength(sLastRecord, 3) &
                 Utilities.FixStringLength(De.Src, 1) &
                 "   "

        Return myString

    End Function

    Private Sub ProductListDataTable(ByRef dtProductList As DataTable)
        With dtProductList.Columns
            .Add("ProductCode", GetType(String))
            .Add("ProductDescription", GetType(String))
            .Add("ProductYear", GetType(String))
            .Add("ProductDate", GetType(String))
            .Add("ProductTime", GetType(String))
            .Add("ProductOnSale", GetType(String))
            .Add("ProductAvailForSale", GetType(String))
            .Add("ProductReqdMem", GetType(String))
            .Add("ProductReqdMemDesc", GetType(String))
            .Add("ProductReqdMemOK", GetType(String))
            .Add("ProductType", GetType(String))
            .Add("ProductReqdLoyalityPoints", GetType(String))
            .Add("ProductMDTE08", GetType(String))
            .Add("ProductPriceBand", GetType(String))
            .Add("ProductTicketLimit", GetType(String))
            .Add("ProductEntryTime", GetType(String))
            .Add("ProductAssocTravelProd", GetType(String))
            .Add("ProductOppositionCode", GetType(String))
            .Add("ProductOppositionDesc", GetType(String))
            .Add("ProductCompetitionCode", GetType(String))
            .Add("ProductCompetitionDesc", GetType(String))
            .Add("ProductDescription2", GetType(String))
            .Add("ProductPrice", GetType(String))
            .Add("PriceCode", GetType(String))
            .Add("PriceCodeDescription", GetType(String))
            .Add("ProductStadium", GetType(String))
            .Add("ProductStadiumDescription", GetType(String))
            .Add("CampaignCode", GetType(String))
            .Add("CustomerEligibillity", GetType(String))
            .Add("RelatingSeasonTicket", GetType(String))
            .Add("ProductSponsorCode", GetType(String))
            .Add("UseVisualSeatLevelSelection", GetType(Boolean))
            .Add("PPSSchemeType", GetType(String))
            .Add("LoyaltyRequirementMet", GetType(String))
            .Add("LimitRequirementMet", GetType(String))
            .Add("PreReqProductGroup", GetType(String))
            .Add("PreReqDescription", GetType(String))
            .Add("PreReqMultiGroup", GetType(String))
            .Add("PreReqStadium", GetType(String))
            .Add("PreReqValidationRule", GetType(String))
            .Add("PreReqComments", GetType(String))
            .Add("LoyaltyDetailsApplyRestriction", GetType(String))
            .Add("LoyaltyDetailsNoOfPointsAwarded", GetType(String))
            .Add("LoyaltyDetailsUpdatePreviouslyAwardedPoints", GetType(String))
            .Add("LoyaltyDetailsUpdateFromDate", GetType(String))
            .Add("LoyaltyDetailsUpdateToDate", GetType(String))
            .Add("LoyaltyDetailsNoOfPurchasePointsAwarded", GetType(String))
            .Add("LoyaltyDetailsAwardToSeasonTicketHolders", GetType(String))
            .Add("LoyaltyDetailsSeasonTicketID", GetType(String))
            .Add("ProductHomeAsAway", GetType(String))
            .Add("ProductHomeCode", GetType(String))
            .Add("ProductSubType", GetType(String))
            .Add("ProductSubTypeDesc", GetType(String))
            .Add("ProductDetailCode", GetType(String))
            .Add("capacityCnt", GetType(String))
            .Add("unavailableCnt", GetType(String))
            .Add("reservationsCnt", GetType(String))
            .Add("bookingsCnt", GetType(String))
            .Add("salesCnt", GetType(String))
            .Add("returnsCnt", GetType(String))
            .Add("location", GetType(String))
            .Add("ageRangeFrm", GetType(String))
            .Add("ageRangeTo", GetType(String))
            .Add("duration", GetType(String))
            .Add("HasAssociatedTravelProduct", GetType(Boolean))
            .Add("IsSoldOut", GetType(Boolean))
            .Add("AlternativeSeatSelection", GetType(Boolean))
            .Add("AlternativeSeatSelectionAcrossStands", GetType(Boolean))
            .Add("IsProductBundle", GetType(Boolean))
            .Add("RestrictGraphical", GetType(Boolean))
            .Add("ExcludeProductFromWebSales", GetType(Boolean))
            .Add("HideDate", GetType(Boolean))
            .Add("HideTime", GetType(Boolean))
            .Add("BundleStartDate", GetType(String))
            .Add("BundleEndDate", GetType(String))
            .Add("ProductSetAsSoldOut", GetType(Boolean))
            .Add("CorporateHospitalityProductCode", GetType(String))
            .Add("ProductLinkedToHospitality", GetType(String))
        End With
    End Sub

    Private Sub PreReqGroupDataTable(ByRef dtRequiredList As DataTable)

        With dtRequiredList.Columns
            .Add("ProductCode", GetType(String))
            .Add("PreReqProductCode", GetType(String))
            .Add("PreReqProductDescription", GetType(String))
            .Add("PreReqProductType", GetType(String))
            .Add("PreReqProductDate", GetType(String))
        End With

    End Sub

    Private Sub LoyaltyScheduleTable(ByRef dtRequiredList As DataTable)

        With dtRequiredList.Columns
            .Add("ProductCode", GetType(String))
            .Add("From", GetType(String))
            .Add("RequiredPoints", GetType(String))
            .Add("Time", GetType(String))
        End With

    End Sub

    Private Function AccessDatabaseWS011R() As ErrorObj
        Dim err As New ErrorObj
        ResultDataSet = New DataSet
        Dim sLastRecord As String = "000"
        Dim sRecordTotal As String = "000"
        Dim bMoreRecords As Boolean = True
        Dim iPosition As Integer = 0
        Dim iCounter As Integer = 1
        Dim dRow As DataRow = Nothing
        Dim PARAMOUT As String = String.Empty
        Dim PARAMOUT2 As String = String.Empty
        Dim PARAMOUT3 As String = String.Empty
        Dim regEx As New System.Text.RegularExpressions.Regex("[0-9a-zA-Z\#\s\u0370-\u03FF]", Text.RegularExpressions.RegexOptions.IgnoreCase)

        'Create the Status data table
        Dim DtStatusResults As New DataTable(GlobalConstants.STATUS_RESULTS_TABLE_NAME)
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
            .Add("StadiumCode", GetType(String))
        End With

        'Create the product list data table
        Dim DtProductStadiumAvailability As New DataTable("StadiumAvailability")
        ResultDataSet.Tables.Add(DtProductStadiumAvailability)
        With DtProductStadiumAvailability.Columns
            .Add("ProductCode", GetType(String))
            .Add("StandCode", GetType(String))
            .Add("StandDescription", GetType(String))
            .Add("AreaCode", GetType(String))
            .Add("AreaDescription", GetType(String))
            .Add("MinTicketExchangePrice", GetType(String))
            .Add("MaxTicketExchangePrice", GetType(String))
            .Add("Availability", GetType(String))
            .Add("AdditionalText", GetType(String))
            .Add("Capacity", GetType(String))
            .Add("Reserved", GetType(String))
            .Add("SeatSelection", GetType(String))
            .Add("Roving", GetType(String))
            .Add("TicketExchangeAllowPurchase", GetType(Boolean))
        End With

        'Create the product price break list data table
        Dim DtProductPriceBreaks As New DataTable("AvailablePriceBreaks")
        ResultDataSet.Tables.Add(DtProductPriceBreaks)
        With DtProductPriceBreaks.Columns
            .Add("Stand", GetType(String))
            .Add("Area", GetType(String))
            .Add("PriceBreakId", GetType(Long))
        End With

        Try
            Do While bMoreRecords = True
                PARAMOUT = CallWS011R(sLastRecord, PARAMOUT2, PARAMOUT3)
                Dim hasErrors As Boolean = False
                'Set the response data on the first call to WS016R
                If sLastRecord = "000" Then
                    dRow = Nothing
                    dRow = DtStatusResults.NewRow
                    If PARAMOUT.Substring(3071, 1) = GlobalConstants.ERRORFLAG Or PARAMOUT.Substring(3069, 2).Trim <> String.Empty Then
                        dRow("ErrorOccurred") = GlobalConstants.ERRORFLAG
                        dRow("ReturnCode") = PARAMOUT.Substring(3069, 2)
                        dRow("StadiumCode") = String.Empty
                        bMoreRecords = False
                        hasErrors = True
                    Else
                        dRow("ErrorOccurred") = String.Empty
                        dRow("ReturnCode") = String.Empty
                        dRow("StadiumCode") = PARAMOUT.Substring(2992, 2)
                    End If
                    DtStatusResults.Rows.Add(dRow)
                End If

                If Not hasErrors Then

                    'retrieve stadium availability

                    Dim productCode As String = PARAMOUT.Substring(3056, 6)
                    If Not String.IsNullOrWhiteSpace(PARAMOUT2) Then
                        For value As Integer = 0 To 159
                            Dim stadiumAvailabilityLine As String = PARAMOUT2.Substring(value * 100, 100)
                            If Not String.IsNullOrWhiteSpace(stadiumAvailabilityLine) Then
                                Dim standCode As String = stadiumAvailabilityLine.Substring(0, 3)
                                Dim areaCode As String = stadiumAvailabilityLine.Substring(3, 4)
                                Dim capacity As String = stadiumAvailabilityLine.Substring(7, 7)
                                Dim reserved As String = stadiumAvailabilityLine.Substring(14, 7)
                                Dim seatSelection As String = stadiumAvailabilityLine.Substring(21, 1)
                                Dim roving As String = stadiumAvailabilityLine.Substring(22, 1)
                                Dim ticketExchangeAllowPurchase As Boolean = convertToBool(stadiumAvailabilityLine.Substring(24, 1))
                                Dim minTicketExchangePrice As String = stadiumAvailabilityLine.Substring(25, 9)
                                Dim maxTicketExchangePrice As String = stadiumAvailabilityLine.Substring(34, 9)
                                If regEx.IsMatch(standCode) And regEx.IsMatch(areaCode) Then
                                    dRow = Nothing
                                    dRow = DtProductStadiumAvailability.NewRow
                                    If CInt(capacity) = 0 Or CInt(reserved) = 0 Then
                                        dRow("Availability") = "0"
                                    Else
                                        ' Round to the nearest percentage
                                        Dim percentage As Decimal = (CInt(reserved) / CInt(capacity)) * 100
                                        If percentage > 0.0 And percentage < 1.0 Then
                                            percentage = 1
                                        End If
                                        dRow("Availability") = Math.Round(percentage, 0).ToString
                                    End If
                                    dRow("ProductCode") = productCode.Trim
                                    dRow("StandCode") = standCode.Trim
                                    dRow("StandDescription") = String.Empty
                                    dRow("AreaCode") = areaCode.Trim
                                    dRow("AreaDescription") = String.Empty
                                    dRow("AdditionalText") = String.Empty
                                    dRow("Capacity") = capacity.TrimStart("0")
                                    dRow("Reserved") = reserved.TrimStart("0")
                                    dRow("SeatSelection") = seatSelection
                                    dRow("Roving") = roving.Trim
                                    dRow("TicketExchangeAllowPurchase") = ticketExchangeAllowPurchase
                                    dRow("MinTicketExchangePrice") = Utilities.FormatPrice(Utilities.GetZeroIfEmpty(minTicketExchangePrice))
                                    dRow("MaxTicketExchangePrice") = Utilities.FormatPrice(Utilities.GetZeroIfEmpty(maxTicketExchangePrice))
                                    DtProductStadiumAvailability.Rows.Add(dRow)
                                End If
                            Else
                                Exit For
                            End If
                        Next
                    End If

                    'retrieve price breaks
                    If Not String.IsNullOrWhiteSpace(PARAMOUT3) Then
                        For value As Integer = 0 To 1599
                            Dim priceBreakLine As String = PARAMOUT3.Substring(value * 20, 20)
                            If Not String.IsNullOrWhiteSpace(priceBreakLine) Then
                                dRow = Nothing
                                dRow = DtProductPriceBreaks.NewRow
                                dRow("Stand") = priceBreakLine.Substring(0, 3).Trim
                                dRow("Area") = priceBreakLine.Substring(3, 4).Trim
                                dRow("PriceBreakId") = Utilities.CheckForDBNull_BigInt(priceBreakLine.Substring(7, 13).Trim)
                                DtProductPriceBreaks.Rows.Add(dRow)
                            Else
                                Exit For
                            End If
                        Next
                    End If

                    'Extract the footer information
                    sLastRecord = PARAMOUT.Substring(3063, 5)
                    bMoreRecords = PARAMOUT.Substring(3062, 1)
                End If
            Loop
        Catch ex As Exception
            ResultDataSet = Nothing
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBPD-WS011R"
                .HasError = True
            End With
        End Try

        Return err
    End Function

    Private Function CallWS011R(ByVal sLastRecord As String, ByRef PARAMOUT2 As String, ByRef PARAMOUT3 As String) As String
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & "/WS011R(@PARAM1, @PARAM2, @PARAM3)"
        Dim parmIO1 As iDB2Parameter
        Dim parmIO2 As iDB2Parameter
        Dim parmIO3 As iDB2Parameter
        Dim PARAMOUT As String = String.Empty

        cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)
        parmIO1 = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 3072)
        parmIO1.Value = WS011Parm(sLastRecord)
        parmIO1.Direction = ParameterDirection.InputOutput

        parmIO2 = cmdSELECT.Parameters.Add(Param2, iDB2DbType.iDB2Char, 16000)
        parmIO2.Value = PARAMOUT2
        parmIO2.Direction = ParameterDirection.InputOutput

        parmIO3 = cmdSELECT.Parameters.Add(Param3, iDB2DbType.iDB2Char, 32000)
        parmIO3.Value = PARAMOUT3
        parmIO3.Direction = ParameterDirection.InputOutput
        'Execute
        TalentCommonLog("CallWS011R", "", "Backend Request: strHEADER=" & strHEADER & ", parmIO1.Value=" & parmIO1.Value & ", parmIO2.Value=" & parmIO2.Value)
        cmdSELECT.ExecuteNonQuery()
        PARAMOUT = cmdSELECT.Parameters(Param1).Value.ToString
        PARAMOUT2 = cmdSELECT.Parameters(Param2).Value.ToString
        PARAMOUT3 = cmdSELECT.Parameters(Param3).Value.ToString
        TalentCommonLog("CallWS011R", "", "Backend Response: PARAMOUT=" & PARAMOUT & "PARAMOUT2=" & PARAMOUT2 & "PARAMOUT3=" & PARAMOUT3)

        Return PARAMOUT
    End Function

    Private Function WS011Parm(ByVal sLastRecord As String) As String
        Dim myString As New StringBuilder
        Dim capacityBy As String = String.Empty
        Dim productTypeAway As String = String.Empty
        Dim agent As String = String.Empty

        'How is capacity determined - by stadium, by web sales seats?
        If _de.CapacityByStadium Then
            capacityBy = "S"
        Else
            capacityBy = "W"
        End If
        If (_de.ProductType.Equals(GlobalConstants.AWAYPRODUCTTYPE)) Then
            productTypeAway = "Y"
        Else
            productTypeAway = "N"
        End If
        If AgentLevelCacheForProductStadiumAvailability Then
            agent = Settings.OriginatingSource
        End If

        myString.Append(Utilities.FixStringLength(String.Empty, 2979))
        myString.Append(Utilities.PadLeadingZeros(_de.PackageID, 13))
        myString.Append(Utilities.PadLeadingZeros(String.Empty, 2))
        myString.Append(Utilities.PadLeadingZerosDec(_de.SelectedMinimumPrice, 9))
        myString.Append(Utilities.PadLeadingZerosDec(_de.SelectedMaximumPrice, 9))
        myString.Append(Utilities.FixStringLength(ConvertToYN(_de.IncludeTicketExchangeSeats), 1))
        myString.Append(Utilities.PadLeadingZeros(_de.PriceBreakId, 13))
        myString.Append(Utilities.FixStringLength(ConvertToYN(Settings.IsAgent), 1))
        myString.Append(Utilities.PadLeadingZeros(De.ComponentID, 13))
        myString.Append(Utilities.FixStringLength("N", 1))
        myString.Append(Utilities.FixStringLength(De.CATMode, 1))
        myString.Append(Utilities.FixStringLength(agent, 10))
        myString.Append(Utilities.FixStringLength(productTypeAway, 1))
        myString.Append(Utilities.FixStringLength(De.CampaignCode, 2))
        myString.Append(Utilities.FixStringLength(capacityBy, 1))
        myString.Append(Utilities.FixStringLength(_de.ProductCode, 6))
        myString.Append(Utilities.PadLeadingZeros(String.Empty, 1))
        myString.Append(Utilities.PadLeadingZeros(sLastRecord, 5))
        myString.Append(Utilities.FixStringLength(_de.Src, 1))
        myString.Append("   ")

        Return myString.ToString()
    End Function

    Private Function AccessDatabaseWS151R() As ErrorObj
        Dim err As New ErrorObj
        ResultDataSet = New DataSet
        Dim sLastRecord As String = "000"
        Dim sRecordTotal As String = "000"
        Dim bMoreRecords As Boolean = True
        Dim dRow As DataRow = Nothing
        Dim PARAMOUT As String = String.Empty
        Dim strRow As String = String.Empty
        Dim charHash As String = "#"
        Dim strOut As String = String.Empty
        Dim intCount As Integer = 0
        Dim sRowName As String = String.Empty
        Dim sRowCapacity As String = String.Empty
        Dim sRowSequence As String = String.Empty
        Dim sRowDetail As String = String.Empty
        Dim regEx As New System.Text.RegularExpressions.Regex("[0-9a-zA-Z\#\s\u0370-\u03FF]", Text.RegularExpressions.RegexOptions.IgnoreCase)
        Dim graphicalString As String = String.Empty

        'Create the Status data table
        Dim DtStatusResults As New DataTable("RowAvailabilityStatus")
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
            .Add("RowTotal", GetType(String))
            .Add("ColTotal", GetType(String))
        End With

        'Create the product row data table
        Dim DtProductRowAvailability As New DataTable("RowAvailability")
        ResultDataSet.Tables.Add(DtProductRowAvailability)
        With DtProductRowAvailability.Columns
            .Add("RowName", GetType(String))
            .Add("RowCapacity", GetType(String))
            .Add("RowSequence", GetType(String))
            .Add("RowDetail", GetType(String))
        End With

        Try
            'Call WS151R for Seat Availability
            PARAMOUT = CallWS151R(sRecordTotal, sLastRecord)

            'Set the status data table
            dRow = Nothing
            dRow = DtStatusResults.NewRow
            If PARAMOUT.Substring(9998, 1) = "E" Or PARAMOUT.Substring(9996, 2).Trim <> "" Then
                dRow("ErrorOccurred") = "E"
                dRow("ReturnCode") = PARAMOUT.Substring(9996, 2)
                dRow("RowTotal") = ""
                dRow("ColTotal") = ""
            Else
                dRow("ErrorOccurred") = ""
                dRow("ReturnCode") = ""
                dRow("RowTotal") = PARAMOUT.Substring(0, 3)
                dRow("ColTotal") = PARAMOUT.Substring(3, 4)
            End If
            DtStatusResults.Rows.Add(dRow)

            If Not (PARAMOUT.Substring(9998, 1) = "E" Or PARAMOUT.Substring(9996, 2).Trim <> "") Then
                With DtProductRowAvailability.Rows
                    strOut = PARAMOUT.Substring(7)
                    Dim rowArray() As String = Split(strOut, charHash)
                    Do While intCount < rowArray.Length

                        ' Determine values
                        If rowArray(intCount).ToString.Trim.Length < 4 Then
                            sRowName = rowArray(intCount).ToString.Substring("0")
                            sRowCapacity = ""
                            sRowSequence = ""
                            sRowDetail = ""
                        Else
                            sRowName = rowArray(intCount).ToString.Substring("0", "4")
                            sRowCapacity = rowArray(intCount).ToString.Substring("4", "7")
                            sRowSequence = rowArray(intCount).ToString.Substring("10", "3")
                            sRowDetail = rowArray(intCount).ToString.Substring("14")
                        End If

                        ' Write new data table row
                        If sRowName.Trim <> "" And regEx.IsMatch(sRowName) Then
                            dRow = Nothing
                            dRow = DtProductRowAvailability.NewRow
                            dRow("RowName") = sRowName
                            dRow("RowCapacity") = sRowCapacity.TrimStart("0")
                            dRow("RowSequence") = sRowSequence
                            dRow("RowDetail") = sRowDetail
                            DtProductRowAvailability.Rows.Add(dRow)
                        Else
                            dRow = Nothing
                            dRow = DtProductRowAvailability.NewRow
                            dRow("RowName") = "Blank "
                            dRow("RowCapacity") = ""
                            dRow("RowSequence") = ""
                            dRow("RowDetail") = ""
                            DtProductRowAvailability.Rows.Add(dRow)
                        End If

                        graphicalString = graphicalString & dRow("RowName") & " - " & sRowDetail & vbCrLf
                        intCount = intCount + 1
                    Loop
                End With
            End If

        Catch ex As Exception
            Const strError8 As String = "Error during database Access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError8
                .ErrorNumber = "TACDABAS-WS151R"
                .HasError = True
            End With
        End Try

        Return err
    End Function

    Private Function CallWS151R(ByVal sRecordTotal As String, ByVal sLastRecord As String) As String
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & "/WS151R(@PARAM1)"
        Dim parmIO As iDB2Parameter
        Dim PARAMOUT As String = String.Empty

        cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)
        parmIO = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 9999)
        parmIO.Value = WS151Parm(sRecordTotal, sLastRecord)
        parmIO.Direction = ParameterDirection.InputOutput

        'Execute
        TalentCommonLog("CallWS151R", "", "Backend Request: strHEADER=" & strHEADER & ", parmIO.Value=" & parmIO.Value)
        cmdSELECT.ExecuteNonQuery()
        PARAMOUT = cmdSELECT.Parameters(Param1).Value.ToString
        TalentCommonLog("CallWS151R", "", "Backend Response: PARAMOUT=" & PARAMOUT)

        Return PARAMOUT
    End Function

    Private Function WS151Parm(ByVal sRecordTotal As String, ByVal sLastRecord As String) As String
        Dim myString As New StringBuilder
        Dim productTypeAway As String = String.Empty
        Dim selectedMinimumPrice As String = _de.SelectedMinimumPrice.ToString()
        Dim selectedMaximumPrice As String = _de.SelectedMaximumPrice.ToString()
        If (_de.ProductType.Equals(GlobalConstants.AWAYPRODUCTTYPE)) Then
            productTypeAway = "Y"
        Else
            productTypeAway = "N"
        End If
        selectedMinimumPrice = selectedMinimumPrice.Replace(".", String.Empty)
        selectedMaximumPrice = selectedMaximumPrice.Replace(".", String.Empty)

        'Construct the parameter
        myString.Append(Utilities.FixStringLength(String.Empty, 9934))
        myString.Append(Utilities.PadLeadingZeros(selectedMinimumPrice, 9))
        myString.Append(Utilities.PadLeadingZeros(selectedMaximumPrice, 9))
        myString.Append(Utilities.FixStringLength(ConvertToYN(Settings.IsAgent), 1))
        myString.Append(Utilities.PadLeadingZeros(_de.PriceBreakId, 13))
        myString.Append(Utilities.PadLeadingZeros(_de.ComponentID, 13))
        myString.Append(Utilities.FixStringLength(productTypeAway, 1))
        myString.Append(Utilities.FixStringLength(_de.CampaignCode, 2))
        myString.Append(Utilities.FixStringLength(_de.ProductCode, 6))
        myString.Append(Utilities.FixStringLength(_de.StandCode, 3))
        myString.Append(Utilities.FixStringLength(_de.AreaCode, 4))
        myString.Append(Utilities.FixStringLength(_de.Src, 1))
        myString.Append("   ")

        Return myString.ToString()
    End Function

    Private Function AccessDatabaseWS152R() As ErrorObj

        Dim err As New ErrorObj
        ResultDataSet = New DataSet
        Dim sLastRecord As String = "000"
        Dim sRecordTotal As String = "000"
        Dim bMoreRecords As Boolean = True
        Dim dRow As DataRow = Nothing
        Dim PARAMOUT As String = String.Empty

        Dim strRow As String = String.Empty
        Dim charHash As String = "#"
        Dim strOut As String = String.Empty
        Dim intCount As Integer = 0
        Dim sRowName As String = String.Empty
        Dim sRowSeatNumbers As String = String.Empty
        Dim regEx As New System.Text.RegularExpressions.Regex("[0-9a-zA-Z\#\s\u0370-\u03FF]", Text.RegularExpressions.RegexOptions.IgnoreCase)
        Dim graphicalString As String = String.Empty

        'Create the Status data table
        Dim DtStatusResults As New DataTable("ProductSeatNumbersStatus")
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
            .Add("RowTotal", GetType(String))
            .Add("ColTotal", GetType(String))
        End With

        'Create the product row data table
        Dim DtProductSeatNumbers As New DataTable("ProductSeatNumbers")
        ResultDataSet.Tables.Add(DtProductSeatNumbers)
        With DtProductSeatNumbers.Columns
            .Add("RowName", GetType(String))
            .Add("RowSeatNumbers", GetType(String))
        End With

        Try

            'Call WS152R for Seat Availability
            PARAMOUT = CallWS152R(sRecordTotal, sLastRecord)

            'Set the status data table
            dRow = Nothing
            dRow = DtStatusResults.NewRow
            If PARAMOUT.Substring(16, 1) = "E" Or PARAMOUT.Substring(14, 2).Trim <> "" Then
                dRow("ErrorOccurred") = "E"
                dRow("ReturnCode") = PARAMOUT.Substring(14, 2)
            Else
                dRow("ErrorOccurred") = ""
                dRow("ReturnCode") = ""
            End If
            DtStatusResults.Rows.Add(dRow)

            If Not (PARAMOUT.Substring(16, 1) = "E" Or PARAMOUT.Substring(14, 2).Trim <> "") Then

                With DtProductSeatNumbers.Rows
                    strOut = PARAMOUT.Substring(17).ToString.Trim
                    Dim rowArray() As String = Split(strOut, charHash)
                    Do While intCount < rowArray.Length

                        ' Determine values
                        If rowArray(intCount).ToString.Trim.Length < 4 Then
                            sRowName = rowArray(intCount).ToString.Substring(0).Trim
                            sRowSeatNumbers = ""
                        Else
                            sRowName = rowArray(intCount).ToString.Substring(0, 4).Trim
                            sRowSeatNumbers = rowArray(intCount).ToString.Substring(4)
                        End If

                        ' Write new data table row
                        If sRowName.Trim <> "" And regEx.IsMatch(sRowName) Then
                            dRow = Nothing
                            dRow = DtProductSeatNumbers.NewRow
                            dRow("RowName") = sRowName
                            dRow("RowSeatNumbers") = sRowSeatNumbers
                            DtProductSeatNumbers.Rows.Add(dRow)
                        Else
                            dRow = Nothing
                            dRow = DtProductSeatNumbers.NewRow
                            dRow("RowName") = "Blank "
                            dRow("RowSeatNumbers") = ""
                            DtProductSeatNumbers.Rows.Add(dRow)
                        End If

                        graphicalString = graphicalString & dRow("RowName") & " - " & dRow("RowSeatNumbers") & vbCrLf
                        intCount = intCount + 1
                    Loop
                End With
            End If

        Catch ex As Exception
            Const strError8 As String = "Error during database Access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError8
                .ErrorNumber = "TACDABAS-WS152R"
                .HasError = True
            End With
        End Try

        Return err


    End Function

    Private Function CallWS152R(ByVal sRecordTotal As String, ByVal sLastRecord As String) As String

        'Create command object
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strProgram As String = "WS152R"
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim &
                                    "/" & strProgram & "(@PARAM1)"
        Dim parmIO As iDB2Parameter
        Dim PARAMOUT As String = String.Empty

        'Set the connection string
        cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)

        'Populate the parameter
        parmIO = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 32765)
        parmIO.Value = WS152Parm(sRecordTotal, sLastRecord)
        parmIO.Direction = ParameterDirection.InputOutput

        'Execute
        TalentCommonLog("CallWS152R", "", "Backend Request: strHEADER=" & strHEADER & ", parmIO.Value=" & parmIO.Value)

        cmdSELECT.ExecuteNonQuery()
        PARAMOUT = cmdSELECT.Parameters(Param1).Value.ToString

        TalentCommonLog("CallWS152R", "", "Backend Response: PARAMOUT=" & PARAMOUT)

        Return PARAMOUT

    End Function

    Private Function WS152Parm(ByVal sRecordTotal As String, ByVal sLastRecord As String) As String

        Dim myString As String

        'Construct the parameter
        myString = Utilities.FixStringLength(_de.ProductCode, 6) &
                 Utilities.FixStringLength(_de.StandCode, 3) &
                 Utilities.FixStringLength(_de.AreaCode, 4) & "W" &
                 Utilities.FixStringLength("", 32751)


        Return myString

    End Function

    Private Function AccessDatabaseWS154R() As ErrorObj
        Dim err As New ErrorObj
        ResultDataSet = New DataSet
        Dim dRow As DataRow = Nothing
        Dim PARAMOUT1 As String = String.Empty
        Dim PARAMOUT2 As String = String.Empty
        Dim charHash As String = "#"
        Dim strOutDesc As String = String.Empty
        Dim intCount As Integer = 0
        Dim regEx As New System.Text.RegularExpressions.Regex("[0-9a-zA-Z\#\s\u0370-\u03FF]", Text.RegularExpressions.RegexOptions.IgnoreCase)

        'Create the Status data table
        Dim DtStatusResults As New DataTable("Status")
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        'Create the restriction descriptions table
        Dim DtProductSeatReservations As New DataTable("SeatReservations")
        ResultDataSet.Tables.Add(DtProductSeatReservations)
        With DtProductSeatReservations.Columns
            .Add("RowCodes", GetType(String))
            .Add("ReservationCodes", GetType(String))
        End With


        'Create the seat restrictions data table
        Dim DtProductReservationDescription As New DataTable("ReservationDesriptions")
        ResultDataSet.Tables.Add(DtProductReservationDescription)
        With DtProductReservationDescription.Columns
            .Add("ReservationCode", GetType(String))
            .Add("Description", GetType(String))
        End With

        Try

            CallWS154R(PARAMOUT1, PARAMOUT2)

            'Set the status data table
            dRow = Nothing
            dRow = DtStatusResults.NewRow
            If PARAMOUT1.Substring(16, 1) = "E" Or PARAMOUT1.Substring(14, 2).Trim <> "" Then
                dRow("ErrorOccurred") = "E"
                dRow("ReturnCode") = PARAMOUT1.Substring(14, 2)
            Else
                dRow("ErrorOccurred") = ""
                dRow("ReturnCode") = ""
            End If
            DtStatusResults.Rows.Add(dRow)

            If Not (PARAMOUT1.Substring(16, 1) = "E" Or PARAMOUT1.Substring(14, 2).Trim <> "") Then
                'Set the description table
                With DtProductSeatReservations
                    strOutDesc = PARAMOUT1.Remove(0, 17).ToString()
                    Dim rowArray() As String = Split(strOutDesc, charHash)
                    intCount = 0
                    Do While intCount < rowArray.Length
                        dRow = Nothing
                        dRow = DtProductSeatReservations.NewRow
                        If Not String.IsNullOrEmpty(rowArray(intCount).ToString.Substring(0, 4).Trim) _
                            AndAlso regEx.IsMatch(String.IsNullOrEmpty(rowArray(intCount).ToString.Substring(0, 4).Trim)) Then
                            dRow("RowCodes") = rowArray(intCount).ToString.Substring(0, 4).Trim()
                            dRow("ReservationCodes") = rowArray(intCount).Substring(4, rowArray(intCount).Length - 4)
                        Else
                            dRow("RowCodes") = "Blank "
                        End If
                        DtProductSeatReservations.Rows.Add(dRow)
                        intCount = intCount + 1
                    Loop
                End With

                'Set the description table
                With DtProductReservationDescription
                    Dim rowArray() As String = Split(PARAMOUT2, charHash)
                    intCount = 0
                    Do While intCount < rowArray.Length
                        dRow = Nothing
                        dRow = DtProductReservationDescription.NewRow
                        dRow("ReservationCode") = rowArray(intCount).ToString.Substring(0, 2)
                        dRow("Description") = rowArray(intCount).ToString.Substring(2, rowArray(intCount).Length - 2)
                        DtProductReservationDescription.Rows.Add(dRow)
                        intCount = intCount + 1
                    Loop
                End With
            End If

        Catch ex As Exception
            Const strError8 As String = "Error during database Access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError8
                .ErrorNumber = "TACDABAS-WS154R"
                .HasError = True
            End With
        End Try

        Return err
    End Function

    Private Function AccessDatabaseWS153R() As ErrorObj

        Dim err As New ErrorObj
        ResultDataSet = New DataSet
        Dim dRow As DataRow = Nothing
        Dim PARAMOUT As String = String.Empty

        Dim strRow As String = String.Empty
        Dim charHash As String = "#"
        Dim strOutDesc As String = String.Empty
        Dim strOut As String = String.Empty
        Dim intCount As Integer = 0
        Dim intStringStart As Integer = 0
        Dim sRowName As String = String.Empty
        Dim sSeatRestrictions As String = String.Empty
        Dim regEx As New System.Text.RegularExpressions.Regex("[0-9a-zA-Z\#\s\u0370-\u03FF]", Text.RegularExpressions.RegexOptions.IgnoreCase)
        Dim graphicalString As String = String.Empty

        'Create the Status data table
        Dim DtStatusResults As New DataTable
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        'Create the restriction descriptions table
        Dim DtProductRestrictionDescription As New DataTable
        ResultDataSet.Tables.Add(DtProductRestrictionDescription)
        With DtProductRestrictionDescription.Columns
            .Add("Restriction", GetType(String))
            .Add("Description", GetType(String))
        End With


        'Create the seat restrictions data table
        Dim DtProductSeatRestrictions As New DataTable
        ResultDataSet.Tables.Add(DtProductSeatRestrictions)
        With DtProductSeatRestrictions.Columns
            .Add("RowName", GetType(String))
            .Add("SeatRestrictions", GetType(String))
        End With

        Try

            'Call WS153R for Seat Restrictions
            PARAMOUT = CallWS153R()

            'Set the status data table
            dRow = Nothing
            dRow = DtStatusResults.NewRow
            If PARAMOUT.Substring(16, 1) = "E" Or PARAMOUT.Substring(14, 2).Trim <> "" Then
                dRow("ErrorOccurred") = "E"
                dRow("ReturnCode") = PARAMOUT.Substring(14, 2)
            Else
                dRow("ErrorOccurred") = ""
                dRow("ReturnCode") = ""
            End If
            DtStatusResults.Rows.Add(dRow)

            If Not (PARAMOUT.Substring(16, 1) = "E" Or PARAMOUT.Substring(14, 2).Trim <> "") Then

                intStringStart = PARAMOUT.IndexOf("DETAIL=") + 7

                'Set the description table
                With DtProductSeatRestrictions
                    strOutDesc = PARAMOUT.Substring(17, intStringStart - 25).ToString.Trim
                    Dim rowArray() As String = Split(strOutDesc, charHash)
                    intCount = 0
                    Do While intCount < rowArray.Length
                        dRow = Nothing
                        dRow = DtProductRestrictionDescription.NewRow
                        dRow("Restriction") = rowArray(intCount).ToString.Substring(0, 2).Trim
                        dRow("Description") = rowArray(intCount).ToString.Substring(2).Trim
                        DtProductRestrictionDescription.Rows.Add(dRow)
                        intCount = intCount + 1
                    Loop
                End With


                'Set the seat restrictions table
                With DtProductSeatRestrictions.Rows
                    strOut = PARAMOUT.Substring(intStringStart).ToString.Trim
                    Dim rowArray() As String = Split(strOut, charHash)
                    intCount = 0
                    Do While intCount < rowArray.Length

                        ' Determine values
                        If rowArray(intCount).ToString.Trim.Length < 4 Then
                            sRowName = rowArray(intCount).ToString.Substring(0).Trim
                            sSeatRestrictions = ""
                        Else
                            sRowName = rowArray(intCount).ToString.Substring(0, 4).Trim
                            sSeatRestrictions = rowArray(intCount).ToString.Substring(4)
                        End If

                        ' Write new data table row
                        If sRowName.Trim <> "" And regEx.IsMatch(sRowName) Then
                            dRow = Nothing
                            dRow = DtProductSeatRestrictions.NewRow
                            dRow("RowName") = sRowName
                            dRow("SeatRestrictions") = sSeatRestrictions
                            DtProductSeatRestrictions.Rows.Add(dRow)
                        Else
                            dRow = Nothing
                            dRow = DtProductSeatRestrictions.NewRow
                            dRow("RowName") = "Blank "
                            dRow("SeatRestrictions") = ""
                            DtProductSeatRestrictions.Rows.Add(dRow)
                        End If

                        graphicalString = graphicalString & dRow("RowName") & " - " & dRow("SeatRestrictions") & vbCrLf
                        intCount = intCount + 1
                    Loop
                End With
            End If

        Catch ex As Exception
            Const strError8 As String = "Error during database Access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError8
                .ErrorNumber = "TACDABAS-WS153R"
                .HasError = True
            End With
        End Try

        Return err


    End Function

    Private Sub CallWS154R(ByRef PARAMOUT1 As String, ByRef PARAMOUT2 As String)

        'Create command object
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strProgram As String = "WS154R"
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim &
                                    "/" & strProgram & "(@PARAM1, @PARAM2)"
        Dim parmIO As iDB2Parameter
        Dim parmIO2 As iDB2Parameter
        Dim PARAMOUT As String = String.Empty

        'Set the connection string
        cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)

        'Populate the parameter
        parmIO = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 32765)
        parmIO.Value = WS154Parm()
        parmIO.Direction = ParameterDirection.InputOutput
        parmIO2 = cmdSELECT.Parameters.Add(Param2, iDB2DbType.iDB2Char, 10000)
        parmIO2.Value = Utilities.FixStringLength(String.Empty, 10000)
        parmIO2.Direction = ParameterDirection.InputOutput

        'Execute
        TalentCommonLog("CallWS154R", "", "Backend Request: strHEADER=" & strHEADER & ", parmIO.Value=" & parmIO.Value)

        cmdSELECT.ExecuteNonQuery()
        PARAMOUT1 = cmdSELECT.Parameters(Param1).Value.ToString()
        PARAMOUT2 = cmdSELECT.Parameters(Param2).Value.ToString()

        TalentCommonLog("CallWS154R", "", "Backend Response: PARAMOUT=" & PARAMOUT)
    End Sub

    Private Function CallWS153R() As String

        'Create command object
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strProgram As String = "WS153R"
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim &
                                    "/" & strProgram & "(@PARAM1)"
        Dim parmIO As iDB2Parameter
        Dim PARAMOUT As String = String.Empty

        'Set the connection string
        cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)

        'Populate the parameter
        parmIO = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 32765)
        parmIO.Value = WS153Parm()
        parmIO.Direction = ParameterDirection.InputOutput

        'Execute
        TalentCommonLog("CallWS153R", "", "Backend Request: strHEADER=" & strHEADER & ", parmIO.Value=" & parmIO.Value)

        cmdSELECT.ExecuteNonQuery()
        PARAMOUT = cmdSELECT.Parameters(Param1).Value.ToString

        TalentCommonLog("CallWS153R", "", "Backend Response: PARAMOUT=" & PARAMOUT)

        Return PARAMOUT

    End Function

    Private Function WS154Parm() As String

        Dim myString As String

        'Construct the parameter
        myString = Utilities.FixStringLength(_de.ProductCode.ToUpper(), 6) &
                 Utilities.FixStringLength(_de.StandCode.ToUpper(), 3) &
                 Utilities.FixStringLength(_de.AreaCode.ToUpper(), 4) & "W" &
                 Utilities.FixStringLength("", 32751)


        Return myString

    End Function

    Private Function WS153Parm() As String

        Dim myString As String

        'Construct the parameter
        myString = Utilities.FixStringLength(_de.ProductCode, 6) &
                 Utilities.FixStringLength(_de.StandCode, 3) &
                 Utilities.FixStringLength(_de.AreaCode, 4) & "W" &
                 Utilities.FixStringLength("", 32751)


        Return myString

    End Function

    Private Function AccessDatabaseWS045R() As ErrorObj

        Dim err As New ErrorObj
        ResultDataSet = New DataSet
        Dim moreRecords As Boolean = True
        Dim dRow As DataRow = Nothing
        Dim PARAMOUT As String = String.Empty


        'Create the Status data table
        Dim DtStatusResults As New DataTable("StatusResults")
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        'Create the product list data table
        Dim DtEligibleCustomers As New DataTable("EligibleCustomers")
        ResultDataSet.Tables.Add(DtEligibleCustomers)
        With DtEligibleCustomers.Columns
            .Add("Customer", GetType(String))
        End With

        '---------------
        ' Test Routine!!
        '---------------
        'dRow = Nothing
        'dRow = DtEligibleCustomers.NewRow
        'dRow("Customer") = "000000000055"

        'DtEligibleCustomers.Rows.Add(dRow)
        'dRow = Nothing
        'dRow = DtEligibleCustomers.NewRow
        'dRow("Customer") = "000000000101"

        'DtEligibleCustomers.Rows.Add(dRow)

        'dRow = Nothing
        'dRow = DtStatusResults.NewRow
        'dRow("ErrorOccurred") = "N"
        'dRow("ReturnCode") = ""

        'DtStatusResults.Rows.Add(dRow)
        '   Return err
        '-------------------
        ' End Test Routine!!
        '-------------------

        Try

            ' Just call once and return up to 500 customers
            'Call WS045R
            PARAMOUT = CallWS045R()
            dRow = Nothing
            dRow = DtStatusResults.NewRow

            'Set the response data on the first call to WS045R

            If PARAMOUT.Substring(6143, 1) = "E" Or PARAMOUT.Substring(6141, 2).Trim <> "" Then
                dRow("ErrorOccurred") = "E"
                dRow("ReturnCode") = PARAMOUT.Substring(6141, 2)
            Else
                dRow("ErrorOccurred") = ""
                dRow("ReturnCode") = ""
            End If
            DtStatusResults.Rows.Add(dRow)

            'No errors 
            If PARAMOUT.Substring(6143, 1) <> "E" And PARAMOUT.Substring(6141, 2).Trim = "" Then

                Dim sCustomerNumber As String = String.Empty
                'Extract the data from the parameter
                Dim iPosition As Integer = 0
                Dim iCounter As Integer = 1
                Do While iCounter <= 500 And moreRecords

                    ' Has a record been returned
                    If PARAMOUT.Substring(iPosition, 12).Trim = "" Then
                        moreRecords = False
                    Else

                        sCustomerNumber = PARAMOUT.Substring(iPosition, 12)

                        'Create a new row
                        dRow = Nothing
                        dRow = DtEligibleCustomers.NewRow
                        dRow("Customer") = sCustomerNumber

                        DtEligibleCustomers.Rows.Add(dRow)

                        ' Next record
                        iPosition = iPosition + 12
                        iCounter = iCounter + 1

                    End If
                Loop

            End If

        Catch ex As Exception
            ResultDataSet = Nothing
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBPD-WS045R"
                .HasError = True
            End With
        End Try

        Return err

    End Function

    Private Function CallWS045R() As String

        'Create command object
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strProgram As String = "WS045R"
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim &
                                    "/" & strProgram & "(@PARAM1)"
        Dim parmIO As iDB2Parameter
        Dim PARAMOUT As String = String.Empty

        'Set the connection string
        cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)

        'Populate the parameter
        parmIO = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 6144)
        parmIO.Value = WS045RParm()
        parmIO.Direction = ParameterDirection.InputOutput

        'Execute
        TalentCommonLog("CallWS045R", "", "Backend Request: strHEADER=" & strHEADER & ", parmIO.Value=" & parmIO.Value)

        cmdSELECT.ExecuteNonQuery()
        PARAMOUT = cmdSELECT.Parameters(Param1).Value.ToString

        TalentCommonLog("CallWS045R", "", "Backend Response: PARAMOUT=" & PARAMOUT)

        Return PARAMOUT

    End Function

    Private Function WS045RParm() As String

        Dim myString As String
        Dim includeProductPurchasers As String = ""
        Dim productSettings As DEProductSettings = CType(Settings, DEProductSettings)

        includeProductPurchasers = productSettings.IncludeProductPurchasers

        'Construct the parameter
        myString = Utilities.FixStringLength(" ", 6121) &
                    Utilities.FixStringLength(includeProductPurchasers, 1) &
                Utilities.FixStringLength(De.ProductCode, 6) &
                    Utilities.FixStringLength(De.CustomerNumber, 12) &
                 Utilities.FixStringLength(_de.Src, 1) &
                 "   "

        Return myString

    End Function

    Private Function AccessDatabaseWS024R() As ErrorObj
        Dim err As New ErrorObj
        ResultDataSet = New DataSet
        Dim dRow As DataRow = Nothing
        Dim PARAMOUT1 As String = String.Empty
        Dim PARAMOUT2 As String = String.Empty

        'Create the Status data table
        Dim DtStatusResults As New DataTable("Status")
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        'Create the product Price Band Details data table
        Dim dtSeatDetails As New DataTable("SeatDetails")
        ResultDataSet.Tables.Add(dtSeatDetails)
        With dtSeatDetails.Columns
            .Add("Available", GetType(Boolean))
            .Add("Category", GetType(String))
            .Add("CategoryDescription", GetType(String))
            .Add("TextDescription", GetType(String))
            .Add("RestrictionCode", GetType(String))
            .Add("RestrictionDescription", GetType(String))
            .Add("Customer", GetType(String))
            .Add("Forename", GetType(String))
            .Add("Surname", GetType(String))
            .Add("AddressLine1", GetType(String))
            .Add("AddressLine2", GetType(String))
            .Add("AddressLine3", GetType(String))
            .Add("SeasonTicketSeat", GetType(String))
            .Add("SeasonTicketBookNumber", GetType(String))
        End With

        Try
            CallWS024R(PARAMOUT1, PARAMOUT2)
            dRow = Nothing
            dRow = DtStatusResults.NewRow
            If PARAMOUT1.Substring(1023, 1) = GlobalConstants.ERRORFLAG Or PARAMOUT1.Substring(1021, 2).Trim <> "" Then
                dRow("ErrorOccurred") = GlobalConstants.ERRORFLAG
                dRow("ReturnCode") = PARAMOUT1.Substring(1021, 2)
            Else
                dRow("ErrorOccurred") = ""
                dRow("ReturnCode") = ""
                DtStatusResults.Rows.Add(dRow)
                If PARAMOUT2.Substring(1, 1) <> String.Empty Then
                    dRow = Nothing
                    dRow = dtSeatDetails.NewRow
                    dRow("Available") = convertToBool(PARAMOUT2.Substring(0, 1).Trim())
                    dRow("Category") = PARAMOUT2.Substring(1, 2).Trim()
                    dRow("CategoryDescription") = PARAMOUT2.Substring(3, 30).Trim()
                    dRow("TextDescription") = PARAMOUT2.Substring(33, 20).Trim()
                    dRow("RestrictionCode") = PARAMOUT2.Substring(53, 2).Trim()
                    dRow("RestrictionDescription") = PARAMOUT2.Substring(55, 20).Trim()
                    dRow("Customer") = PARAMOUT2.Substring(75, 12).Trim()
                    dRow("Forename") = PARAMOUT2.Substring(87, 20).Trim()
                    dRow("Surname") = PARAMOUT2.Substring(107, 30).Trim()
                    dRow("AddressLine1") = PARAMOUT2.Substring(137, 20).Trim()
                    dRow("AddressLine2") = PARAMOUT2.Substring(157, 20).Trim()
                    dRow("AddressLine3") = PARAMOUT2.Substring(187, 30).Trim()
                    dRow("SeasonTicketSeat") = PARAMOUT2.Substring(217, 20).Trim()
                    dRow("SeasonTicketBookNumber") = PARAMOUT2.Substring(237, 7).Trim()

                    dtSeatDetails.Rows.Add(dRow)
                End If
            End If
        Catch ex As Exception
            ResultDataSet = Nothing
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBPD-WS024R"
                .HasError = True
            End With
        End Try
        Return err
    End Function

    Private Sub CallWS024R(ByRef PARAMOUT1 As String, ByRef PARAMOUT2 As String)
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strHEADER As New StringBuilder
        Dim parmIO1 As iDB2Parameter
        Dim parmIO2 As iDB2Parameter

        strHEADER.Append("CALL ")
        strHEADER.Append(Settings.StoredProcedureGroup.Trim)
        strHEADER.Append("/WS024R(@PARAM1,@PARAM2)")
        cmdSELECT = New iDB2Command(strHEADER.ToString(), conTALENTTKT)

        parmIO1 = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 1024)
        parmIO1.Value = WS024Parm()
        parmIO1.Direction = ParameterDirection.InputOutput
        parmIO2 = cmdSELECT.Parameters.Add(Param2, iDB2DbType.iDB2Char, 10000)
        parmIO2.Value = Utilities.FixStringLength("", 10000)
        parmIO2.Direction = ParameterDirection.InputOutput

        'Execute
        TalentCommonLog("CallWS024R", "", "Backend Request: strHEADER=" & strHEADER.ToString() & ", parmIO1.Value=" & parmIO1.Value)
        cmdSELECT.ExecuteNonQuery()
        PARAMOUT1 = cmdSELECT.Parameters(Param1).Value.ToString()
        PARAMOUT2 = cmdSELECT.Parameters(Param2).Value.ToString()
        TalentCommonLog("CallWS024R", "", "Backend Response: PARAMOUT2=" & PARAMOUT2)
    End Sub

    Private Function WS024Parm() As String
        Dim myString As New StringBuilder
        myString.Append(Utilities.FixStringLength(De.ProductCode, 6))
        myString.Append(Utilities.FixStringLength(De.StandCode, 3))
        myString.Append(Utilities.FixStringLength(De.AreaCode, 4))
        myString.Append(Utilities.FixStringLength(De.SeatRow, 4))

        'Handle seat alpha character
        If De.SeatNumber.Length = 5 Then
            myString.Append(De.SeatNumber.Substring(0, 4))
            myString.Append(De.SeatNumber.Substring(4, 1))
        Else
            myString.Append(Utilities.FixStringLength(De.SeatNumber, 4))
            myString.Append(Utilities.FixStringLength(String.Empty, 1))
        End If

        myString.Append(Utilities.FixStringLength(String.Empty, 999))
        myString.Append("   ") 'Error code and error flag
        Return myString.ToString()
    End Function

    Private Function AccessDatabaseMD188R() As ErrorObj
        Dim err As New ErrorObj
        ResultDataSet = New DataSet
        Dim dRow As DataRow = Nothing
        Dim PARAMOUT1 As String = String.Empty
        Dim PARAMOUT2 As String = String.Empty
        Dim moreRecords As Boolean = True
        Dim lastRecord As String = String.Empty

        Dim DtStatusResults As New DataTable("Status")
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        Dim dtSeatDetails As New DataTable("SeatHistory")
        ResultDataSet.Tables.Add(dtSeatDetails)
        With dtSeatDetails.Columns
            .Add("ProductDescription", GetType(String))
            .Add("Date", GetType(Date))
            .Add("Time", GetType(String))
            .Add("Action", GetType(String))
            .Add("Agent", GetType(String))
            .Add("CustomerNumber", GetType(String))
            .Add("Batch", GetType(String))
            .Add("PaymentReference", GetType(String))
            .Add("CommentLine1", GetType(String))
            .Add("CommentLine2", GetType(String))
            .Add("CurrentSeatStatus", GetType(String))
        End With

        Try
            Do While moreRecords
                CallMD188R(PARAMOUT1, PARAMOUT2, moreRecords, lastRecord)
                dRow = Nothing
                dRow = DtStatusResults.NewRow
                If PARAMOUT2.Substring(5119, 1) = GlobalConstants.ERRORFLAG Or PARAMOUT2.Substring(5117, 2).Trim <> "" Then
                    dRow("ErrorOccurred") = GlobalConstants.ERRORFLAG
                    dRow("ReturnCode") = PARAMOUT2.Substring(5117, 2)
                    moreRecords = False
                Else
                    dRow("ErrorOccurred") = ""
                    dRow("ReturnCode") = ""
                    DtStatusResults.Rows.Add(dRow)
                    Dim iPosition As Integer = 0
                    Dim icount As Integer = 0
                    Do While icount < 10
                        dRow = Nothing
                        dRow = dtSeatDetails.NewRow
                        If PARAMOUT2.Substring(iPosition + 249, 7).Trim() <> String.Empty Then
                            dRow("ProductDescription") = PARAMOUT2.Substring(54, 40).Trim()
                            dRow("Date") = Utilities.ISeriesDate(PARAMOUT2.Substring(iPosition + 249, 7).Trim())
                            dRow("Time") = Utilities.ISeriesTime(PARAMOUT2.Substring(iPosition + 256, 6).Trim())
                            dRow("Action") = PARAMOUT2.Substring(iPosition + 262, 12).Trim().Replace(" ", "_")
                            dRow("Agent") = PARAMOUT2.Substring(iPosition + 274, 10).Trim()
                            dRow("CustomerNumber") = PARAMOUT2.Substring(iPosition + 284, 12).Trim()
                            dRow("Batch") = PARAMOUT2.Substring(iPosition + 296, 7).Trim()
                            dRow("PaymentReference") = PARAMOUT2.Substring(iPosition + 303, 10).Trim()
                            dRow("CommentLine1") = PARAMOUT2.Substring(iPosition + 313, 70).Trim().Replace(" ", "_")
                            dRow("CommentLine2") = PARAMOUT2.Substring(iPosition + 383, 70).Trim().Replace(" ", "_")
                            dRow("CurrentSeatStatus") = PARAMOUT2.Substring(44, 10).Trim()
                            dtSeatDetails.Rows.Add(dRow)
                        Else
                            Exit Do
                        End If
                        iPosition += 450
                        icount = icount + 1
                    Loop

                    moreRecords = convertToBool(PARAMOUT2.Substring(5110, 1))
                    If moreRecords Then lastRecord = PARAMOUT2.Substring(5111, 4)
                End If
            Loop
        Catch ex As Exception
            ResultDataSet = Nothing
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBPD-AccessDatabaseMD188R"
                .HasError = True
            End With
        End Try

        Return err
    End Function

    Private Sub CallMD188R(ByRef PARAMOUT1 As String, ByRef PARAMOUT2 As String, ByRef moreRecords As Boolean, ByRef lastRecord As String)
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strHEADER As New StringBuilder
        Dim parmIO1 As iDB2Parameter
        Dim parmIO2 As iDB2Parameter

        strHEADER.Append("CALL ")
        strHEADER.Append(Settings.StoredProcedureGroup.Trim)
        strHEADER.Append("/MD188R(@PARAM1,@PARAM2)")
        cmdSELECT = New iDB2Command(strHEADER.ToString(), conTALENTTKT)

        parmIO1 = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 15)
        parmIO1.Value = Utilities.PadLeadingZeros(String.Empty, 15)
        parmIO1.Direction = ParameterDirection.InputOutput
        parmIO2 = cmdSELECT.Parameters.Add(Param2, iDB2DbType.iDB2Char, 5120)
        parmIO2.Value = MD188Parm2(moreRecords, lastRecord)
        parmIO2.Direction = ParameterDirection.InputOutput

        'Execute
        TalentCommonLog("CallMD188R", "", "Backend Request: strHEADER=" & strHEADER.ToString() & ", parmIO1.Value=" & parmIO1.Value)
        cmdSELECT.ExecuteNonQuery()
        PARAMOUT1 = cmdSELECT.Parameters(Param1).Value.ToString()
        PARAMOUT2 = cmdSELECT.Parameters(Param2).Value.ToString()
        TalentCommonLog("CallMD188R", "", "Backend Response: PARAMOUT2=" & PARAMOUT2)
    End Sub

    Private Function MD188Parm2(ByRef moreRecords As Boolean, ByRef lastRecord As String) As String
        Dim myString As New StringBuilder
        myString.Append(Utilities.FixStringLength(De.ProductCode, 6))
        myString.Append(Utilities.FixStringLength(De.FullSeatDetails.FormattedSeat, 20))
        myString.Append(Utilities.FixStringLength(De.PaymentReference, 15))
        myString.Append(Utilities.FixStringLength(Settings.Company, 3))
        myString.Append(Utilities.FixStringLength(String.Empty, 10)) 'Current seat status '54
        myString.Append(Utilities.FixStringLength(String.Empty, 5056))
        myString.Append(Utilities.FixStringLength(ConvertToYN(moreRecords), 1))
        myString.Append(Utilities.FixStringLength(lastRecord, 5))
        myString.Append(Utilities.FixStringLength(GlobalConstants.SOURCE, 1))
        Return myString.ToString()
    End Function

    Private Function PerformRetrieveSeatPrintHistory() As ErrorObj
        Dim err As New ErrorObj
        Dim cmdSelect As iDB2Command = Nothing
        Dim dtr As iDB2DataReader = Nothing
        Dim dtSeatPrintHistory As New DataTable
        ResultDataSet = New DataSet
        dtSeatPrintHistory.TableName = "SeatPrintHistory"
        dtSeatPrintHistory.Columns.Add("Agent", GetType(String))
        dtSeatPrintHistory.Columns.Add("Date", GetType(Date))
        dtSeatPrintHistory.Columns.Add("Time", GetType(String))
        dtSeatPrintHistory.Columns.Add("CustomerNumber", GetType(String))
        dtSeatPrintHistory.Columns.Add("CustomerForename", GetType(String))
        dtSeatPrintHistory.Columns.Add("CustomerSurname", GetType(String))
        dtSeatPrintHistory.Columns.Add("Program", GetType(String))
        dtSeatPrintHistory.Columns.Add("ProductDescription", GetType(String))
        dtSeatPrintHistory.Columns.Add("TicketsPrinted", GetType(Integer))

        Try
            Dim sqlselect As New StringBuilder
            sqlselect.Append("SELECT USER01, UPDT01, UPTM01, MEMB01, FNAM20, SNAM20, PGMD01, DESC01, QTTK01 ")
            sqlselect.Append("FROM CF001L2, CD020 ")
            sqlselect.Append("WHERE CONO01 = @COMPANY AND MTCD01 = @MATCH  AND STND01 = @STAND ")
            sqlselect.Append("AND AREA01 = @AREA AND ROWN01 = @ROW AND SNUM01 = @SEAT AND ASFX01 = @SUFFIX ")
            sqlselect.Append("AND PGMD01 <> 'CF007R' AND CONO20 = @COMPANY AND MEMB20 = MEMB01 ")
            cmdSelect = New iDB2Command(sqlselect.ToString(), conTALENTTKT)
            cmdSelect.Parameters.Add("@COMPANY", iDB2DbType.iDB2VarChar, 3).Value = Settings.Company
            cmdSelect.Parameters.Add("@MATCH", iDB2DbType.iDB2VarChar, 6).Value = De.ProductCode
            cmdSelect.Parameters.Add("@STAND", iDB2DbType.iDB2VarChar, 3).Value = De.FullSeatDetails.Stand
            cmdSelect.Parameters.Add("@AREA", iDB2DbType.iDB2VarChar, 4).Value = De.FullSeatDetails.Area
            cmdSelect.Parameters.Add("@ROW", iDB2DbType.iDB2VarChar, 4).Value = De.FullSeatDetails.Row
            cmdSelect.Parameters.Add("@SEAT", iDB2DbType.iDB2VarChar, 4).Value = De.FullSeatDetails.Seat
            cmdSelect.Parameters.Add("@SUFFIX", iDB2DbType.iDB2VarChar, 1).Value = De.FullSeatDetails.AlphaSuffix
            cmdSelect.Parameters.Add("@COMPANY", iDB2DbType.iDB2VarChar, 3).Value = Settings.Company
            dtr = cmdSelect.ExecuteReader

            While dtr.Read
                Dim dRow As DataRow = Nothing
                dRow = dtSeatPrintHistory.NewRow
                dRow("Agent") = dtr.Item("USER01").ToString().Trim()
                dRow.Item("Date") = Utilities.ISeriesDate(dtr.Item("UPDT01"))
                dRow.Item("Time") = Utilities.ISeriesTime(dtr.Item("UPTM01").ToString().Trim())
                dRow.Item("CustomerNumber") = dtr.Item("MEMB01").ToString().Trim()
                dRow.Item("CustomerForename") = dtr.Item("FNAM20").ToString().Trim()
                dRow.Item("CustomerSurname") = dtr.Item("SNAM20").ToString().Trim()
                dRow.Item("Program") = dtr.Item("PGMD01").ToString().Trim()
                dRow.Item("ProductDescription") = dtr.Item("DESC01").ToString().Trim()
                dRow.Item("TicketsPrinted") = CheckForDBNull_Int(dtr.Item("QTTK01").ToString().Trim())
                dtSeatPrintHistory.Rows.Add(dRow)
            End While

            ResultDataSet.Tables.Add(dtSeatPrintHistory)

        Catch ex As Exception
            Const strError As String = "Error Retrieving Seat Print History"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBPD-PerformRetrieveSeatPrintHistory"
                .HasError = True
            End With
        End Try
        Return err
    End Function

    Private Function AccessDatabaseWS145R() As ErrorObj
        Dim err As New ErrorObj
        ResultDataSet = New DataSet
        Dim dRow As DataRow = Nothing
        Dim PARAMOUT As String = String.Empty

        'Create the Status data table
        Dim DtStatusResults As New DataTable
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        Try
            'Call WS145R to ammend template information
            PARAMOUT = CallWS145R()

            dRow = Nothing
            dRow = DtStatusResults.NewRow
            If PARAMOUT.Substring(1023, 1) = "E" Or PARAMOUT.Substring(1022, 2).Trim <> "" Then
                dRow("ErrorOccurred") = "E"
                dRow("ReturnCode") = PARAMOUT.Substring(1022, 2)
            End If
            DtStatusResults.Rows.Add(dRow)

        Catch ex As Exception
            ResultDataSet = Nothing
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBPD-WS145R"
                .HasError = True
            End With
        End Try

        Return err
    End Function

    Private Function CallWS145R() As String

        'Create command object
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strProgram As String = "WS145R"
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim &
                                    "/" & strProgram & "(@PARAM1)"
        Dim parmIO As iDB2Parameter
        Dim PARAMOUT As String = String.Empty

        'Set the connection string
        cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)

        'Populate the parameter
        parmIO = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 1024)
        parmIO.Value = WS145Parm()
        parmIO.Direction = ParameterDirection.InputOutput

        'Execute
        TalentCommonLog("CallWS145R", "", "Backend Request: strHEADER=" & strHEADER & ", parmIO.Value=" & parmIO.Value)

        cmdSELECT.ExecuteNonQuery()
        PARAMOUT = cmdSELECT.Parameters(Param1).Value.ToString

        TalentCommonLog("CallWS145R", "", "Backend Response: PARAMOUT=" & PARAMOUT)

        Return PARAMOUT

    End Function

    Private Function WS145Parm() As String

        Dim myString As New StringBuilder
        Dim strIsActive As String
        If De.TemplateIsActive Then
            strIsActive = "Y"
        Else
            strIsActive = "N"
        End If
        'Construct the parameter
        myString.Append(Utilities.FixStringLength(_de.TemplateMode, 1))
        myString.Append(Utilities.FixStringLength(PadLeadingZeros(_de.TemplateID, 13), 13))
        myString.Append(Utilities.FixStringLength(_de.TemplateDescription, 50))
        myString.Append(Utilities.FixStringLength(strIsActive, 1))
        myString.Append(Utilities.FixStringLength(_de.TemplateTypeId, 1))
        If Not _de.TemplateTypeId.Equals(GlobalConstants.ACTIVITY_TEMPLATE_TYPE_ID_PROD_TRANS) Then
            myString.Append(Utilities.FixStringLength(_de.BusinessUnitFlag, 954))
        Else
            myString.Append(Utilities.FixStringLength(String.Empty, 954))
        End If
        myString.Append(GlobalConstants.SOURCE)
        myString.Append(Utilities.FixStringLength("", 3))
        Return myString.ToString()

    End Function
    Private Function AccessDatabaseLP001S() As ErrorObj
        Dim err As New ErrorObj
        ResultDataSet = New DataSet

        'Create the Status data table
        Dim DtStatusResults As New DataTable("Error Status")
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        'Create the Status data table
        Dim DtRelationshipDetails As New DataTable("Relationship Details")
        ResultDataSet.Tables.Add(DtRelationshipDetails)
        With DtRelationshipDetails.Columns
            .Add("ProductRelationsID", GetType(Integer))
        End With

        Try
            Dim cmd As iDB2Command = conTALENTTKT.CreateCommand()
            Dim cmdAdapter As iDB2DataAdapter = Nothing

            cmd = conTALENTTKT.CreateCommand()
            cmd.CommandText = "CALL LP001S(@PARAM0, @PARAM1, @PARAM2, @PARAM3, @PARAM4, @PARAM5," &
                                          "@PARAM6, @PARAM7, @PARAM8, @PARAM9, @PARAM10, @PARAM11," &
                                          "@PARAM12, @PARAM13, @PARAM14, @PARAM15, @PARAM16, @PARAM17, @PARAM18)"
            cmd.CommandType = CommandType.Text

            Dim pMode As New iDB2Parameter
            Dim pProductRelationID As iDB2Parameter
            Dim pMasterPackageProduct As iDB2Parameter
            Dim pPackageProduct As iDB2Parameter
            Dim pPriceCode As New iDB2Parameter
            Dim pStandCode As iDB2Parameter
            Dim pAreaCode As iDB2Parameter
            Dim pQuantity As iDB2Parameter
            Dim pCampaignCode As iDB2Parameter
            Dim pProductDetailID As New iDB2Parameter
            Dim pSource As iDB2Parameter
            Dim pNumLevelCheck As iDB2Parameter
            Dim pComponentValue1 As iDB2Parameter
            Dim pComponentValue2 As iDB2Parameter
            Dim pComponentValue3 As iDB2Parameter
            Dim pComponentValue4 As iDB2Parameter
            Dim pComponentValue5 As iDB2Parameter
            Dim pComponentPriceBands As iDB2Parameter
            Dim pErrorCode As iDB2Parameter

            pMode = cmd.Parameters.Add(Param0, iDB2DbType.iDB2Char, 1)
            pProductRelationID = cmd.Parameters.Add(Param1, iDB2DbType.iDB2Decimal, 13)
            pProductRelationID.Direction = ParameterDirection.InputOutput
            pMasterPackageProduct = cmd.Parameters.Add(Param2, iDB2DbType.iDB2Char, 6)
            pPackageProduct = cmd.Parameters.Add(Param3, iDB2DbType.iDB2Char, 6)
            pPriceCode = cmd.Parameters.Add(Param4, iDB2DbType.iDB2Char, 2)
            pStandCode = cmd.Parameters.Add(Param5, iDB2DbType.iDB2Char, 3)
            pAreaCode = cmd.Parameters.Add(Param6, iDB2DbType.iDB2Char, 4)
            pQuantity = cmd.Parameters.Add(Param7, iDB2DbType.iDB2Decimal, 5)
            pCampaignCode = cmd.Parameters.Add(Param8, iDB2DbType.iDB2Char, 6)
            pProductDetailID = cmd.Parameters.Add(Param9, iDB2DbType.iDB2Char, 6)
            pSource = cmd.Parameters.Add(Param10, iDB2DbType.iDB2Char, 1)
            pNumLevelCheck = cmd.Parameters.Add(Param11, iDB2DbType.iDB2Decimal, 4)
            pComponentValue1 = cmd.Parameters.Add(Param12, iDB2DbType.iDB2Decimal, 11)
            pComponentValue2 = cmd.Parameters.Add(Param13, iDB2DbType.iDB2Decimal, 11)
            pComponentValue3 = cmd.Parameters.Add(Param14, iDB2DbType.iDB2Decimal, 11)
            pComponentValue4 = cmd.Parameters.Add(Param15, iDB2DbType.iDB2Decimal, 11)
            pComponentValue5 = cmd.Parameters.Add(Param16, iDB2DbType.iDB2Decimal, 11)
            pComponentPriceBands = cmd.Parameters.Add(Param17, iDB2DbType.iDB2Char, 9)
            pErrorCode = cmd.Parameters.Add(Param18, iDB2DbType.iDB2Char, 10)
            pErrorCode.Direction = ParameterDirection.InputOutput

            pMode.Value = De.LinkedProductPackageMode
            pProductRelationID.Value = De.ProductRelationsID
            pMasterPackageProduct.Value = De.MasterPackageProduct
            pPackageProduct.Value = De.RelatedProduct
            pPriceCode.Value = De.PriceCode
            pStandCode.Value = De.StandCode
            pAreaCode.Value = De.AreaCode
            pQuantity.Value = De.Quantity
            pCampaignCode.Value = De.CampaignCode
            pProductDetailID.Value = De.ProductDetailID
            pSource.Value = De.Src
            pNumLevelCheck.Value = De.NumOfMasterProducts
            pComponentValue1.Value = De.PackageComponentValue1
            pComponentValue2.Value = De.PackageComponentValue2
            pComponentValue3.Value = De.PackageComponentValue3
            pComponentValue4.Value = De.PackageComponentValue4
            pComponentValue5.Value = De.PackageComponentValue5
            pComponentPriceBands.Value = De.PackageComponentPriceBands
            pErrorCode.Value = String.Empty

            cmd.ExecuteNonQuery()

            Dim drStatus As DataRow = DtStatusResults.NewRow
            If CStr(cmd.Parameters(18).Value).Trim.Length > 0 Then
                drStatus("ErrorOccurred") = CStr(cmd.Parameters(18).Value).Trim
                drStatus("ReturnCode") = "E"
            End If
            DtStatusResults.Rows.Add(drStatus)

            Dim drRelationshipDetails As DataRow = DtRelationshipDetails.NewRow
            If CStr(cmd.Parameters(1).Value).Trim.Length > 0 Then
                drRelationshipDetails("ProductRelationsID") = CStr(cmd.Parameters(1).Value).Trim
            End If
            DtRelationshipDetails.Rows.Add(drRelationshipDetails)

        Catch ex As Exception
            Const strError8 As String = "Error during database Access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError8
                .ErrorNumber = "TACDBPD-LP001S"
                .HasError = True
            End With
        End Try

        Return err


    End Function
    Private Function AccessDatabaseWS146R() As ErrorObj
        Dim err As New ErrorObj
        ResultDataSet = New DataSet
        Dim dRow As DataRow = Nothing
        Dim PARAMOUT As String = String.Empty
        Dim bMoreRecords As Boolean = True
        Dim moreRecords As String = ""
        Dim nextRecord As String = "0000"
        Dim QA002RRN As Integer = 0
        Dim CurrentCallNumber As Integer = 0

        'Create the Status data table
        Dim DtStatusResults As New DataTable("StatusResults")
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        'Create the product questions answers data table from WS003B
        Dim DtCurrentProductQuestionsAnswers As New DataTable("CurrentQuestionsAndAnswers")
        ResultDataSet.Tables.Add(DtCurrentProductQuestionsAnswers)
        With DtCurrentProductQuestionsAnswers.Columns
            .Add("Question", GetType(String))
            .Add("Answer", GetType(String))
            .Add("PurchaseID", GetType(String))
            .Add("AllocationID", GetType(String))
            .Add("ProductCode", GetType(String))
            .Add("QuestionID", GetType(Integer))
            .Add("PriceBand", GetType(String))
            .Add("SeatDetails", GetType(String))
            .Add("CallID", GetType(Integer))
        End With

        'Create the product questions answers data table from QA002
        Dim DtPreviousProductQuestionsAnswers As New DataTable("PreviousQuestionsAndAnswers")
        ResultDataSet.Tables.Add(DtPreviousProductQuestionsAnswers)
        With DtPreviousProductQuestionsAnswers.Columns
            .Add("Question", GetType(String))
            .Add("Answer", GetType(String))
            .Add("QuestionID", GetType(Integer))
            .Add("CallID", GetType(Integer))
        End With

        Try
            Do While bMoreRecords = True
                CurrentCallNumber = CurrentCallNumber + 1
                'Call WS146R to retrieve template information
                PARAMOUT = CallWS146R(nextRecord, moreRecords, QA002RRN, CurrentCallNumber)
                If PARAMOUT.Substring(32764, 1) <> "E" And PARAMOUT.Substring(32762, 2).Trim = "" Then
                    moreRecords = PARAMOUT.Substring(32756, 1)
                    nextRecord = PARAMOUT.Substring(32757, 4)
                    QA002RRN = Utilities.CheckForDBNull_Int(PARAMOUT.Substring(32693, 14))
                    Dim position As Integer = 0

                    For count As Integer = 0 To 20
                        If String.IsNullOrEmpty(PARAMOUT.Substring(position + 1047, 10).Trim) Then Exit For
                        dRow = Nothing
                        If PARAMOUT.Substring(position + 1012, 12).Trim = String.Empty Then
                            dRow = DtPreviousProductQuestionsAnswers.NewRow
                            dRow("Question") = PARAMOUT.Substring(position, 500).TrimEnd
                            dRow("Answer") = PARAMOUT.Substring(position + 500, 500).TrimEnd
                            dRow("QuestionID") = Utilities.CheckForDBNull_Int(PARAMOUT.Substring(position + 1047, 10).Trim)
                            dRow("CallID") = Utilities.CheckForDBNull_Int(PARAMOUT.Substring(position + 1057, 13).Trim)
                            DtPreviousProductQuestionsAnswers.Rows.Add(dRow)
                        Else
                            dRow = DtCurrentProductQuestionsAnswers.NewRow
                            dRow("Question") = PARAMOUT.Substring(position, 500).TrimEnd
                            dRow("Answer") = PARAMOUT.Substring(position + 500, 500).TrimEnd
                            dRow("PurchaseID") = PARAMOUT.Substring(position + 1000, 12).Trim
                            dRow("AllocationID") = PARAMOUT.Substring(position + 1012, 12).Trim
                            dRow("ProductCode") = PARAMOUT.Substring(position + 1024, 6).Trim
                            dRow("PriceBand") = PARAMOUT.Substring(position + 1030, 1).Trim
                            dRow("SeatDetails") = PARAMOUT.Substring(position + 1031, 16)
                            dRow("QuestionID") = Utilities.CheckForDBNull_Int(PARAMOUT.Substring(position + 1047, 10).Trim)
                            dRow("CallID") = Utilities.CheckForDBNull_Int(PARAMOUT.Substring(position + 1057, 13).Trim)
                            DtCurrentProductQuestionsAnswers.Rows.Add(dRow)
                        End If

                        'Increment
                        position = position + 1500
                    Next
                    If moreRecords.Trim = "Y" Then
                        bMoreRecords = True
                    Else
                        bMoreRecords = False
                    End If
                Else
                    bMoreRecords = False
                End If
            Loop

            dRow = Nothing
            dRow = DtStatusResults.NewRow
            If PARAMOUT.Substring(32764, 1) = "E" Or PARAMOUT.Substring(32762, 2).Trim <> "" Then
                dRow("ErrorOccurred") = "E"
                dRow("ReturnCode") = PARAMOUT.Substring(32762, 2)
            Else
                dRow("ErrorOccurred") = String.Empty
                dRow("ReturnCode") = String.Empty
            End If
            DtStatusResults.Rows.Add(dRow)
        Catch ex As Exception
            ResultDataSet = Nothing
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBPD-WS146R"
                .HasError = True
            End With
        End Try

        Return err
    End Function

    Private Function CallWS146R(ByVal nextRecord As String, ByVal moreRecords As String, ByVal QA002RRN As Integer, ByVal CurrentCallNumber As Integer) As String
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & "/WS146R(@PARAM1)"
        Dim parmIO As iDB2Parameter
        Dim PARAMOUT As String = String.Empty

        'Set the connection string
        cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)
        parmIO = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 32765)
        parmIO.Value = WS146Parm(nextRecord, moreRecords, QA002RRN, CurrentCallNumber)
        parmIO.Direction = ParameterDirection.InputOutput

        'Execute
        TalentCommonLog("CallWS146R", "", "Backend Request: strHEADER=" & strHEADER & ", parmIO.Value=" & parmIO.Value)
        cmdSELECT.ExecuteNonQuery()
        PARAMOUT = cmdSELECT.Parameters(Param1).Value.ToString
        TalentCommonLog("CallWS146R", "", "Backend Response: PARAMOUT=" & PARAMOUT)

        Return PARAMOUT
    End Function

    Private Function WS146Parm(ByVal nextRecord As String, ByVal moreRecords As String, ByVal QA002RRN As Integer, ByVal CurrentCallNumber As Integer) As String
        Dim myString As New StringBuilder
        myString.Append(Utilities.FixStringLength(String.Empty, 30000))                                     '1-30000 questions/answers array
        myString.Append(Utilities.PadLeadingZeros(_dep.ProductQuestionAnswers.CallID, 13))
        myString.Append(Utilities.PadLeadingZeros(CurrentCallNumber, 5))
        myString.Append(Utilities.FixStringLength(String.Empty, 2025))
        For Each dataEntityItem As DEProductQuestionsAnswers In _dep.CollDEProductQuestionAnswers
            myString.Append(Utilities.PadLeadingZeros(dataEntityItem.QuestionID, 13))                       '50 x 13 long (question IDs)
        Next
        Dim padQuestionIDs As Integer = 13 * (50 - _dep.CollDEProductQuestionAnswers.Count)
        myString.Append(Utilities.PadLeadingZeros(String.Empty, padQuestionIDs))                            'Fill remaining space
        myString.Append(Utilities.PadLeadingZeros(QA002RRN, 14))
        myString.Append(Utilities.FixStringLength(String.Empty, 1))
        myString.Append(Utilities.PadLeadingZeros(_dep.ProductQuestionAnswers.CustomerNumber, 12))
        myString.Append(Utilities.FixStringLength(_dep.ProductQuestionAnswers.BasketID, 36))
        If Dep.ProductQuestionAnswers.CallID Then
            myString.Append(Utilities.FixStringLength(moreRecords, 1))
        Else
            myString.Append(Utilities.FixStringLength(String.Empty, 1))
        End If

        myString.Append(Utilities.FixStringLength(nextRecord, 4))
        myString.Append(GlobalConstants.SOURCE)
        myString.Append(Utilities.FixStringLength(String.Empty, 3))
        Return myString.ToString()
    End Function

    Private Function AccessDatabaseWS144R() As ErrorObj
        Dim err As New ErrorObj
        ResultDataSet = New DataSet
        Dim dRow As DataRow = Nothing
        Dim PARAMOUT As String = String.Empty
        Dim repeaterId As String = String.Empty

        'Create the Status data table
        Dim DtStatusResults As New DataTable("StatusResults")
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        Try
            Dim filterQnAs As List(Of DEProductQuestionsAnswers)
            Dim productEntity As DEProduct = New DEProduct
            Dim questionAnswersList As List(Of DEProductQuestionsAnswers) = _dep.CollDEProductQuestionAnswers
            Dim errorString As String = String.Empty

            If questionAnswersList.Count < 20 Then
                filterQnAs = questionAnswersList.GetRange(0, questionAnswersList.Count)
                productEntity.CollDEProductQuestionAnswers = filterQnAs
                PARAMOUT = CallWS144R(productEntity, "Y", repeaterId)
                If PARAMOUT.Substring(32764, 1) = "E" Or PARAMOUT.Substring(32762, 2).Trim <> "" Then
                    errorString = "E"
                End If
            Else
                Dim c As Integer = 0
                For number As Integer = 0 To questionAnswersList.Count Step 20
                    If (number + 20) > questionAnswersList.Count Then
                        If c = 0 Then
                            filterQnAs = questionAnswersList.GetRange(0, 20)
                            productEntity.CollDEProductQuestionAnswers = filterQnAs
                            PARAMOUT = CallWS144R(productEntity, "Y", repeaterId)
                            repeaterId = PARAMOUT.Substring(32677, 10)
                            If PARAMOUT.Substring(32764, 1) = "E" Or PARAMOUT.Substring(32762, 2).Trim <> "" Then
                                errorString = "E"
                                Exit For
                            End If
                        End If
                        Dim testCount As Integer = questionAnswersList.Count - number
                        If (testCount <> 0) Then
                            filterQnAs = questionAnswersList.GetRange(number, testCount)
                            productEntity.CollDEProductQuestionAnswers = filterQnAs
                            PARAMOUT = CallWS144R(productEntity, "N", repeaterId)
                            repeaterId = PARAMOUT.Substring(32677, 10)
                            If PARAMOUT.Substring(32764, 1) = "E" Or PARAMOUT.Substring(32762, 2).Trim <> "" Then
                                errorString = "E"
                            End If
                            Exit For
                        End If
                    Else
                        If c = 0 Then
                            filterQnAs = questionAnswersList.GetRange(0, 20)
                            productEntity.CollDEProductQuestionAnswers = filterQnAs
                            PARAMOUT = CallWS144R(productEntity, "Y", repeaterId)
                            repeaterId = PARAMOUT.Substring(32677, 10)
                            If PARAMOUT.Substring(32764, 1) = "E" Or PARAMOUT.Substring(32762, 2).Trim <> "" Then
                                errorString = "E"
                                Exit For
                            End If
                        Else
                            filterQnAs = questionAnswersList.GetRange(number, 20)
                            productEntity.CollDEProductQuestionAnswers = filterQnAs
                            PARAMOUT = CallWS144R(productEntity, "N", repeaterId)
                            repeaterId = PARAMOUT.Substring(32677, 10)
                            If PARAMOUT.Substring(32764, 1) = "E" Or PARAMOUT.Substring(32762, 2).Trim <> "" Then
                                errorString = "E"
                                Exit For
                            End If
                        End If
                    End If
                    If c < 1 Then
                        c += 1
                    End If
                Next
            End If

            dRow = Nothing
            dRow = DtStatusResults.NewRow
            If PARAMOUT.Substring(32764, 1) = "E" Or PARAMOUT.Substring(32762, 2).Trim <> "" Then
                dRow("ErrorOccurred") = errorString
                dRow("ReturnCode") = PARAMOUT.Substring(32763, 2)
            Else
                dRow("ErrorOccurred") = String.Empty
                dRow("ReturnCode") = String.Empty
            End If
            DtStatusResults.Rows.Add(dRow)
        Catch ex As Exception
            ResultDataSet = Nothing
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBPD-WS145R"
                .HasError = True
            End With
        End Try

        Return err
    End Function

    Private Function CallWS144R(ByVal productEntity As DEProduct, ByVal newCallFlag As String, ByVal repeaterId As String) As String
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & "/WS144R(@PARAM1)"
        Dim parmIO As iDB2Parameter
        Dim PARAMOUT As String = String.Empty

        'Set the connection string
        cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)
        parmIO = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 32765)
        parmIO.Value = WS144Parm(productEntity, newCallFlag, repeaterId)
        parmIO.Direction = ParameterDirection.InputOutput
        'Execute
        TalentCommonLog("CallWS144R", "", "Backend Request: strHEADER=" & strHEADER & ", parmIO.Value=" & parmIO.Value)
        cmdSELECT.ExecuteNonQuery()
        PARAMOUT = cmdSELECT.Parameters(Param1).Value.ToString
        TalentCommonLog("CallWS144R", "", "Backend Response: PARAMOUT=" & PARAMOUT)

        Return PARAMOUT
    End Function

    Private Function WS144Parm(ByVal productEntity As DEProduct, ByVal newCallFlag As String, ByVal repeaterId As String) As String
        Dim myString As New StringBuilder

        For Each productQnAs As DEProductQuestionsAnswers In productEntity.CollDEProductQuestionAnswers
            myString.Append(Utilities.FixStringLength(productQnAs.QuestionText, 500))
            myString.Append(Utilities.FixStringLength(productQnAs.AnswerText, 500))
            myString.Append(Utilities.FixStringLength(productQnAs.CustomerNumber, 12))
            myString.Append(Utilities.FixStringLength(productQnAs.AllocationCustomerNumber, 12))
            myString.Append(Utilities.FixStringLength(productQnAs.ProductCode, 6))
            myString.Append(Utilities.FixStringLength(productQnAs.PriceBand, 1))
            myString.Append(Utilities.FixStringLength(productQnAs.SeatData, 15))
            myString.Append(Utilities.FixStringLength(productQnAs.AlphaSeat, 1))
            myString.Append(Utilities.PadLeadingZeros(productQnAs.QuestionID, 10))
            myString.Append(Utilities.FixStringLength(ConvertToYN(productQnAs.RememberedAnswer), 1)) '1058
            myString.Append(Utilities.FixStringLength(ConvertToYN(productQnAs.QuestionPerBooking), 1))
            myString.Append(Utilities.FixStringLength(String.Empty, 441))
        Next
        Dim blank As Integer = 31410 - myString.Length
        myString.Append(Utilities.FixStringLength(String.Empty, blank))
        myString.Append(Utilities.FixStringLength(String.Empty, 1267))
        myString.Append(Utilities.FixStringLength(repeaterId, 10))
        myString.Append(Utilities.FixStringLength(String.Empty, 1))
        myString.Append(Utilities.FixStringLength(productEntity.CollDEProductQuestionAnswers(0).AgentName, 10))
        myString.Append(Utilities.FixStringLength(productEntity.CollDEProductQuestionAnswers(0).TemplateID, 13))
        myString.Append((Utilities.PadLeadingZeros(productEntity.CollDEProductQuestionAnswers(0).CallID, 13)))
        myString.Append(Utilities.FixStringLength(De.SessionId, 36))
        myString.Append(Utilities.FixStringLength(newCallFlag, 1))
        myString.Append(Utilities.FixStringLength(GlobalConstants.SOURCE, 1))
        myString.Append(Utilities.FixStringLength(String.Empty, 3))

        Return myString.ToString()
    End Function

    Private Function AccessDatabaseMD008S() As ErrorObj
        Dim err As New ErrorObj
        ResultDataSet = New DataSet

        'Create the Status data table
        Dim DtStatusResults As New DataTable("ErrorStatus")
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        Dim DtProductEntries As New DataTable("ProductEntries")
        ResultDataSet.Tables.Add(DtProductEntries)

        Try
            Dim cmd As iDB2Command = conTALENTTKT.CreateCommand()

            cmd.CommandText = "Call MD008S(@PARAM0, @PARAM1, @PARAM2, @PARAM3, @PARAM4, @PARAM5)"
            cmd.CommandType = CommandType.Text

            Dim parmIO_0 As iDB2Parameter
            parmIO_0 = cmd.Parameters.Add(Param0, iDB2DbType.iDB2Char, 2)
            parmIO_0.Value = De.StadiumCode
            parmIO_0.Direction = ParameterDirection.Input

            Dim parmIO_1 As iDB2Parameter
            parmIO_1 = cmd.Parameters.Add(Param1, iDB2DbType.iDB2Char, 1)
            parmIO_1.Value = De.ProductType
            parmIO_1.Direction = ParameterDirection.Input

            Dim parmIO_2 As iDB2Parameter
            parmIO_2 = cmd.Parameters.Add(Param2, iDB2DbType.iDB2Char, 4)
            parmIO_2.Value = De.ProductSupertype
            parmIO_2.Direction = ParameterDirection.Input

            Dim parmIO_3 As iDB2Parameter
            parmIO_3 = cmd.Parameters.Add(Param3, iDB2DbType.iDB2Char, 4)
            parmIO_3.Value = De.ProductSubtype
            parmIO_3.Direction = ParameterDirection.Input

            Dim parmIO_4 As iDB2Parameter
            parmIO_4 = cmd.Parameters.Add(Param3, iDB2DbType.iDB2Decimal, 2.0)
            parmIO_4.Value = De.YearsOfPastProductsToShow
            parmIO_4.Direction = ParameterDirection.Input

            ' Error code 
            Dim parmIO_5 As iDB2Parameter
            parmIO_5 = cmd.Parameters.Add(Param5, iDB2DbType.iDB2Char, 10)
            parmIO_5.Value = String.Empty
            parmIO_5.Direction = ParameterDirection.InputOutput

            Dim cmdAdapter As New IBM.Data.DB2.iSeries.iDB2DataAdapter
            cmdAdapter.SelectCommand = cmd
            cmdAdapter.Fill(ResultDataSet, "ProductEntries")
            Utilities.ConvertISeriesTables(ResultDataSet)

            Dim drStatus As DataRow = DtStatusResults.NewRow
            If CStr(cmd.Parameters(1).Value).Trim.Length > 0 Then
                drStatus("ErrorOccurred") = CStr(cmd.Parameters(1).Value).Trim
                drStatus("ReturnCode") = "E"
            Else
                drStatus("ErrorOccurred") = String.Empty
                drStatus("ReturnCode") = String.Empty
            End If

            DtStatusResults.Rows.Add(drStatus)


        Catch ex As Exception
            Const strError8 As String = "Error during database Access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError8
                .ErrorNumber = "TACDBPD-MD008S"
                .HasError = True
            End With
        End Try

        Return err

    End Function

    Private Function AccessDatabaseSD002S() As ErrorObj
        Dim err As New ErrorObj
        ResultDataSet = New DataSet

        'Create the Status data table
        Dim DtStatusResults As New DataTable("ErrorStatus")
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        Dim DtStadiumStands As New DataTable("StadiumStands")
        ResultDataSet.Tables.Add(DtStadiumStands)

        Try
            Dim cmd As iDB2Command = conTALENTTKT.CreateCommand()

            cmd.CommandText = "Call SD002S(@PARAM0, @PARAM1)"
            cmd.CommandType = CommandType.Text

            Dim parmIO_0 As iDB2Parameter
            parmIO_0 = cmd.Parameters.Add(Param0, iDB2DbType.iDB2Char, 2)
            parmIO_0.Value = De.StadiumCode
            parmIO_0.Direction = ParameterDirection.Input

            Dim parmIO_1 As iDB2Parameter
            parmIO_1 = cmd.Parameters.Add(Param1, iDB2DbType.iDB2Char, 10)
            parmIO_1.Value = String.Empty
            parmIO_1.Direction = ParameterDirection.InputOutput

            Dim cmdAdapter As New IBM.Data.DB2.iSeries.iDB2DataAdapter
            cmdAdapter.SelectCommand = cmd
            cmdAdapter.Fill(ResultDataSet, "StadiumStands")

            Dim drStatus As DataRow = DtStatusResults.NewRow
            If CStr(cmd.Parameters(1).Value).Trim.Length > 0 Then
                drStatus("ErrorOccurred") = CStr(cmd.Parameters(1).Value).Trim
                drStatus("ReturnCode") = "E"
            Else
                drStatus("ErrorOccurred") = String.Empty
                drStatus("ReturnCode") = String.Empty
            End If

            DtStatusResults.Rows.Add(drStatus)


        Catch ex As Exception
            Const strError8 As String = "Error during database Access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError8
                .ErrorNumber = "TACDBPD-SD002S"
                .HasError = True
            End With
        End Try

        Return err
    End Function

    Private Function AccessDatabaseSD005S() As ErrorObj
        Dim err As New ErrorObj
        ResultDataSet = New DataSet

        'Create the Status data table
        Dim DtStatusResults As New DataTable("ErrorStatus")
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        Dim DtStadiums As New DataTable("Stadiums")
        ResultDataSet.Tables.Add(DtStadiums)

        Try
            Dim cmd As iDB2Command = conTALENTTKT.CreateCommand()

            cmd.CommandText = "Call SD005S(@PARAM0)"
            cmd.CommandType = CommandType.Text

            Dim parmIO_0 As iDB2Parameter
            parmIO_0 = cmd.Parameters.Add(Param0, iDB2DbType.iDB2Char, 10)
            parmIO_0.Value = String.Empty
            parmIO_0.Direction = ParameterDirection.InputOutput

            Dim cmdAdapter As New IBM.Data.DB2.iSeries.iDB2DataAdapter
            cmdAdapter.SelectCommand = cmd
            cmdAdapter.Fill(ResultDataSet, "Stadiums")

            Dim drStatus As DataRow = DtStatusResults.NewRow
            If CStr(cmd.Parameters(0).Value).Trim.Length > 0 Then
                drStatus("ErrorOccurred") = CStr(cmd.Parameters(0).Value).Trim
                drStatus("ReturnCode") = "E"
            Else
                drStatus("ErrorOccurred") = String.Empty
                drStatus("ReturnCode") = String.Empty
            End If

            DtStatusResults.Rows.Add(drStatus)


        Catch ex As Exception
            Const strError8 As String = "Error during database Access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError8
                .ErrorNumber = "TACDBPD-SD005S"
                .HasError = True
            End With
        End Try

        Return err
    End Function

    Private Function AccessDatabaseSD021S() As ErrorObj
        Dim err As New ErrorObj
        ResultDataSet = New DataSet

        'Create the Status data table
        Dim DtStatusResults As New DataTable("ErrorStatus")
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        Dim DtStadiumStandAreas As New DataTable("StadiumStandAreas")
        ResultDataSet.Tables.Add(DtStadiumStandAreas)

        Try
            Dim cmd As iDB2Command = conTALENTTKT.CreateCommand()

            cmd.CommandText = "Call SD021S(@PARAM0, @PARAM1)"
            cmd.CommandType = CommandType.Text

            Dim parmIO_0 As iDB2Parameter
            parmIO_0 = cmd.Parameters.Add(Param0, iDB2DbType.iDB2Char, 2)
            parmIO_0.Value = De.StadiumCode
            parmIO_0.Direction = ParameterDirection.Input

            Dim parmIO_1 As iDB2Parameter
            parmIO_1 = cmd.Parameters.Add(Param1, iDB2DbType.iDB2Char, 10)
            parmIO_1.Value = String.Empty
            parmIO_1.Direction = ParameterDirection.InputOutput

            Dim cmdAdapter As New IBM.Data.DB2.iSeries.iDB2DataAdapter
            cmdAdapter.SelectCommand = cmd
            cmdAdapter.Fill(ResultDataSet, "StadiumStandAreas")

            Dim drStatus As DataRow = DtStatusResults.NewRow
            If CStr(cmd.Parameters(1).Value).Trim.Length > 0 Then
                drStatus("ErrorOccurred") = CStr(cmd.Parameters(1).Value).Trim
                drStatus("ReturnCode") = "E"
            Else
                drStatus("ErrorOccurred") = String.Empty
                drStatus("ReturnCode") = String.Empty
            End If

            DtStatusResults.Rows.Add(drStatus)

        Catch ex As Exception
            Const strError8 As String = "Error during database Access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError8
                .ErrorNumber = "TACDBPD-SD021S"
                .HasError = True
            End With
        End Try

        Return err
    End Function

    ''' <summary>
    ''' Call MD011S to retrieve away product availability information
    ''' </summary>
    ''' <returns>Error object</returns>
    ''' <remarks></remarks>
    Private Function AccessDatabaseMD011S() As ErrorObj
        Dim err As New ErrorObj
        ResultDataSet = New DataSet

        'Create the Status data table
        Dim dtStatusResults As New DataTable("ErrorStatus")
        ResultDataSet.Tables.Add(dtStatusResults)
        With dtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        'Create the availability table (specific to product type)
        Select Case _settings.ModuleName
            Case Is = AwayProductAvailability
                Dim dtAwayProdAvail As New DataTable("AwayProductAvailability")
                With dtAwayProdAvail.Columns
                    .Add("ProductCode", GetType(String))
                    .Add("PriceCode", GetType(String))
                    .Add("Availability", GetType(Integer))
                    .Add("Returned", GetType(Integer))
                    .Add("Percentage", GetType(Integer))
                    .Add("Capacity", GetType(Integer))
                    .Add("Sold", GetType(Integer))
                    .Add("Reserved", GetType(Integer))
                    .Add("Booked", GetType(Integer))
                    .Add("Unavailable", GetType(Integer))
                End With
                ResultDataSet.Tables.Add(dtAwayProdAvail)

            Case Is = TravelProductAvailability
                Dim dtTravProdAvail As New DataTable("TravelProductAvailability")
                With dtTravProdAvail.Columns
                    .Add("ProductCode", GetType(String))
                    .Add("TravelItemCode", GetType(String))
                    .Add("Availability", GetType(Integer))
                    .Add("Returned", GetType(Integer))
                    .Add("Percentage", GetType(Integer))
                    .Add("Capacity", GetType(Integer))
                    .Add("Sold", GetType(Integer))
                    .Add("Reserved", GetType(Integer))
                    .Add("Booked", GetType(Integer))
                    .Add("Unavailable", GetType(Integer))
                End With
                ResultDataSet.Tables.Add(dtTravProdAvail)

            Case Is = EventProductAvailability
                Dim dtEventProdAvail As New DataTable("EventProductAvailability")
                With dtEventProdAvail.Columns
                    .Add("ProductCode", GetType(String))
                    .Add("Availability", GetType(Integer))
                    .Add("Returned", GetType(Integer))
                    .Add("Percentage", GetType(Integer))
                    .Add("Capacity", GetType(Integer))
                    .Add("Sold", GetType(Integer))
                    .Add("Reserved", GetType(Integer))
                    .Add("Booked", GetType(Integer))
                    .Add("Unavailable", GetType(Integer))
                End With
                ResultDataSet.Tables.Add(dtEventProdAvail)
        End Select
        Try
            CallMD011S()
        Catch ex As Exception
            Const strError8 As String = "Error during database Access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError8
                .ErrorNumber = "TACDBProduct-MD011S"
                .HasError = True
            End With
        End Try
        Return err
    End Function

    ''' <summary>
    ''' Build the parameters and call the stored procedure
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub CallMD011S()
        _cmd = conTALENTTKT.CreateCommand()
        _cmd.CommandText = "CALL MD011S(@PARAM0, @PARAM1, @PARAM2)"
        _cmd.CommandType = CommandType.Text

        Dim pErrorCode As iDB2Parameter
        Dim pSource As iDB2Parameter
        Dim pProductType As IDbDataParameter

        pErrorCode = _cmd.Parameters.Add(Param0, iDB2DbType.iDB2Char, 10)
        pErrorCode.Direction = ParameterDirection.InputOutput
        pSource = _cmd.Parameters.Add(Param1, iDB2DbType.iDB2Char, 1)
        pProductType = _cmd.Parameters.Add(Param2, iDB2DbType.iDB2Char, 1)

        pErrorCode.Value = String.Empty
        pSource.Value = De.Src
        Select Case _settings.ModuleName
            Case Is = AwayProductAvailability
                pProductType.Value = GlobalConstants.AWAYPRODUCTTYPE
            Case Is = TravelProductAvailability
                pProductType.Value = GlobalConstants.TRAVELPRODUCTTYPE
            Case Is = EventProductAvailability
                pProductType.Value = GlobalConstants.EVENTPRODUCTTYPE
            Case Else
                pProductType.Value = GlobalConstants.AWAYPRODUCTTYPE
        End Select

        _cmdAdapter = New iDB2DataAdapter
        _cmdAdapter.SelectCommand = _cmd

        Select Case _settings.ModuleName
            Case Is = AwayProductAvailability
                _cmdAdapter.Fill(ResultDataSet, "AwayProductAvailability")
            Case Is = TravelProductAvailability
                _cmdAdapter.Fill(ResultDataSet, "TravelProductAvailability")
            Case Is = EventProductAvailability
                _cmdAdapter.Fill(ResultDataSet, "EventProductAvailability")
            Case Else
                _cmdAdapter.Fill(ResultDataSet, "AwayProductAvailability")
        End Select

        Dim drStatus As DataRow = ResultDataSet.Tables("ErrorStatus").NewRow
        If CStr(_cmd.Parameters(Param0).Value).Trim.Length > 0 Then
            drStatus("ErrorOccurred") = GlobalConstants.ERRORFLAG
            drStatus("ReturnCode") = CStr(_cmd.Parameters(Param0).Value).Trim
        Else
            drStatus("ErrorOccurred") = String.Empty
            drStatus("ReturnCode") = String.Empty
        End If
        ResultDataSet.Tables("ErrorStatus").Rows.Add(drStatus)
    End Sub

    ''' <summary>
    ''' Retrieve price break information based on the product
    ''' </summary>
    ''' <returns>Error object</returns>
    ''' <remarks></remarks>
    Private Function AccessDatabaseMD141S() As ErrorObj
        Dim err As New ErrorObj
        ResultDataSet = New DataSet

        'Create the Status data table
        Dim dtStatusResults As New DataTable(GlobalConstants.STATUS_RESULTS_TABLE_NAME)
        ResultDataSet.Tables.Add(dtStatusResults)
        With dtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        'Create the Price Break data table
        Dim dtProductPriceBreaks As New DataTable("ProductPriceBreaks")
        With dtProductPriceBreaks.Columns
            .Add("PriceBreakId", GetType(Long))
            .Add("PriceBreakDescription", GetType(String))
            .Add("ProductCode", GetType(String))
            .Add("PriceCode", GetType(String))
            .Add("PriceBandA", GetType(Decimal))
            .Add("PriceBandB", GetType(Decimal))
            .Add("PriceBandC", GetType(Decimal))
            .Add("PriceBandD", GetType(Decimal))
            .Add("PriceBandE", GetType(Decimal))
            .Add("PriceBandF", GetType(Decimal))
            .Add("PriceBandG", GetType(Decimal))
            .Add("PriceBandH", GetType(Decimal))
            .Add("PriceBandI", GetType(Decimal))
            .Add("PriceBandJ", GetType(Decimal))
            .Add("PriceBandK", GetType(Decimal))
            .Add("PriceBandL", GetType(Decimal))
            .Add("PriceBandM", GetType(Decimal))
            .Add("PriceBandN", GetType(Decimal))
            .Add("PriceBandO", GetType(Decimal))
            .Add("PriceBandP", GetType(Decimal))
            .Add("PriceBandQ", GetType(Decimal))
            .Add("PriceBandR", GetType(Decimal))
            .Add("PriceBandS", GetType(Decimal))
            .Add("PriceBandT", GetType(Decimal))
            .Add("PriceBandU", GetType(Decimal))
            .Add("PriceBandV", GetType(Decimal))
            .Add("PriceBandW", GetType(Decimal))
            .Add("PriceBandX", GetType(Decimal))
            .Add("PriceBandY", GetType(Decimal))
            .Add("PriceBandAvailableZ", GetType(Boolean))
            .Add("PriceBandAvailable0", GetType(Boolean))
            .Add("PriceBandAvailable1", GetType(Boolean))
            .Add("PriceBandAvailable2", GetType(Boolean))
            .Add("PriceBandAvailable3", GetType(Boolean))
            .Add("PriceBandAvailable4", GetType(Boolean))
            .Add("PriceBandAvailable5", GetType(Boolean))
            .Add("PriceBandAvailable6", GetType(Boolean))
            .Add("PriceBandAvailable7", GetType(Boolean))
            .Add("PriceBandAvailable8", GetType(Boolean))
            .Add("PriceBandAvailable9", GetType(Boolean))
        End With
        ResultDataSet.Tables.Add(dtProductPriceBreaks)

        Try
            CallMD141S()
        Catch ex As Exception
            Const strError8 As String = "Error during database Access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError8
                .ErrorNumber = "TACDBProduct-MD141S"
                .HasError = True
            End With
        End Try
        Return err
    End Function

    ''' <summary>
    ''' Build the parameters and call the stored procedure
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub CallMD141S()
        _cmd = conTALENTTKT.CreateCommand()
        _cmd.CommandText = "CALL MD141S(@PARAM0, @PARAM1, @PARAM2)"
        _cmd.CommandType = CommandType.Text

        Dim pErrorCode As iDB2Parameter
        Dim pSource As iDB2Parameter
        Dim pProductCode As IDbDataParameter

        pErrorCode = _cmd.Parameters.Add(Param0, iDB2DbType.iDB2Char, 10)
        pErrorCode.Direction = ParameterDirection.InputOutput
        pSource = _cmd.Parameters.Add(Param1, iDB2DbType.iDB2Char, 1)
        pProductCode = _cmd.Parameters.Add(Param2, iDB2DbType.iDB2Char, 6)

        pErrorCode.Value = String.Empty
        pSource.Value = De.Src
        pProductCode.Value = De.ProductCode

        _cmdAdapter = New iDB2DataAdapter
        _cmdAdapter.SelectCommand = _cmd
        _cmdAdapter.Fill(ResultDataSet, "ProductPriceBreaks")

        Dim drStatus As DataRow = ResultDataSet.Tables(GlobalConstants.STATUS_RESULTS_TABLE_NAME).NewRow
        If CStr(_cmd.Parameters(Param0).Value).Trim.Length > 0 Then
            drStatus("ErrorOccurred") = GlobalConstants.ERRORFLAG
            drStatus("ReturnCode") = CStr(_cmd.Parameters(Param0).Value).Trim
        Else
            drStatus("ErrorOccurred") = String.Empty
            drStatus("ReturnCode") = String.Empty
        End If
        ResultDataSet.Tables(GlobalConstants.STATUS_RESULTS_TABLE_NAME).Rows.Add(drStatus)
    End Sub

    ''' <summary>
    ''' Retrieve price break information based on the product, stand and area
    ''' </summary>
    ''' <returns>Error object</returns>
    ''' <remarks></remarks>
    Private Function AccessDatabaseMD143S() As ErrorObj
        Dim err As New ErrorObj
        ResultDataSet = New DataSet

        'Create the Status data table
        Dim dtStatusResults As New DataTable("ErrorStatus")
        ResultDataSet.Tables.Add(dtStatusResults)
        With dtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        'Create the Price Break data table
        Dim dtPriceBreakDetails As New DataTable("PriceBreakSeatDetails")
        With dtPriceBreakDetails.Columns

        End With
        ResultDataSet.Tables.Add(dtPriceBreakDetails)

        Try
            CallMD143S()
        Catch ex As Exception
            Const strError8 As String = "Error during database Access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError8
                .ErrorNumber = "TACDBProduct-MD143S"
                .HasError = True
            End With
        End Try
        Return err
    End Function

    ''' <summary>
    ''' Build the parameters and call the stored procedure
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub CallMD143S()
        _cmd = conTALENTTKT.CreateCommand()
        _cmd.CommandText = "CALL MD143S(@PARAM0, @PARAM1, @PARAM2, @PARAM3, @PARAM4, @PARAM5, @PARAM6, @PARAM7)"
        _cmd.CommandType = CommandType.Text

        Dim pErrorCode As iDB2Parameter = Nothing
        Dim pSource As iDB2Parameter = Nothing
        Dim pProductCode As IDbDataParameter = Nothing
        Dim pStandCode As IDbDataParameter = Nothing
        Dim pAreaCode As IDbDataParameter = Nothing
        Dim pBoxOfficeMode As IDbDataParameter = Nothing
        Dim pCampaignCode As IDbDataParameter = Nothing
        Dim pReverseRow As IDbDataParameter = Nothing

        pErrorCode = _cmd.Parameters.Add(Param0, iDB2DbType.iDB2Char, 10)
        pErrorCode.Direction = ParameterDirection.InputOutput
        pSource = _cmd.Parameters.Add(Param1, iDB2DbType.iDB2Char, 1)
        pProductCode = _cmd.Parameters.Add(Param2, iDB2DbType.iDB2Char, 6)
        pStandCode = _cmd.Parameters.Add(Param3, iDB2DbType.iDB2Char, 3)
        pAreaCode = _cmd.Parameters.Add(Param4, iDB2DbType.iDB2Char, 4)
        pBoxOfficeMode = _cmd.Parameters.Add(Param5, iDB2DbType.iDB2Char, 1)
        pCampaignCode = _cmd.Parameters.Add(Param5, iDB2DbType.iDB2Char, 2)
        pReverseRow = _cmd.Parameters.Add(Param5, iDB2DbType.iDB2Char, 1)
        pReverseRow.Direction = ParameterDirection.Output

        pErrorCode.Value = String.Empty
        pSource.Value = De.Src
        pProductCode.Value = De.ProductCode
        pStandCode.Value = De.StandCode
        pAreaCode.Value = De.AreaCode
        pBoxOfficeMode.Value = ConvertToYN(Settings.IsAgent)
        pCampaignCode.Value = De.CampaignCode


        _cmdAdapter = New iDB2DataAdapter
        _cmdAdapter.SelectCommand = _cmd
        _cmdAdapter.Fill(ResultDataSet, "PriceBreakSeatDetails")

        Dim drStatus As DataRow = ResultDataSet.Tables("ErrorStatus").NewRow
        If CStr(_cmd.Parameters(Param0).Value).Trim.Length > 0 Then
            drStatus("ErrorOccurred") = GlobalConstants.ERRORFLAG
            drStatus("ReturnCode") = CStr(_cmd.Parameters(Param0).Value).Trim
        Else
            drStatus("ErrorOccurred") = String.Empty
            drStatus("ReturnCode") = String.Empty
            De.PriceBreakReverseRow = convertToBool(_cmd.Parameters(Param5).Value)
        End If
        ResultDataSet.Tables("ErrorStatus").Rows.Add(drStatus)
    End Sub

#Region "MD074R"

    ''' <summary>
    ''' Retrieve products by description 
    ''' </summary>
    ''' <returns>Error object</returns>
    ''' <remarks></remarks>
    Private Function AccessDatabaseMD074S() As ErrorObj
        Dim err As New ErrorObj
        ResultDataSet = New DataSet

        'Create the Status data table
        Dim dtStatusResults As New DataTable("ErrorStatus")
        ResultDataSet.Tables.Add(dtStatusResults)
        With dtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        Try
            CallMD074S()
        Catch ex As Exception
            Const strError8 As String = "Error during database Access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError8
                .ErrorNumber = "TACDBProduct-MD143S"
                .HasError = True
            End With
        End Try
        Return err
    End Function

    ''' <summary>
    ''' Build the parameters and call the stored procedure
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub CallMD074S()
        _cmd = conTALENTTKT.CreateCommand()
        _cmd.CommandText = "CALL MD074S(@PARAM0, @PARAM1, @PARAM2)"
        _cmd.CommandType = CommandType.Text

        Dim pErrorCode As iDB2Parameter = Nothing
        Dim pSource As iDB2Parameter = Nothing
        Dim pProductDescription As IDbDataParameter = Nothing

        pProductDescription = _cmd.Parameters.Add(Param0, iDB2DbType.iDB2Char, 50)
        pSource = _cmd.Parameters.Add(Param1, iDB2DbType.iDB2Char, 1)
        pErrorCode = _cmd.Parameters.Add(Param2, iDB2DbType.iDB2Char, 10)
        pErrorCode.Direction = ParameterDirection.InputOutput


        pErrorCode.Value = String.Empty
        pSource.Value = De.Src
        pProductDescription.Value = De.ProductDescription

        _cmdAdapter = New iDB2DataAdapter
        _cmdAdapter.SelectCommand = _cmd
        _cmdAdapter.Fill(ResultDataSet)
        Utilities.ConvertISeriesTables(ResultDataSet)
        If ResultDataSet.Tables.Count = 2 Then
            ResultDataSet.Tables(1).TableName = "ProductsDescriptionSearch"
        End If


        Dim drStatus As DataRow = ResultDataSet.Tables("ErrorStatus").NewRow
        If CStr(_cmd.Parameters(Param2).Value).Trim.Length > 0 Then
            drStatus("ErrorOccurred") = GlobalConstants.ERRORFLAG
            drStatus("ReturnCode") = CStr(_cmd.Parameters(Param0).Value).Trim
        Else
            drStatus("ErrorOccurred") = String.Empty
            drStatus("ReturnCode") = String.Empty
        End If
        ResultDataSet.Tables("ErrorStatus").Rows.Add(drStatus)
    End Sub

#End Region

#Region "MD075R"

    ''' <summary>
    ''' Retrieve Corporate packages by description 
    ''' </summary>
    ''' <returns>Error object</returns>
    ''' <remarks></remarks>
    Private Function AccessDatabaseMD075S() As ErrorObj
        Dim err As New ErrorObj
        ResultDataSet = New DataSet

        'Create the Status data table
        Dim dtStatusResults As New DataTable("ErrorStatus")
        ResultDataSet.Tables.Add(dtStatusResults)
        With dtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        Try
            CallMD075S()
        Catch ex As Exception
            Const strError8 As String = "Error during database Access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError8
                .ErrorNumber = "TACDBProduct-MD143S"
                .HasError = True
            End With
        End Try
        Return err
    End Function

    ''' <summary>
    ''' Build the parameters and call the stored procedure
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub CallMD075S()
        _cmd = conTALENTTKT.CreateCommand()
        _cmd.CommandText = "CALL MD075S(@PARAM0, @PARAM1, @PARAM2)"
        _cmd.CommandType = CommandType.Text

        Dim pErrorCode As iDB2Parameter = Nothing
        Dim pSource As iDB2Parameter = Nothing
        Dim pProductDescription As IDbDataParameter = Nothing

        pProductDescription = _cmd.Parameters.Add(Param0, iDB2DbType.iDB2Char, 50)
        pSource = _cmd.Parameters.Add(Param1, iDB2DbType.iDB2Char, 1)
        pErrorCode = _cmd.Parameters.Add(Param2, iDB2DbType.iDB2Char, 10)
        pErrorCode.Direction = ParameterDirection.InputOutput

        pErrorCode.Value = String.Empty
        pSource.Value = De.Src
        pProductDescription.Value = De.ProductDescription

        _cmdAdapter = New iDB2DataAdapter
        _cmdAdapter.SelectCommand = _cmd
        _cmdAdapter.Fill(ResultDataSet)
        Utilities.ConvertISeriesTables(ResultDataSet)
        If ResultDataSet.Tables.Count = 2 Then
            ResultDataSet.Tables(1).TableName = "CorporatePackageDescriptionSearch"
        End If


        Dim drStatus As DataRow = ResultDataSet.Tables("ErrorStatus").NewRow
        If CStr(_cmd.Parameters(Param2).Value).Trim.Length > 0 Then
            drStatus("ErrorOccurred") = GlobalConstants.ERRORFLAG
            drStatus("ReturnCode") = CStr(_cmd.Parameters(Param0).Value).Trim
        Else
            drStatus("ErrorOccurred") = String.Empty
            drStatus("ReturnCode") = String.Empty
        End If
        ResultDataSet.Tables("ErrorStatus").Rows.Add(drStatus)
    End Sub

#End Region

#Region "WS002R"

    Private Sub PackageListDataTable(ByRef dtPackageList As DataTable)

        With dtPackageList.Columns
            .Add("PackageID", GetType(Long))
            .Add("PackageDescription", GetType(String))
            .Add("Comment1", GetType(String))
            .Add("Comment2", GetType(String))
            .Add("PricingMethod", GetType(String))
            .Add("Availability", GetType(String))
            .Add("Price", GetType(Decimal))
            .Add("MaxQuantity", GetType(String))
            .Add("VatValue", GetType(String))
            .Add("PackageCode", GetType(String))
            .Add("AllowComments", GetType(Boolean))
            .Add("MaxPercentageToSellPWS", GetType(Decimal))
            .Add("Sold", GetType(Integer))
            .Add("Reserved", GetType(Integer))
            .Add("Cancelled", GetType(Integer))
            .Add("DataCaptureTemplateID", GetType(Decimal))

        End With

    End Sub

    Private Sub ComponentListDataTable(ByRef dtComponentList As DataTable)

        With dtComponentList.Columns
            .Add("ComponentID", GetType(Long))
            .Add("PackageID", GetType(Long))
            .Add("ComponentDescription", GetType(String))
            .Add("Comment1", GetType(String))
            .Add("UnitPrice", GetType(Decimal))
            .Add("NumberOfUnits", GetType(String))
            .Add("IsSeatComponent", GetType(Boolean))
            .Add("MinUnits", GetType(String))
            .Add("MaxUnits", GetType(String))
            .Add("ComponentType", GetType(String))
            .Add("MaxDiscountPercent", GetType(Decimal))
            .Add("AvailableUnitsPWS", GetType(Integer))
            .Add("OriginalUnits", GetType(Integer))
            .Add("AvailableUnitsBUI", GetType(Integer))
            .Add("ProductCode", GetType(String))
        End With

    End Sub

    Private Sub LeadSourceDataTable(ByRef dtLeadSourceList As DataTable)
        With dtLeadSourceList.Columns
            .Add("LeadSourceID", GetType(String))
            .Add("LeadSourceDescription", GetType(String))
            .Add("CampaignName", GetType(String))
        End With

    End Sub

    Private Function AccessDatabaseWS002R() As ErrorObj
        Dim err As New ErrorObj
        ResultDataSet = New DataSet
        Dim sLastRecord As String = "000"
        Dim sNextPkgID As String = ""
        Dim bMoreRecords As Boolean = True
        Dim dRow As DataRow = Nothing
        Dim PARAMOUT As String = String.Empty

        'Create the Status data table
        Dim DtStatusResults As New DataTable("StatusResults")
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        'Create the package and component list data table
        Dim DtPackageListResults As New DataTable("PackageList")
        Dim DtComponentListResults As New DataTable("ComponentList")
        Dim DtLeadSourceListResults As New DataTable("LeadSourceList")
        ResultDataSet.Tables.Add(DtPackageListResults)
        ResultDataSet.Tables.Add(DtComponentListResults)
        ResultDataSet.Tables.Add(DtLeadSourceListResults)
        PackageListDataTable(DtPackageListResults)
        ComponentListDataTable(DtComponentListResults)
        LeadSourceDataTable(DtLeadSourceListResults)

        Try

            'Loop until no more products available
            Do While bMoreRecords = True

                'Call WS002R
                PARAMOUT = CallWS002R(sNextPkgID, sLastRecord)

                'Set the response data on the first call to WS002R
                If sLastRecord = "000" Then
                    dRow = Nothing
                    dRow = DtStatusResults.NewRow
                    If PARAMOUT.Substring(5117, 1) = "E" Or PARAMOUT.Substring(5118, 2).Trim <> "" Then
                        dRow("ErrorOccurred") = "E"
                        dRow("ReturnCode") = PARAMOUT.Substring(5118, 2)
                        bMoreRecords = False
                    Else
                        dRow("ErrorOccurred") = ""
                        dRow("ReturnCode") = ""
                    End If
                    DtStatusResults.Rows.Add(dRow)
                End If

                'No errors 
                If PARAMOUT.Substring(5117, 1) <> "E" And PARAMOUT.Substring(5118, 2).Trim = "" Then

                    'Extract the data from the parameter
                    Dim iPosition As Integer = 0
                    Dim iCounter As Integer = 1
                    Do While iCounter <= 20

                        ' Has a product been returned
                        If PARAMOUT.Substring(iPosition, 13).Trim = "" Then
                            Exit Do
                        Else
                            'Create a new row
                            dRow = Nothing
                            dRow = DtPackageListResults.NewRow
                            dRow("PackageID") = CLng(PARAMOUT.Substring(iPosition, 13).Trim())
                            dRow("PackageDescription") = PARAMOUT.Substring(iPosition + 13, 30).Trim()
                            dRow("Comment1") = PARAMOUT.Substring(iPosition + 43, 60).Trim()
                            dRow("Comment2") = PARAMOUT.Substring(iPosition + 103, 60).Trim()
                            dRow("PricingMethod") = PARAMOUT.Substring(iPosition + 163, 1).Trim()
                            dRow("Availability") = PARAMOUT.Substring(iPosition + 164, 3).Trim() ' davetodo i have left this for now so old style hospitality works - remove once new style finished  
                            dRow("Price") = Utilities.ConvertStringToDecimal(PARAMOUT.Substring(iPosition + 167, 10).Trim()) / 100
                            dRow("MaxQuantity") = PARAMOUT.Substring(iPosition + 177, 3).Trim()
                            dRow("VatValue") = PARAMOUT.Substring(iPosition + 180, 9).Trim()
                            dRow("PackageCode") = (PARAMOUT.Substring(iPosition + 189, 12)).Trim()
                            dRow("AllowComments") = Utilities.convertToBool(PARAMOUT.Substring(iPosition + 201, 1))
                            dRow("MaxPercentageToSellPWS") = Utilities.ConvertStringToDecimal(PARAMOUT.Substring(iPosition + 202, 3))
                            dRow("Sold") = Utilities.ConvertStringToDecimal(PARAMOUT.Substring(iPosition + 205, 5))
                            dRow("Reserved") = Utilities.ConvertStringToDecimal(PARAMOUT.Substring(iPosition + 210, 5))
                            dRow("Cancelled") = Utilities.ConvertStringToDecimal(PARAMOUT.Substring(iPosition + 215, 5))
                            dRow("DataCaptureTemplateID") = Utilities.ConvertStringToDecimal(PARAMOUT.Substring(iPosition + 220, 13).Trim())
                            DtPackageListResults.Rows.Add(dRow)

                            'Increment
                            iPosition = iPosition + 250
                            iCounter = iCounter + 1

                        End If
                    Loop
                    'Extract the component list to table
                    AddComponentListToTable(DtComponentListResults)

                    'Extract the lead source information to table
                    AddLeadSourceListToTable(DtLeadSourceListResults)

                    'Extract the footer information
                    sNextPkgID = PARAMOUT.Substring(5094, 12)
                    sLastRecord = PARAMOUT.Substring(5113, 3)
                    If PARAMOUT.Substring(5112, 1).Trim = "Y" Then
                        bMoreRecords = True
                    Else
                        bMoreRecords = False
                    End If
                Else
                    bMoreRecords = False
                End If

            Loop

        Catch ex As Exception
            ResultDataSet = Nothing
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBPD-WS002Ra"
                .HasError = True
            End With
        End Try
        Return err
    End Function

    Private Function CallWS002R(ByVal sNextPkgID As String, ByVal sLastRecord As String) As String

        'Create command object
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strProgram As String = "WS002R"
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim &
                                    "/" & strProgram & "(@PARAM1,@PARAM2,@PARM3)"
        Dim parmIO, parmIO2, parmIO3 As iDB2Parameter
        Dim PARAMOUT As String = String.Empty

        'Set the connection string
        cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)

        'Populate the parameter
        parmIO = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 5120)
        parmIO.Value = WS002Parm(sNextPkgID, sLastRecord)
        parmIO.Direction = ParameterDirection.InputOutput

        parmIO2 = cmdSELECT.Parameters.Add(Param2, iDB2DbType.iDB2Char, 10432)
        parmIO2.Value = Utilities.FixStringLength(String.Empty, 10432)
        parmIO2.Direction = ParameterDirection.InputOutput

        parmIO3 = cmdSELECT.Parameters.Add(Param3, iDB2DbType.iDB2Char, 10000)
        parmIO3.Value = Utilities.FixStringLength(String.Empty, 10000)
        parmIO3.Direction = ParameterDirection.Output

        'Execute
        TalentCommonLog("CallWS002R", "", "Backend Request: strHEADER=" & strHEADER & ", parmIO.Value=" & parmIO.Value & ", parmIO2.Value=" & parmIO2.Value & ", parmIO3.Value=" & parmIO3.Value)

        cmdSELECT.ExecuteNonQuery()
        PARAMOUT = cmdSELECT.Parameters(Param1).Value.ToString
        PARAMOUT2 = cmdSELECT.Parameters(Param2).Value.ToString
        PARAMOUT3 = cmdSELECT.Parameters(Param3).Value.ToString

        TalentCommonLog("CallWS002R", "", "Backend Response: PARAMOUT=" & PARAMOUT)

        Return PARAMOUT

    End Function

    Private Function WS002Parm(ByVal sNextPkgID As String, ByVal sLastRecord As String) As String
        Dim sb As New StringBuilder
        Dim IsAgent As String
        If Settings.IsAgent Then
            IsAgent = "Y"
        Else
            IsAgent = "N"
        End If

        'Construct the parameter
        sb.Append(Utilities.FixStringLength(String.Empty, 5073))
        sb.Append(Utilities.FixStringLength(De.BusinessUnit, 20))
        sb.Append(Utilities.FixStringLength(IsAgent, 1))
        sb.Append(Utilities.FixStringLength(sNextPkgID, 12))
        sb.Append(Utilities.FixStringLength(De.ProductCode, 6))
        sb.Append(Utilities.FixStringLength("N", 1))
        sb.Append(Utilities.FixStringLength(sLastRecord, 3))
        sb.Append(Utilities.FixStringLength(Settings.OriginatingSourceCode, 1))
        sb.Append(Utilities.FixStringLength(String.Empty, 1))
        sb.Append(Utilities.FixStringLength(String.Empty, 2))
        Return sb.ToString()
    End Function

    Private Function AddComponentListToTable(ByRef DtComponentListResults As DataTable) As ErrorObj
        Dim err As New ErrorObj
        Dim dRow As DataRow = Nothing
        Try
            Dim iPosition As Integer = 0
            Dim iCounter As Integer = 1
            Do While iCounter <= 64
                ' Has a product been returned
                If PARAMOUT2.Substring(iPosition, 13).Trim = "" Then
                    Exit Do
                Else
                    'Create a new row
                    dRow = Nothing
                    dRow = DtComponentListResults.NewRow
                    dRow("PackageID") = PARAMOUT2.Substring(iPosition, 13).Trim()
                    dRow("ComponentID") = PARAMOUT2.Substring(iPosition + 13, 13).Trim()
                    dRow("ComponentDescription") = PARAMOUT2.Substring(iPosition + 26, 30).Trim()
                    dRow("Comment1") = PARAMOUT2.Substring(iPosition + 56, 60).Trim()
                    dRow("UnitPrice") = Utilities.FormatPrice(PARAMOUT2.Substring(iPosition + 116, 10).Trim())
                    dRow("NumberOfUnits") = PARAMOUT2.Substring(iPosition + 126, 3).Trim()
                    dRow("IsSeatComponent") = Utilities.convertToBool(PARAMOUT2.Substring(iPosition + 129, 1).Trim())
                    dRow("MinUnits") = PARAMOUT2.Substring(iPosition + 130, 3).Trim()
                    dRow("MaxUnits") = PARAMOUT2.Substring(iPosition + 133, 3).Trim()
                    dRow("ComponentType") = PARAMOUT2.Substring(iPosition + 136, 1).Trim()
                    dRow("MaxDiscountPercent") = Utilities.ConvertStringToDecimal(PARAMOUT2.Substring(iPosition + 137, 5).Trim()) / 100
                    dRow("AvailableUnitsPWS") = Utilities.ConvertStringToDecimal(PARAMOUT2.Substring(iPosition + 142, 5).Trim()) 'Units of package available based on qty of this component available in PWS
                    dRow("OriginalUnits") = Utilities.ConvertStringToDecimal(PARAMOUT2.Substring(iPosition + 147, 5).Trim()) 'Units of package available before any sold based on original qty of this component 
                    dRow("AvailableUnitsBUI") = Utilities.ConvertStringToDecimal(PARAMOUT2.Substring(iPosition + 152, 5).Trim()) 'Units of package available based on qty of this component available in BUI
                    dRow("ProductCode") = PARAMOUT2.Substring(iPosition + 157, 6).Trim() 'Product code for product group
                    DtComponentListResults.Rows.Add(dRow)

                    'Increment
                    iPosition = iPosition + 163
                    iCounter = iCounter + 1

                End If
            Loop
        Catch ex As Exception
            ResultDataSet = Nothing
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "DBProduct-WS670R"
                .HasError = True
            End With
        End Try

        Return err
    End Function

    Private Function AddLeadSourceListToTable(ByRef DtLeadSourceListResults As DataTable) As ErrorObj
        Dim err As New ErrorObj
        Dim dRow As DataRow = Nothing
        Try
            Dim iPosition As Integer = 0
            Dim iCounter As Integer = 1
            Do While iCounter < 100
                ' Has a product been returned
                If PARAMOUT3.Substring(iPosition, 13).Trim = "" Then
                    Exit Do
                Else
                    'Create a new row
                    dRow = Nothing
                    dRow = DtLeadSourceListResults.NewRow
                    dRow("LeadSourceID") = Utilities.PadLeadingZeros(PARAMOUT3.Substring(iPosition, 13).Trim(), 13)
                    dRow("LeadSourceDescription") = PARAMOUT3.Substring(iPosition + 13, 30).Trim()
                    dRow("CampaignName") = PARAMOUT3.Substring(iPosition + 43, 30).Trim()
                    DtLeadSourceListResults.Rows.Add(dRow)

                    'Increment
                    iPosition = iPosition + 100
                    iCounter = iCounter + 1

                End If
            Loop
        Catch ex As Exception
            ResultDataSet = Nothing
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "DBProduct-WS670R"
                .HasError = True
            End With
        End Try

        Return err
    End Function

    Private Function AddSoldOutProductToTable(ByRef DtSoldPackagesListResults As DataTable) As ErrorObj
        Dim err As New ErrorObj
        Dim dRow As DataRow = Nothing
        Try
            Dim iPosition As Integer = 0
            Dim iCounter As Integer = 1
            Do While iCounter < 20
                ' Has a product been returned
                If PARAMOUT3.Substring(iPosition, 13).Trim = "" Then
                    Exit Do
                Else
                    'Create a new row
                    dRow = Nothing
                    dRow = DtSoldPackagesListResults.NewRow
                    dRow("PackageId") = Utilities.PadLeadingZeros(PARAMOUT4.Substring(iPosition, 13).Trim(), 13)
                    dRow("ProductCode") = PARAMOUT3.Substring(iPosition + 13, 6).Trim()
                    dRow("ProductDescription") = PARAMOUT3.Substring(iPosition + 13, 40).Trim()
                    DtSoldPackagesListResults.Rows.Add(dRow)

                    'Increment
                    iPosition = iPosition + 59
                    iCounter = iCounter + 1

                End If
            Loop
        Catch ex As Exception
            ResultDataSet = Nothing
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "DBProduct-WS670R"
                .HasError = True
            End With
        End Try

        Return err
    End Function

    Private Function AccessDatabasePG001S() As ErrorObj
        Dim err As New ErrorObj
        Dim dRow As DataRow = Nothing
        ResultDataSet = New DataSet

        'Create the Status data table
        Dim DtStatusResults As New DataTable("StatusResults")
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        Try
            CallPG001S()
        Catch ex As Exception
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = "Error during database Access"
                .ErrorNumber = "DBProduct-PG001S"
                .HasError = True
            End With
        End Try
        Return err
    End Function

    ''' <summary>
    '''  Get hospitality product group details
    ''' </summary>
    Private Sub CallPG001S()
        _cmd = conTALENTTKT.CreateCommand()
        _cmd.CommandText = "CALL PG001S(@ErrorCode, @Source, @Param0)"
        _cmd.CommandType = CommandType.Text

        Dim pErrorCode As iDB2Parameter
        Dim pSource As iDB2Parameter
        Dim pBusinessUnit As iDB2Parameter

        pErrorCode = _cmd.Parameters.Add(ErrorCode, iDB2DbType.iDB2Char, 10)
        pSource = _cmd.Parameters.Add(Source, iDB2DbType.iDB2Char, 1)
        pBusinessUnit = _cmd.Parameters.Add(Param0, iDB2DbType.iDB2Char, 20)

        pErrorCode.Direction = ParameterDirection.InputOutput

        pErrorCode.Value = String.Empty
        pSource.Value = De.Src
        pBusinessUnit.Value = De.BusinessUnit

        Dim cmdAdapter As New IBM.Data.DB2.iSeries.iDB2DataAdapter
        cmdAdapter.SelectCommand = _cmd
        cmdAdapter.Fill(ResultDataSet, "ProductGroupsDetails")

        Dim drStatus As DataRow = ResultDataSet.Tables(GlobalConstants.STATUS_RESULTS_TABLE_NAME).NewRow
        If CStr(_cmd.Parameters(ErrorCode).Value).Trim.Length > 0 Then
            drStatus("ErrorOccurred") = GlobalConstants.ERRORFLAG
            drStatus("ReturnCode") = CStr(_cmd.Parameters(ErrorCode).Value).Trim
        Else
            drStatus("ErrorOccurred") = String.Empty
            drStatus("ReturnCode") = String.Empty
        End If
        ResultDataSet.Tables(GlobalConstants.STATUS_RESULTS_TABLE_NAME).Rows.Add(drStatus)
    End Sub

    Private Function AccessDatabasePG002S() As ErrorObj
        Dim err As New ErrorObj
        Dim dRow As DataRow = Nothing
        ResultDataSet = New DataSet

        'Create the Status data table
        Dim DtStatusResults As New DataTable("StatusResults")
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With
        Dim dtProductGroupFixturesDeatils As New DataTable("ProductGroupFixturesDeatils")
        With dtProductGroupFixturesDeatils.Columns
            .Add("ProductDate", GetType(String))
        End With
        ResultDataSet.Tables.Add(dtProductGroupFixturesDeatils)

        Try
            CallPG002S()
            getFormattedDate(ResultDataSet.Tables("ProductGroupFixturesDeatils"))
        Catch ex As Exception
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = "Error during database Access"
                .ErrorNumber = "DBProduct-PG002S"
                .HasError = True
            End With
        End Try
        Return err
    End Function

    ''' <summary>
    '''  Get hospitality product group fixtures details
    ''' </summary>
    Private Sub CallPG002S()
        _cmd = conTALENTTKT.CreateCommand()
        _cmd.CommandText = "CALL PG002S( @ErrorCode, @Source, @Param0)"
        _cmd.CommandType = CommandType.Text

        Dim pErrorCode As iDB2Parameter
        Dim pSource As iDB2Parameter
        Dim pProductGroup As iDB2Parameter

        pErrorCode = _cmd.Parameters.Add(ErrorCode, iDB2DbType.iDB2Char, 10)
        pSource = _cmd.Parameters.Add(Source, iDB2DbType.iDB2Char, 1)
        pProductGroup = _cmd.Parameters.Add(Param0, iDB2DbType.iDB2Char, 6)

        pErrorCode.Direction = ParameterDirection.InputOutput

        pErrorCode.Value = String.Empty
        pSource.Value = De.Src
        pProductGroup.Value = De.ProductGroupCode

        Dim cmdAdapter As New IBM.Data.DB2.iSeries.iDB2DataAdapter
        cmdAdapter.TableMappings.Add("Table", "ProductGroupFixturesDeatils")
        cmdAdapter.SelectCommand = _cmd
        cmdAdapter.Fill(ResultDataSet)
        Utilities.ConvertISeriesTables(ResultDataSet)

        Dim drStatus As DataRow = ResultDataSet.Tables(GlobalConstants.STATUS_RESULTS_TABLE_NAME).NewRow
        If CStr(_cmd.Parameters(ErrorCode).Value).Trim.Length > 0 Then
            drStatus("ErrorOccurred") = GlobalConstants.ERRORFLAG
            drStatus("ReturnCode") = CStr(_cmd.Parameters(ErrorCode).Value).Trim
        Else
            drStatus("ErrorOccurred") = String.Empty
            drStatus("ReturnCode") = String.Empty
        End If
        ResultDataSet.Tables(GlobalConstants.STATUS_RESULTS_TABLE_NAME).Rows.Add(drStatus)
    End Sub

    ''' <summary>
    ''' Format the iSeries 7 char date CYYMMDD to a date object and then to a string based on the global date format
    ''' </summary>
    ''' <param name="dtProductGroupFixtures">The hospitality product group fixtures data table</param>
    ''' <remarks></remarks>
    Private Sub getFormattedDate(ByRef dtProductGroupFixtures)
        Dim processDate As Date = Nothing
        Dim formattedDate As String = String.Empty
        For Each row As DataRow In dtProductGroupFixtures.Rows
            processDate = Utilities.ISeriesDate(row("ISeriesProductDate"))
            formattedDate = processDate.ToString(Settings.EcommerceModuleDefaultsValues.GlobalDateFormat)
            row("ProductDate") = formattedDate
        Next
    End Sub

    Private Function AccessDatabaseWS670R() As ErrorObj
        Dim err As New ErrorObj
        ResultDataSet = New DataSet
        Dim sLastRecord As String = "000"
        Dim sNextPkgID As String = ""
        Dim sNextProductID As String = ""
        Dim bMoreRecords As Boolean = True
        Dim dRow As DataRow = Nothing
        Dim PARAMOUT As String = String.Empty
        Dim packageIds As String = String.Empty

        'Create the Status data table
        Dim DtStatusResults As New DataTable("StatusResults")
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        'Create the package and component list data table
        Dim DtPackageListResults As New DataTable("PackageList")
        Dim DtComponentListResults As New DataTable("ComponentList")
        Dim DtLeadSourceListResults As New DataTable("LeadSourceList")
        ResultDataSet.Tables.Add(DtPackageListResults)
        ResultDataSet.Tables.Add(DtComponentListResults)
        ResultDataSet.Tables.Add(DtLeadSourceListResults)
        PackageListDataTable(DtPackageListResults)
        ComponentListDataTable(DtComponentListResults)
        LeadSourceDataTable(DtLeadSourceListResults)
        Try

            'Loop until no more products available
            Do While bMoreRecords = True

                'Call WS670R

                PARAMOUT = CallWS670R(sNextPkgID, sNextProductID, sLastRecord)

                'Set the response data on the first call to WS002R
                If sLastRecord = "000" Then
                    dRow = Nothing
                    dRow = DtStatusResults.NewRow
                    If PARAMOUT.Substring(5117, 1) = "E" Or PARAMOUT.Substring(5118, 2).Trim <> "" Then
                        dRow("ErrorOccurred") = "E"
                        dRow("ReturnCode") = PARAMOUT.Substring(5118, 2)
                        bMoreRecords = False
                    Else
                        dRow("ErrorOccurred") = ""
                        dRow("ReturnCode") = ""
                    End If
                    DtStatusResults.Rows.Add(dRow)
                End If

                'No errors 
                If PARAMOUT.Substring(5117, 1) <> "E" And PARAMOUT.Substring(5118, 2).Trim = "" Then

                    'Extract the data from the parameter
                    Dim iPosition As Integer = 0
                    Dim iCounter As Integer = 1

                    Do While iCounter <= 20

                        ' Has a product been returned
                        If PARAMOUT.Substring(iPosition, 13).Trim = "" Then
                            Exit Do
                        Else
                            If packageIds.Contains(CLng(PARAMOUT.Substring(iPosition, 13).Trim()).ToString()) Then
                                Exit Do
                            Else
                                'Create a new row
                                dRow = Nothing
                                dRow = DtPackageListResults.NewRow
                                dRow("PackageID") = CLng(PARAMOUT.Substring(iPosition, 13).Trim())
                                If String.IsNullOrEmpty(packageIds) Then
                                    packageIds = dRow("PackageID")
                                Else
                                    packageIds = packageIds + "," + dRow("PackageID").ToString()
                                End If
                                dRow("PackageDescription") = PARAMOUT.Substring(iPosition + 13, 30).Trim()
                                dRow("Comment1") = PARAMOUT.Substring(iPosition + 43, 60).Trim()
                                dRow("Comment2") = PARAMOUT.Substring(iPosition + 103, 60).Trim()
                                dRow("PricingMethod") = PARAMOUT.Substring(iPosition + 163, 1).Trim()
                                dRow("Availability") = PARAMOUT.Substring(iPosition + 164, 3).Trim()
                                dRow("Price") = Utilities.ConvertStringToDecimal(PARAMOUT.Substring(iPosition + 167, 10).Trim()) / 100
                                dRow("MaxQuantity") = PARAMOUT.Substring(iPosition + 177, 3).Trim()
                                dRow("VatValue") = PARAMOUT.Substring(iPosition + 180, 9).Trim()
                                dRow("PackageCode") = (PARAMOUT.Substring(iPosition + 189, 12)).Trim()
                                dRow("AllowComments") = Utilities.convertToBool(PARAMOUT.Substring(iPosition + 201, 1))
                                dRow("MaxPercentageToSellPWS") = Utilities.ConvertStringToDecimal(PARAMOUT.Substring(iPosition + 202, 3))
                                dRow("Sold") = Utilities.ConvertStringToDecimal(PARAMOUT.Substring(iPosition + 205, 5))
                                dRow("Reserved") = Utilities.ConvertStringToDecimal(PARAMOUT.Substring(iPosition + 210, 5))
                                dRow("Cancelled") = Utilities.ConvertStringToDecimal(PARAMOUT.Substring(iPosition + 215, 5))
                                dRow("DataCaptureTemplateID") = Utilities.ConvertStringToDecimal(PARAMOUT.Substring(iPosition + 220, 13).Trim())
                                DtPackageListResults.Rows.Add(dRow)

                                'Increment
                                iPosition = iPosition + 250
                                iCounter = iCounter + 1
                            End If
                        End If
                    Loop
                    'Extract the component list to table
                    AddComponentListToTable(DtComponentListResults)

                    'Extract the lead source information to table
                    AddLeadSourceListToTable(DtLeadSourceListResults)

                    'Extract the footer information
                    sNextPkgID = PARAMOUT.Substring(5094, 12)
                    sNextProductID = PARAMOUT.Substring(5046, 6).Trim()
                    sLastRecord = PARAMOUT.Substring(5113, 3)
                    If PARAMOUT.Substring(5112, 1).Trim = "Y" Then
                        bMoreRecords = True
                    Else
                        bMoreRecords = False
                    End If

                Else
                    bMoreRecords = False
                End If

            Loop

        Catch ex As Exception
            ResultDataSet = Nothing
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "DBProduct-WS670R"
                .HasError = True
            End With
        End Try
        Return err
    End Function

    Private Function CallWS670R(ByVal sNextPkgID As String, ByVal sNextProductID As String, ByVal sLastRecord As String) As String

        'Create command object
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strProgram As String = "WS670R"
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim &
                                    "/" & strProgram & "(@PARAM1,@PARAM2,@PARM3)"
        Dim parmIO, parmIO2, parmIO3 As iDB2Parameter
        Dim PARAMOUT As String = String.Empty

        'Set the connection string
        cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)

        'Populate the parameter
        parmIO = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 5120)
        parmIO.Value = WS670Parm(sNextPkgID, sNextProductID, sLastRecord)
        parmIO.Direction = ParameterDirection.InputOutput

        parmIO2 = cmdSELECT.Parameters.Add(Param2, iDB2DbType.iDB2Char, 10432)
        parmIO2.Value = Utilities.FixStringLength(String.Empty, 10432)
        parmIO2.Direction = ParameterDirection.InputOutput

        parmIO3 = cmdSELECT.Parameters.Add(Param3, iDB2DbType.iDB2Char, 10000)
        parmIO3.Value = Utilities.FixStringLength(String.Empty, 10000)
        parmIO3.Direction = ParameterDirection.Output

        'Execute
        TalentCommonLog("CallWS670R", "", "Backend Request: strHEADER=" & strHEADER & ", parmIO.Value=" & parmIO.Value & ", parmIO2.Value=" & parmIO2.Value & ", parmIO3.Value=" & parmIO3.Value)

        cmdSELECT.ExecuteNonQuery()
        PARAMOUT = cmdSELECT.Parameters(Param1).Value.ToString
        PARAMOUT2 = cmdSELECT.Parameters(Param2).Value.ToString
        PARAMOUT3 = cmdSELECT.Parameters(Param3).Value.ToString

        TalentCommonLog("CallWS670R", "", "Backend Response: PARAMOUT=" & PARAMOUT)

        Return PARAMOUT

    End Function

    Private Function WS670Parm(ByVal sNextPkgID As String, ByVal sNextProductID As String, ByVal sLastRecord As String) As String
        Dim sb As New StringBuilder
        Dim IsAgent As String
        If Settings.IsAgent Then
            IsAgent = "Y"
        Else
            IsAgent = "N"
        End If

        'Construct the parameter
        sb.Append(Utilities.FixStringLength(String.Empty, 5046))
        sb.Append(Utilities.FixStringLength(sNextProductID, 6))
        sb.Append(Utilities.FixStringLength(String.Empty, 15))
        sb.Append(Utilities.FixStringLength(De.ProductGroupCode, 6))
        sb.Append(Utilities.FixStringLength(De.BusinessUnit, 20))
        sb.Append(Utilities.FixStringLength(IsAgent, 1))
        sb.Append(Utilities.FixStringLength(sNextPkgID, 12))
        sb.Append(Utilities.FixStringLength(String.Empty, 6))
        sb.Append(Utilities.FixStringLength("N", 1))
        sb.Append(Utilities.FixStringLength(sLastRecord, 3))
        sb.Append(Utilities.FixStringLength(Settings.OriginatingSourceCode, 1))
        sb.Append(Utilities.FixStringLength(String.Empty, 1))
        sb.Append(Utilities.FixStringLength(String.Empty, 2))

        Return sb.ToString()
    End Function

#End Region

#Region "SQL2005"

    Protected Overrides Function AccessDataBaseSQL2005() As ErrorObj
        Dim err As New ErrorObj

        Select Case _settings.ModuleName
            Case Is = ProductNavigationLoad
                err = AccessDatabaseSQL2005_ProductNavigationLoad()
            Case Is = ProductStockLoad
                err = AccessDatabaseSQL2005_ProductStockLoad()
            Case Is = ProductLoad
                err = AccessDatabaseSQL2005_ProductLoad()
            Case Is = ProductOptionsLoad
                err = AccessDatabaseSQL2005_ProductOptionsLoad()
            Case Is = ProductRelationsLoad
                err = AccessDatabaseSQL2005_ProductRelationsLoad()
            Case Is = ProductPriceLoad
                err = AccessDatabaseSQL2005_ProductPriceLoad()
            Case Else

        End Select

        Return err
    End Function

    Private Function generateSqlStr(ByVal SqlAction As String,
                                    ByVal Tablename As String,
                                    ByVal columnNames As ArrayList,
                                    ByVal keyColumnNames As ArrayList) As String
        Dim mySqlStr As String = ""

        Select Case UCase(SqlAction)
            Case "INSERT"
                mySqlStr = " INSERT INTO " & Tablename & "("
                If columnNames.Count > 0 Then
                    Dim i As Integer = 0
                    For Each col As String In columnNames
                        If i = 0 Then
                            mySqlStr += " "
                        Else
                            mySqlStr += " , "
                        End If
                        mySqlStr += " " & col
                        i += 1
                    Next
                End If
                mySqlStr += " ) VALUES ("
                If columnNames.Count > 0 Then
                    Dim i As Integer = 0
                    For Each col As String In columnNames
                        If i = 0 Then
                            mySqlStr += " "
                        Else
                            mySqlStr += " , "
                        End If
                        mySqlStr += " @" & col
                        i += 1
                    Next
                End If
                mySqlStr += " ) "

            Case "UPDATE"

                mySqlStr = " UPDATE " & Tablename
                If columnNames.Count > 0 Then
                    Dim i As Integer = 0
                    For Each col As String In columnNames
                        If i = 0 Then
                            mySqlStr += " SET "
                        Else
                            mySqlStr += " , "
                        End If
                        mySqlStr += " " & col & " = @" & col
                        i += 1
                    Next
                End If
                If keyColumnNames.Count > 0 Then
                    Dim i As Integer = 0
                    For Each col As String In keyColumnNames
                        If i = 0 Then
                            mySqlStr += " WHERE "
                        Else
                            mySqlStr += " AND "
                        End If
                        mySqlStr += " " & col & " = @" & col
                        i += 1
                    Next
                End If

            Case "DELETE"

                mySqlStr = " DELETE FROM " & Tablename

                If keyColumnNames.Count > 0 Then
                    Dim i As Integer = 0
                    For Each col As String In keyColumnNames
                        If i = 0 Then
                            mySqlStr += " WHERE "
                        Else
                            mySqlStr += " AND "
                        End If
                        mySqlStr += " " & col & " = @" & col
                        i += 1
                    Next
                End If

            Case "SELECT"

                mySqlStr = " SELECT * FROM " & Tablename

                If keyColumnNames.Count > 0 Then
                    Dim i As Integer = 0
                    For Each col As String In keyColumnNames
                        If i = 0 Then
                            mySqlStr += " WHERE "
                        Else
                            mySqlStr += " AND "
                        End If
                        mySqlStr += " " & col & " = @" & col
                        i += 1
                    Next
                End If

        End Select

        Return mySqlStr
    End Function


    Private Function AccessDatabaseSQL2005_ProductOptionsLoad() As ErrorObj
        Dim err As New ErrorObj
        Dim cmd As SqlCommand

        If Not Depo Is Nothing Then
            cmd = New SqlCommand("", conSql2005)
            ProcessProductOptionTypes(cmd)
            ProcessProductOptionDefinitions(cmd)
            ProcessProductOptionsAndDefaults(cmd)
        End If

        Return err
    End Function


    Private Sub ProcessProductOptionTypes(ByVal cmd As SqlCommand)
        Try
            Dim typesCols As New ArrayList
            Dim typesKeys As New ArrayList
            Dim typesLangCols As New ArrayList
            Dim typesLangKeys As New ArrayList

            With typesCols
                .Add("OPTION_TYPE")
                .Add("DESCRIPTION")
            End With
            With typesKeys
                .Add("OPTION_TYPE")
            End With

            With typesLangCols
                .Add("BUSINESS_UNIT")
                .Add("PARTNER")
                .Add("OPTION_TYPE")
                .Add("LANGUAGE_CODE")
                .Add("DISPLAY_NAME")
                .Add("LABEL_TEXT")
            End With
            With typesLangKeys
                .Add("BUSINESS_UNIT")
                .Add("PARTNER")
                .Add("OPTION_TYPE")
                .Add("LANGUAGE_CODE")
            End With

            Dim typesSelect As String = generateSqlStr("SELECT", "tbl_product_option_types", typesCols, typesKeys)
            Dim typesLangSelect As String = generateSqlStr("SELECT", "tbl_product_option_types_lang", typesLangCols, typesLangKeys)
            Dim typesInsert As String = generateSqlStr("INSERT", "tbl_product_option_types", typesCols, typesKeys)
            Dim typesLangInsert As String = generateSqlStr("INSERT", "tbl_product_option_types_lang", typesLangCols, typesLangKeys)
            Dim typesUpdate As String = generateSqlStr("UPDATE", "tbl_product_option_types", typesCols, typesKeys)
            Dim typesLangUpdate As String = generateSqlStr("UPDATE", "tbl_product_option_types_lang", typesLangCols, typesLangKeys)
            Dim typesDelete As String = generateSqlStr("DELETE", "tbl_product_option_types", typesCols, typesKeys)
            Dim typesLangDelete As String = generateSqlStr("DELETE", "tbl_product_option_types_lang", typesLangCols, typesLangKeys)
            Dim typesInsertWork As String = generateSqlStr("INSERT", "tbl_product_option_types_work", typesCols, typesKeys)
            Dim typesLangInsertWork As String = generateSqlStr("INSERT", "tbl_product_option_types_lang_work", typesLangCols, typesLangKeys)

            Const typesDeleteWork As String = " DELETE FROM [tbl_product_option_types_work] "
            Const typesLangDeleteWork As String = " DELETE FROM [tbl_product_option_types_lang_work] "

            '-----------------------------------
            ' PRODUCT OPTION TYPES
            '-----------------------------------
            cmd.CommandText = typesDeleteWork
            cmd.ExecuteNonQuery()
            cmd.CommandText = typesLangDeleteWork
            cmd.ExecuteNonQuery()

            Select Case UCase(Depo.OptionsTypeLoadMode)

                Case "REPLACE"

                    For Each item As DEProductOptionType In Depo.OptionTypesDEs

                        'Add the types records
                        '-------------------------
                        cmd.CommandText = typesInsertWork
                        With cmd.Parameters
                            .Clear()
                            .Add("OPTION_TYPE", SqlDbType.NVarChar).Value = item.OptionType
                            .Add("DESCRIPTION", SqlDbType.NVarChar).Value = item.Description
                        End With
                        cmd.ExecuteNonQuery()

                        For Each langItem As DEProductOptionTypeLang In item.LanguageSpecificDEs
                            'Add the types lang records
                            '---------------------------
                            cmd.CommandText = typesLangInsertWork
                            With cmd.Parameters
                                .Clear()
                                .Add("BUSINESS_UNIT", SqlDbType.NVarChar).Value = langItem.BusinessUnit
                                .Add("PARTNER", SqlDbType.NVarChar).Value = langItem.Partner
                                .Add("OPTION_TYPE", SqlDbType.NVarChar).Value = item.OptionType
                                .Add("LANGUAGE_CODE", SqlDbType.NVarChar).Value = langItem.LanguageCode
                                .Add("DISPLAY_NAME", SqlDbType.NVarChar).Value = langItem.DisplayName
                                .Add("LABEL_TEXT", SqlDbType.NVarChar).Value = langItem.LabelText
                            End With
                            cmd.ExecuteNonQuery()
                        Next

                    Next

                    DataTransfer.SQLConnectionString = Me.Settings.BackOfficeConnectionString
                    DataTransfer.DoUpdateTable_EBPRDO_types()
                    DataTransfer.DoUpdateTable_EBPRDO_types_lang()

                Case "UPDATE"

                    For Each item As DEProductOptionType In Depo.OptionTypesDEs

                        Select Case UCase(item.Mode)
                            Case "ADD"

                                Dim dtrReader As SqlDataReader
                                Dim dtrReader2 As SqlDataReader
                                Dim cmdSelect As SqlCommand
                                cmdSelect = New SqlCommand(typesSelect, conSql2005)
                                With cmdSelect.Parameters
                                    .Clear()
                                    .Add(New SqlParameter("@OPTION_TYPE", SqlDbType.NVarChar)).Value = item.OptionType
                                End With
                                dtrReader = cmdSelect.ExecuteReader
                                If Not dtrReader.HasRows Then
                                    dtrReader.Close()

                                    'Add the types records
                                    '-------------------------
                                    cmd.CommandText = typesInsert
                                    With cmd.Parameters
                                        .Clear()
                                        .Add("OPTION_TYPE", SqlDbType.NVarChar).Value = item.OptionType
                                        .Add("DESCRIPTION", SqlDbType.NVarChar).Value = item.Description
                                    End With
                                    cmd.ExecuteNonQuery()

                                End If
                                If Not dtrReader.IsClosed Then
                                    dtrReader.Close()
                                End If

                                For Each langItem As DEProductOptionTypeLang In item.LanguageSpecificDEs

                                    cmdSelect = New SqlCommand(typesLangSelect, conSql2005)
                                    With cmdSelect.Parameters
                                        .Clear()
                                        .Add("@BUSINESS_UNIT", SqlDbType.NVarChar).Value = langItem.BusinessUnit
                                        .Add("@PARTNER", SqlDbType.NVarChar).Value = langItem.Partner
                                        .Add("@OPTION_TYPE", SqlDbType.NVarChar).Value = item.OptionType
                                        .Add("@LANGUAGE_CODE", SqlDbType.NVarChar).Value = langItem.LanguageCode
                                    End With
                                    dtrReader2 = cmdSelect.ExecuteReader
                                    If Not dtrReader2.HasRows Then
                                        dtrReader2.Close()

                                        'Add the types lang records
                                        '---------------------------
                                        cmd.CommandText = typesLangInsert
                                        With cmd.Parameters
                                            .Clear()
                                            .Add("BUSINESS_UNIT", SqlDbType.NVarChar).Value = langItem.BusinessUnit
                                            .Add("PARTNER", SqlDbType.NVarChar).Value = langItem.Partner
                                            .Add("OPTION_TYPE", SqlDbType.NVarChar).Value = item.OptionType
                                            .Add("LANGUAGE_CODE", SqlDbType.NVarChar).Value = langItem.LanguageCode
                                            .Add("DISPLAY_NAME", SqlDbType.NVarChar).Value = langItem.DisplayName
                                            .Add("LABEL_TEXT", SqlDbType.NVarChar).Value = langItem.LabelText
                                        End With
                                        cmd.ExecuteNonQuery()

                                    End If
                                    If Not dtrReader2.IsClosed Then
                                        dtrReader2.Close()
                                    End If

                                Next


                            Case "UPDATE"

                                'Update the types records
                                '-------------------------
                                cmd.CommandText = typesUpdate
                                With cmd.Parameters
                                    .Clear()
                                    .Add("OPTION_TYPE", SqlDbType.NVarChar).Value = item.OptionType
                                    .Add("DESCRIPTION", SqlDbType.NVarChar).Value = item.Description
                                End With
                                cmd.ExecuteNonQuery()

                                For Each langItem As DEProductOptionTypeLang In item.LanguageSpecificDEs
                                    'Update the types lang records
                                    '---------------------------
                                    cmd.CommandText = typesLangUpdate
                                    With cmd.Parameters
                                        .Clear()
                                        .Add("BUSINESS_UNIT", SqlDbType.NVarChar).Value = langItem.BusinessUnit
                                        .Add("PARTNER", SqlDbType.NVarChar).Value = langItem.Partner
                                        .Add("OPTION_TYPE", SqlDbType.NVarChar).Value = item.OptionType
                                        .Add("LANGUAGE_CODE", SqlDbType.NVarChar).Value = langItem.LanguageCode
                                        .Add("DISPLAY_NAME", SqlDbType.NVarChar).Value = langItem.DisplayName
                                        .Add("LABEL_TEXT", SqlDbType.NVarChar).Value = langItem.LabelText
                                    End With
                                    cmd.ExecuteNonQuery()
                                Next


                            Case "DELETE"

                                'Delete the types records
                                '---------------------------
                                cmd.CommandText = typesDelete
                                With cmd.Parameters
                                    .Clear()
                                    .Add("OPTION_TYPE", SqlDbType.NVarChar).Value = item.OptionType
                                End With
                                cmd.ExecuteNonQuery()

                                For Each langItem As DEProductOptionTypeLang In item.LanguageSpecificDEs
                                    'Delete the types lang records
                                    '---------------------------
                                    cmd.CommandText = typesLangDelete
                                    With cmd.Parameters
                                        .Clear()
                                        .Add("BUSINESS_UNIT", SqlDbType.NVarChar).Value = langItem.BusinessUnit
                                        .Add("PARTNER", SqlDbType.NVarChar).Value = langItem.Partner
                                        .Add("OPTION_TYPE", SqlDbType.NVarChar).Value = item.OptionType
                                        .Add("LANGUAGE_CODE", SqlDbType.NVarChar).Value = langItem.LanguageCode
                                    End With
                                    cmd.ExecuteNonQuery()
                                Next


                        End Select
                    Next
            End Select
        Catch ex As Exception

        End Try

    End Sub

    Private Sub ProcessProductOptionDefinitions(ByVal cmd As SqlCommand)

        Try
            Dim definitionsCols As New ArrayList
            Dim definitionsKeys As New ArrayList
            Dim definitionsLangCols As New ArrayList
            Dim definitionsLangKeys As New ArrayList

            With definitionsCols
                .Add("OPTION_CODE")
                .Add("DESCRIPTION")
            End With
            With definitionsKeys
                .Add("OPTION_CODE")
            End With

            With definitionsLangCols
                .Add("BUSINESS_UNIT")
                .Add("PARTNER")
                .Add("OPTION_CODE")
                .Add("LANGUAGE_CODE")
                .Add("DISPLAY_NAME")
            End With
            With definitionsLangKeys
                .Add("BUSINESS_UNIT")
                .Add("PARTNER")
                .Add("OPTION_CODE")
                .Add("LANGUAGE_CODE")
            End With

            Dim definitionsSelect As String = generateSqlStr("SELECT", "tbl_product_option_definitions", definitionsCols, definitionsKeys)
            Dim definitionsLangSelect As String = generateSqlStr("SELECT", "tbl_product_option_definitions_lang", definitionsLangCols, definitionsLangKeys)
            Dim definitionsInsert As String = generateSqlStr("INSERT", "tbl_product_option_definitions", definitionsCols, definitionsKeys)
            Dim definitionsLangInsert As String = generateSqlStr("INSERT", "tbl_product_option_definitions_lang", definitionsLangCols, definitionsLangKeys)
            Dim definitionsUpdate As String = generateSqlStr("UPDATE", "tbl_product_option_definitions", definitionsCols, definitionsKeys)
            Dim definitionsLangUpdate As String = generateSqlStr("UPDATE", "tbl_product_option_definitions_lang", definitionsLangCols, definitionsLangKeys)
            Dim definitionsDelete As String = generateSqlStr("DELETE", "tbl_product_option_definitions", definitionsCols, definitionsKeys)
            Dim definitionsLangDelete As String = generateSqlStr("DELETE", "tbl_product_option_definitions_lang", definitionsLangCols, definitionsLangKeys)
            Dim definitionsInsertWork As String = generateSqlStr("INSERT", "tbl_product_option_definitions_work", definitionsCols, definitionsKeys)
            Dim definitionsLangInsertWork As String = generateSqlStr("INSERT", "tbl_product_option_definitions_lang_work", definitionsLangCols, definitionsLangKeys)
            Const definitionsDeleteWork As String = " DELETE FROM [tbl_product_option_definitions_work] "
            Const definitionsLangDeleteWork As String = " DELETE FROM [tbl_product_option_definitions_lang_work] "

            '----------------------------
            ' PRODUCT OPTION DEFINITIONS
            '----------------------------
            cmd.CommandText = definitionsDeleteWork
            cmd.ExecuteNonQuery()
            cmd.CommandText = definitionsLangDeleteWork
            cmd.ExecuteNonQuery()

            Select Case UCase(Depo.OptionsDefinitionsLoadMode)

                Case "REPLACE"

                    For Each item As DEProductOptionDefinition In Depo.OptionDefinitionDEs

                        'Add the types records
                        '-------------------------
                        cmd.CommandText = definitionsInsertWork
                        With cmd.Parameters
                            .Clear()
                            .Add("OPTION_CODE", SqlDbType.NVarChar).Value = item.OptionCode
                            .Add("DESCRIPTION", SqlDbType.NVarChar).Value = item.Description
                        End With
                        cmd.ExecuteNonQuery()

                        For Each langItem As DEProductOptionDefinitionLang In item.LanguageSpecificDEs
                            'Add the types lang records
                            '---------------------------
                            cmd.CommandText = definitionsLangInsertWork
                            With cmd.Parameters
                                .Clear()
                                .Add("BUSINESS_UNIT", SqlDbType.NVarChar).Value = langItem.BusinessUnit
                                .Add("PARTNER", SqlDbType.NVarChar).Value = langItem.Partner
                                .Add("OPTION_CODE", SqlDbType.NVarChar).Value = item.OptionCode
                                .Add("LANGUAGE_CODE", SqlDbType.NVarChar).Value = langItem.LanguageCode
                                .Add("DISPLAY_NAME", SqlDbType.NVarChar).Value = langItem.DisplayName
                            End With
                            cmd.ExecuteNonQuery()
                        Next

                    Next
                    DataTransfer.SQLConnectionString = Me.Settings.BackOfficeConnectionString
                    DataTransfer.DoUpdateTable_EBPRDO_definitions()
                    DataTransfer.DoUpdateTable_EBPRDO_definitions_lang()

                Case "UPDATE"

                    For Each item As DEProductOptionDefinition In Depo.OptionDefinitionDEs

                        Select Case UCase(item.Mode)
                            Case "ADD"

                                Dim dtrReader As SqlDataReader
                                Dim dtrReader2 As SqlDataReader
                                Dim cmdSelect As SqlCommand
                                cmdSelect = New SqlCommand(definitionsSelect, conSql2005)
                                With cmdSelect.Parameters
                                    .Clear()
                                    .Add("@OPTION_CODE", SqlDbType.NVarChar).Value = item.OptionCode
                                End With
                                dtrReader = cmdSelect.ExecuteReader
                                If Not dtrReader.HasRows Then
                                    dtrReader.Close()

                                    'Add the definition records
                                    '---------------------------
                                    cmd.CommandText = definitionsInsert
                                    With cmd.Parameters
                                        .Clear()
                                        .Add("OPTION_CODE", SqlDbType.NVarChar).Value = item.OptionCode
                                        .Add("DESCRIPTION", SqlDbType.NVarChar).Value = item.Description
                                    End With
                                    cmd.ExecuteNonQuery()

                                End If
                                If Not dtrReader.IsClosed Then
                                    dtrReader.Close()
                                End If

                                For Each langItem As DEProductOptionDefinitionLang In item.LanguageSpecificDEs

                                    cmdSelect = New SqlCommand(definitionsLangSelect, conSql2005)
                                    With cmdSelect.Parameters
                                        .Clear()
                                        .Add("@BUSINESS_UNIT", SqlDbType.NVarChar).Value = langItem.BusinessUnit
                                        .Add("@PARTNER", SqlDbType.NVarChar).Value = langItem.Partner
                                        .Add("@OPTION_CODE", SqlDbType.NVarChar).Value = item.OptionCode
                                        .Add("@LANGUAGE_CODE", SqlDbType.NVarChar).Value = langItem.LanguageCode
                                    End With
                                    dtrReader2 = cmdSelect.ExecuteReader
                                    If Not dtrReader2.HasRows Then
                                        dtrReader2.Close()

                                        'Add the definition lang records
                                        '--------------------------------
                                        cmd.CommandText = definitionsLangInsert
                                        With cmd.Parameters
                                            .Clear()
                                            .Add("BUSINESS_UNIT", SqlDbType.NVarChar).Value = langItem.BusinessUnit
                                            .Add("PARTNER", SqlDbType.NVarChar).Value = langItem.Partner
                                            .Add("OPTION_CODE", SqlDbType.NVarChar).Value = item.OptionCode
                                            .Add("LANGUAGE_CODE", SqlDbType.NVarChar).Value = langItem.LanguageCode
                                            .Add("DISPLAY_NAME", SqlDbType.NVarChar).Value = langItem.DisplayName
                                        End With
                                        cmd.ExecuteNonQuery()

                                    End If
                                    If Not dtrReader2.IsClosed Then
                                        dtrReader2.Close()
                                    End If

                                Next


                            Case "UPDATE"

                                'Update the definition records
                                '---------------------------
                                cmd.CommandText = definitionsUpdate
                                With cmd.Parameters
                                    .Clear()
                                    .Add("OPTION_CODE", SqlDbType.NVarChar).Value = item.OptionCode
                                    .Add("DESCRIPTION", SqlDbType.NVarChar).Value = item.Description
                                End With
                                cmd.ExecuteNonQuery()

                                For Each langItem As DEProductOptionDefinitionLang In item.LanguageSpecificDEs
                                    'Update the definition lang records
                                    '--------------------------------
                                    cmd.CommandText = definitionsLangUpdate
                                    With cmd.Parameters
                                        .Clear()
                                        .Add("BUSINESS_UNIT", SqlDbType.NVarChar).Value = langItem.BusinessUnit
                                        .Add("PARTNER", SqlDbType.NVarChar).Value = langItem.Partner
                                        .Add("OPTION_CODE", SqlDbType.NVarChar).Value = item.OptionCode
                                        .Add("LANGUAGE_CODE", SqlDbType.NVarChar).Value = langItem.LanguageCode
                                        .Add("DISPLAY_NAME", SqlDbType.NVarChar).Value = langItem.DisplayName
                                    End With
                                    cmd.ExecuteNonQuery()
                                Next


                            Case "DELETE"

                                'Delete the definition records
                                '---------------------------
                                cmd.CommandText = definitionsDelete
                                With cmd.Parameters
                                    .Clear()
                                    .Add("OPTION_CODE", SqlDbType.NVarChar).Value = item.OptionCode
                                    .Add("DESCRIPTION", SqlDbType.NVarChar).Value = item.Description
                                End With
                                cmd.ExecuteNonQuery()

                                For Each langItem As DEProductOptionDefinitionLang In item.LanguageSpecificDEs
                                    'Delete the definition lang records
                                    '--------------------------------
                                    cmd.CommandText = definitionsLangDelete
                                    With cmd.Parameters
                                        .Clear()
                                        .Add("BUSINESS_UNIT", SqlDbType.NVarChar).Value = langItem.BusinessUnit
                                        .Add("PARTNER", SqlDbType.NVarChar).Value = langItem.Partner
                                        .Add("OPTION_CODE", SqlDbType.NVarChar).Value = item.OptionCode
                                        .Add("LANGUAGE_CODE", SqlDbType.NVarChar).Value = langItem.LanguageCode
                                    End With
                                    cmd.ExecuteNonQuery()
                                Next


                        End Select
                    Next
            End Select
        Catch ex As Exception

        End Try

    End Sub

    Private Sub ProcessProductOptionsAndDefaults(ByVal cmd As SqlCommand)

        Try
            Dim defaultsCols As New ArrayList
            Dim defaultsKeys As New ArrayList
            Dim optionsCols As New ArrayList
            Dim optionsKeys As New ArrayList

            With defaultsCols
                .Add("BUSINESS_UNIT")
                .Add("PARTNER")
                .Add("MASTER_PRODUCT")
                .Add("OPTION_TYPE")
                .Add("MATCH_ACTION")
                .Add("IS_DEFAULT")
                .Add("APPEND_SEQUENCE")
                .Add("DISPLAY_SEQUENCE")
                .Add("DISPLAY_TYPE")
            End With
            With defaultsKeys
                .Add("BUSINESS_UNIT")
                .Add("PARTNER")
                .Add("MASTER_PRODUCT")
                .Add("OPTION_TYPE")
                .Add("MATCH_ACTION")
                .Add("APPEND_SEQUENCE")
                .Add("DISPLAY_SEQUENCE")
            End With

            With optionsCols
                .Add("BUSINESS_UNIT")
                .Add("PARTNER")
                .Add("MASTER_PRODUCT")
                .Add("OPTION_TYPE")
                .Add("OPTION_CODE")
                .Add("PRODUCT_CODE")
                .Add("DISPLAY_ORDER")
            End With
            With optionsKeys
                .Add("BUSINESS_UNIT")
                .Add("PARTNER")
                .Add("MASTER_PRODUCT")
                .Add("PRODUCT_CODE")
            End With

            Dim defaultsSelect As String = generateSqlStr("SELECT", "tbl_product_option_defaults", defaultsCols, defaultsKeys)
            Dim optionsSelect As String = generateSqlStr("SELECT", "tbl_product_options", optionsCols, optionsKeys)
            Dim defaultsInsert As String = generateSqlStr("INSERT", "tbl_product_option_defaults", defaultsCols, defaultsKeys)
            Dim optionsInsert As String = generateSqlStr("INSERT", "tbl_product_options", optionsCols, optionsKeys)
            Dim defaultsUpdate As String = generateSqlStr("UPDATE", "tbl_product_option_defaults", defaultsCols, defaultsKeys)
            Dim optionsUpdate As String = generateSqlStr("UPDATE", "tbl_product_options", optionsCols, optionsKeys)
            Dim defaultsDelete As String = generateSqlStr("DELETE", "tbl_product_option_defaults", defaultsCols, defaultsKeys)
            Dim optionsDelete As String = generateSqlStr("DELETE", "tbl_product_options", optionsCols, optionsKeys)
            Dim defaultsInsertWork As String = generateSqlStr("INSERT", "tbl_product_option_defaults_work", defaultsCols, defaultsKeys)
            Dim optionsInsertWork As String = generateSqlStr("INSERT", "tbl_product_options_work", optionsCols, optionsKeys)
            Const defaultsDeleteWork As String = " DELETE FROM [tbl_product_option_defaults_work] "
            Const optionsDeleteWork As String = " DELETE FROM [tbl_product_options_work] "

            '------------------------------------
            ' PRODUCT OPTIONS AND OPTION DEFAULTS
            '------------------------------------
            cmd.CommandText = defaultsDeleteWork
            cmd.ExecuteNonQuery()
            cmd.CommandText = optionsDeleteWork
            cmd.ExecuteNonQuery()

            For Each defAndOpt As DEProductOptionDefaultsAndOptions In Depo.OptionDefaultsAndOptionsDEs
                'For Each item As DEProductOptionDefault In defAndOpt.OptionDefaultDEs

                Select Case UCase(defAndOpt.Mode)
                    Case "REPLACE"

                        For Each item As DEProductOptionDefault In defAndOpt.OptionDefaultDEs
                            For Each opt As DEProductOption In item.ProductOptions

                                'Add the option records
                                '---------------------------
                                cmd.CommandText = optionsInsertWork
                                With cmd.Parameters
                                    .Clear()
                                    .Add("BUSINESS_UNIT", SqlDbType.NVarChar).Value = defAndOpt.BusinessUnit
                                    .Add("PARTNER", SqlDbType.NVarChar).Value = defAndOpt.Partner
                                    .Add("MASTER_PRODUCT", SqlDbType.NVarChar).Value = item.MasterProduct
                                    .Add("OPTION_TYPE", SqlDbType.NVarChar).Value = item.OptionType
                                    .Add("OPTION_CODE", SqlDbType.NVarChar).Value = opt.OptionCode
                                    .Add("PRODUCT_CODE", SqlDbType.NVarChar).Value = opt.ProductCode
                                    .Add("DISPLAY_ORDER", SqlDbType.Int).Value = opt.DisplayOrder
                                End With
                                cmd.ExecuteNonQuery()
                            Next

                            'Add the option default records
                            '--------------------------------
                            cmd.CommandText = defaultsInsertWork
                            With cmd.Parameters
                                .Clear()
                                .Add("BUSINESS_UNIT", SqlDbType.NVarChar).Value = defAndOpt.BusinessUnit
                                .Add("PARTNER", SqlDbType.NVarChar).Value = defAndOpt.Partner
                                .Add("MASTER_PRODUCT", SqlDbType.NVarChar).Value = item.MasterProduct
                                .Add("OPTION_TYPE", SqlDbType.NVarChar).Value = item.OptionType
                                .Add("MATCH_ACTION", SqlDbType.NVarChar).Value = item.MatchAction
                                .Add("IS_DEFAULT", SqlDbType.Bit).Value = False
                                .Add("APPEND_SEQUENCE", SqlDbType.Int).Value = item.AppendSequence
                                .Add("DISPLAY_SEQUENCE", SqlDbType.Int).Value = item.DisplaySequence
                                .Add("DISPLAY_TYPE", SqlDbType.NVarChar).Value = item.DisplayType
                            End With
                            cmd.ExecuteNonQuery()
                        Next

                        DataTransfer.SQLConnectionString = Me.Settings.BackOfficeConnectionString
                        DataTransfer.DoUpdateTable_EBPRDO()
                        DataTransfer.DoUpdateTable_EBPRDD(True)

                    Case "UPDATE"

                        For Each item As DEProductOptionDefault In defAndOpt.OptionDefaultDEs
                            Select Case UCase(item.Action)
                                Case "ADD"
                                    For Each opt As DEProductOption In item.ProductOptions

                                        Dim dtrReader As SqlDataReader
                                        Dim cmdSelect As SqlCommand
                                        cmdSelect = New SqlCommand(optionsSelect, conSql2005)
                                        With cmdSelect.Parameters
                                            .Clear()
                                            .Add("@BUSINESS_UNIT", SqlDbType.NVarChar).Value = defAndOpt.BusinessUnit
                                            .Add("@PARTNER", SqlDbType.NVarChar).Value = defAndOpt.Partner
                                            .Add("@MASTER_PRODUCT", SqlDbType.NVarChar).Value = item.MasterProduct
                                            .Add("@OPTION_TYPE", SqlDbType.NVarChar).Value = item.OptionType
                                            .Add("@OPTION_CODE", SqlDbType.NVarChar).Value = opt.OptionCode
                                            .Add("@PRODUCT_CODE", SqlDbType.NVarChar).Value = opt.ProductCode
                                        End With
                                        dtrReader = cmdSelect.ExecuteReader
                                        If Not dtrReader.HasRows Then
                                            dtrReader.Close()

                                            'Add the option records
                                            '---------------------------
                                            cmd.CommandText = optionsInsert
                                            With cmd.Parameters
                                                .Clear()
                                                .Add("BUSINESS_UNIT", SqlDbType.NVarChar).Value = defAndOpt.BusinessUnit
                                                .Add("PARTNER", SqlDbType.NVarChar).Value = defAndOpt.Partner
                                                .Add("MASTER_PRODUCT", SqlDbType.NVarChar).Value = item.MasterProduct
                                                .Add("OPTION_TYPE", SqlDbType.NVarChar).Value = item.OptionType
                                                .Add("OPTION_CODE", SqlDbType.NVarChar).Value = opt.OptionCode
                                                .Add("PRODUCT_CODE", SqlDbType.NVarChar).Value = opt.ProductCode
                                                .Add("DISPLAY_ORDER", SqlDbType.Int).Value = opt.DisplayOrder
                                            End With
                                            cmd.ExecuteNonQuery()

                                        End If
                                        If Not dtrReader.IsClosed Then
                                            dtrReader.Close()
                                        End If

                                    Next

                                    Dim dtrReader2 As SqlDataReader
                                    Dim cmdSelect2 As SqlCommand
                                    cmdSelect2 = New SqlCommand(defaultsSelect, conSql2005)
                                    With cmdSelect2.Parameters
                                        .Clear()
                                        .Add("@BUSINESS_UNIT", SqlDbType.NVarChar).Value = defAndOpt.BusinessUnit
                                        .Add("@PARTNER", SqlDbType.NVarChar).Value = defAndOpt.Partner
                                        .Add("@MASTER_PRODUCT", SqlDbType.NVarChar).Value = item.MasterProduct
                                        .Add("@OPTION_TYPE", SqlDbType.NVarChar).Value = item.OptionType
                                        .Add("@MATCH_ACTION", SqlDbType.NVarChar).Value = item.MatchAction
                                        .Add("@APPEND_SEQUENCE", SqlDbType.Int).Value = item.AppendSequence
                                        .Add("@DISPLAY_SEQUENCE", SqlDbType.Int).Value = item.DisplaySequence
                                    End With
                                    dtrReader2 = cmdSelect2.ExecuteReader
                                    If Not dtrReader2.HasRows Then
                                        dtrReader2.Close()

                                        'Add the option defaults records
                                        '--------------------------------
                                        cmd.CommandText = defaultsInsert
                                        With cmd.Parameters
                                            .Clear()
                                            .Add("BUSINESS_UNIT", SqlDbType.NVarChar).Value = defAndOpt.BusinessUnit
                                            .Add("PARTNER", SqlDbType.NVarChar).Value = defAndOpt.Partner
                                            .Add("MASTER_PRODUCT", SqlDbType.NVarChar).Value = item.MasterProduct
                                            .Add("OPTION_TYPE", SqlDbType.NVarChar).Value = item.OptionType
                                            .Add("MATCH_ACTION", SqlDbType.NVarChar).Value = item.MatchAction
                                            .Add("IS_DEFAULT", SqlDbType.Bit).Value = False
                                            .Add("APPEND_SEQUENCE", SqlDbType.Int).Value = item.AppendSequence
                                            .Add("DISPLAY_SEQUENCE", SqlDbType.Int).Value = item.DisplaySequence
                                            .Add("DISPLAY_TYPE", SqlDbType.NVarChar).Value = item.DisplayType
                                        End With
                                        cmd.ExecuteNonQuery()

                                    End If
                                    If Not dtrReader2.IsClosed() Then
                                        dtrReader2.Close()
                                    End If

                                Case "UPDATE"
                                    For Each opt As DEProductOption In item.ProductOptions

                                        'Update the option records
                                        '---------------------------
                                        cmd.CommandText = optionsUpdate
                                        With cmd.Parameters
                                            .Clear()
                                            .Add("BUSINESS_UNIT", SqlDbType.NVarChar).Value = defAndOpt.BusinessUnit
                                            .Add("PARTNER", SqlDbType.NVarChar).Value = defAndOpt.Partner
                                            .Add("MASTER_PRODUCT", SqlDbType.NVarChar).Value = item.MasterProduct
                                            .Add("OPTION_TYPE", SqlDbType.NVarChar).Value = item.OptionType
                                            .Add("OPTION_CODE", SqlDbType.NVarChar).Value = opt.OptionCode
                                            .Add("PRODUCT_CODE", SqlDbType.NVarChar).Value = opt.ProductCode
                                            .Add("DISPLAY_ORDER", SqlDbType.Int).Value = opt.DisplayOrder
                                        End With
                                        cmd.ExecuteNonQuery()
                                    Next

                                    'Update the option defaults records
                                    '--------------------------------
                                    cmd.CommandText = defaultsUpdate
                                    With cmd.Parameters
                                        .Clear()
                                        .Add("BUSINESS_UNIT", SqlDbType.NVarChar).Value = defAndOpt.BusinessUnit
                                        .Add("PARTNER", SqlDbType.NVarChar).Value = defAndOpt.Partner
                                        .Add("MASTER_PRODUCT", SqlDbType.NVarChar).Value = item.MasterProduct
                                        .Add("OPTION_TYPE", SqlDbType.NVarChar).Value = item.OptionType
                                        .Add("MATCH_ACTION", SqlDbType.NVarChar).Value = item.MatchAction
                                        .Add("IS_DEFAULT", SqlDbType.Bit).Value = False
                                        .Add("APPEND_SEQUENCE", SqlDbType.Int).Value = item.AppendSequence
                                        .Add("DISPLAY_SEQUENCE", SqlDbType.Int).Value = item.DisplaySequence
                                        .Add("DISPLAY_TYPE", SqlDbType.NVarChar).Value = item.DisplayType
                                    End With
                                    cmd.ExecuteNonQuery()


                                Case "DELETE"
                                    For Each opt As DEProductOption In item.ProductOptions

                                        'delete the option records
                                        '---------------------------
                                        cmd.CommandText = optionsDelete
                                        With cmd.Parameters
                                            .Clear()
                                            .Add("BUSINESS_UNIT", SqlDbType.NVarChar).Value = defAndOpt.BusinessUnit
                                            .Add("PARTNER", SqlDbType.NVarChar).Value = defAndOpt.Partner
                                            .Add("MASTER_PRODUCT", SqlDbType.NVarChar).Value = item.MasterProduct
                                            .Add("OPTION_TYPE", SqlDbType.NVarChar).Value = item.OptionType
                                            .Add("OPTION_CODE", SqlDbType.NVarChar).Value = opt.OptionCode
                                            .Add("PRODUCT_CODE", SqlDbType.NVarChar).Value = opt.ProductCode
                                            .Add("DISPLAY_ORDER", SqlDbType.Int).Value = opt.DisplayOrder
                                        End With
                                        cmd.ExecuteNonQuery()
                                    Next

                                    'delete the option defaults records
                                    '--------------------------------
                                    cmd.CommandText = defaultsDelete
                                    With cmd.Parameters
                                        .Clear()
                                        .Add("BUSINESS_UNIT", SqlDbType.NVarChar).Value = defAndOpt.BusinessUnit
                                        .Add("PARTNER", SqlDbType.NVarChar).Value = defAndOpt.Partner
                                        .Add("MASTER_PRODUCT", SqlDbType.NVarChar).Value = item.MasterProduct
                                        .Add("OPTION_TYPE", SqlDbType.NVarChar).Value = item.OptionType
                                        .Add("MATCH_ACTION", SqlDbType.NVarChar).Value = item.MatchAction
                                        .Add("IS_DEFAULT", SqlDbType.Bit).Value = False
                                        .Add("APPEND_SEQUENCE", SqlDbType.Int).Value = item.AppendSequence
                                        .Add("DISPLAY_SEQUENCE", SqlDbType.Int).Value = item.DisplaySequence
                                        .Add("DISPLAY_TYPE", SqlDbType.NVarChar).Value = item.DisplayType
                                    End With
                                    cmd.ExecuteNonQuery()
                            End Select
                        Next
                End Select
            Next
        Catch ex As Exception

        End Try

    End Sub

    Private Function AccessDatabaseSQL2005_ProductNavigationLoad() As ErrorObj
        Dim err As New ErrorObj

        '==========================
        ' PROCESS GROUP DEFINITIONS
        '==========================
        Dim groupDetails As DEProductGroupDetails = Nothing
        Dim productGroup As DEProductGroup
        Dim groupExists As Boolean = False
        Dim groupLangExists As Boolean = False

        Const strInsert As String = "INSERT INTO tbl_group " &
                                     "       (GROUP_NAME, " &
                                     "        GROUP_DESCRIPTION_1, " &
                                     "        GROUP_DESCRIPTION_2, " &
                                     "        GROUP_HTML_1, " &
                                     "        GROUP_HTML_2, " &
                                     "        GROUP_HTML_3, " &
                                     "        GROUP_PAGE_TITLE, " &
                                     "        GROUP_META_DESCRIPTION, " &
                                     "        GROUP_META_KEYWORDS) " &
                                     "   VALUES(@GROUP_NAME, " &
                                     "        @GROUP_DESCRIPTION_1, " &
                                     "        @GROUP_DESCRIPTION_2, " &
                                     "        @GROUP_HTML_1, " &
                                     "        @GROUP_HTML_2, " &
                                     "        @GROUP_HTML_3, " &
                                     "        @GROUP_PAGE_TITLE, " &
                                     "        @GROUP_META_DESCRIPTION, " &
                                     "        @GROUP_META_KEYWORDS) "

        Const strInsertLang As String = "INSERT INTO tbl_group_lang " &
                             "       (GROUP_CODE, " &
                             "        GROUP_LANGUAGE, " &
                             "        GROUP_DESCRIPTION_1, " &
                             "        GROUP_DESCRIPTION_2, " &
                             "        GROUP_HTML_1, " &
                             "        GROUP_HTML_2, " &
                             "        GROUP_HTML_3, " &
                             "        GROUP_PAGE_TITLE, " &
                             "        GROUP_META_DESCRIPTION, " &
                             "        GROUP_META_KEYWORDS) " &
                             "   VALUES(@GROUP_CODE, " &
                             "        @GROUP_LANGUAGE, " &
                             "        @GROUP_DESCRIPTION_1, " &
                             "        @GROUP_DESCRIPTION_2, " &
                             "        @GROUP_HTML_1, " &
                             "        @GROUP_HTML_2, " &
                             "        @GROUP_HTML_3, " &
                             "        @GROUP_PAGE_TITLE, " &
                             "        @GROUP_META_DESCRIPTION, " &
                             "        @GROUP_META_KEYWORDS) "

        Const strInsertWork As String = "INSERT INTO tbl_group_work " &
                                   "       (GROUP_NAME, " &
                                   "        GROUP_DESCRIPTION_1, " &
                                   "        GROUP_DESCRIPTION_2, " &
                                   "        GROUP_HTML_1, " &
                                   "        GROUP_HTML_2, " &
                                   "        GROUP_HTML_3, " &
                                   "        GROUP_PAGE_TITLE, " &
                                   "        GROUP_META_DESCRIPTION, " &
                                   "        GROUP_META_KEYWORDS) " &
                                   "   VALUES(@GROUP_NAME, " &
                                   "        @GROUP_DESCRIPTION_1, " &
                                   "        @GROUP_DESCRIPTION_2, " &
                                   "        @GROUP_HTML_1, " &
                                   "        @GROUP_HTML_2, " &
                                   "        @GROUP_HTML_3, " &
                                   "        @GROUP_PAGE_TITLE, " &
                                   "        @GROUP_META_DESCRIPTION, " &
                                   "        @GROUP_META_KEYWORDS) "

        Const strInsertLangWork As String = "INSERT INTO tbl_group_lang_work " &
                             "       (GROUP_CODE, " &
                             "        GROUP_LANGUAGE, " &
                             "        GROUP_DESCRIPTION_1, " &
                             "        GROUP_DESCRIPTION_2, " &
                             "        GROUP_HTML_1, " &
                             "        GROUP_HTML_2, " &
                             "        GROUP_HTML_3, " &
                             "        GROUP_PAGE_TITLE, " &
                             "        GROUP_META_DESCRIPTION, " &
                             "        GROUP_META_KEYWORDS) " &
                             "   VALUES(@GROUP_CODE, " &
                             "        @GROUP_LANGUAGE, " &
                             "        @GROUP_DESCRIPTION_1, " &
                             "        @GROUP_DESCRIPTION_2, " &
                             "        @GROUP_HTML_1, " &
                             "        @GROUP_HTML_2, " &
                             "        @GROUP_HTML_3, " &
                             "        @GROUP_PAGE_TITLE, " &
                             "        @GROUP_META_DESCRIPTION, " &
                             "        @GROUP_META_KEYWORDS) "

        Const strSelect1 As String = "SELECT * FROM TBL_GROUP WHERE GROUP_NAME = @GROUP_NAME"
        Const strSelectGroupLang As String = "SELECT * FROM TBL_GROUP_LANG WHERE GROUP_CODE = @GROUP_CODE" &
                                                 " AND GROUP_LANGUAGE = @GROUP_LANGUAGE "

        Const strDelete As String = "DELETE FROM TBL_GROUP_WORK"
        Const strDeleteLang As String = "DELETE FROM TBL_GROUP_LANG_WORK"
        Const strDeleteGroupByGroup As String = "DELETE FROM TBL_GROUP WHERE GROUP_NAME = @GROUP_NAME"
        Const strDeleteGroupLangByGroup As String = "DELETE FROM TBL_GROUP_LANG WHERE GROUP_CODE = @GROUP_CODE"

        Const strUpdateGroupByGroup As String = "UPDATE TBL_GROUP SET " &
                                                " GROUP_DESCRIPTION_1 = @GROUP_DESCRIPTION_1,  " &
                                                " GROUP_DESCRIPTION_2 = @GROUP_DESCRIPTION_2,  " &
                                                " GROUP_HTML_1 = @GROUP_HTML_1 ,  " &
                                                " GROUP_HTML_2 = @GROUP_HTML_2,  " &
                                                " GROUP_HTML_3 = @GROUP_HTML_3,  " &
                                                " GROUP_PAGE_TITLE = @GROUP_PAGE_TITLE,  " &
                                                " GROUP_META_DESCRIPTION = @GROUP_META_DESCRIPTION ,  " &
                                                " GROUP_META_KEYWORDS= @GROUP_META_KEYWORDS  " &
                                                "WHERE GROUP_NAME = @GROUP_NAME"

        Const strUpdateGroupLangByGroup As String = "UPDATE TBL_GROUP_LANG SET " &
                                                " GROUP_DESCRIPTION_1 = @GROUP_DESCRIPTION_1,  " &
                                                " GROUP_DESCRIPTION_2 = @GROUP_DESCRIPTION_2,  " &
                                                " GROUP_HTML_1 = @GROUP_HTML_1 ,  " &
                                                " GROUP_HTML_2 = @GROUP_HTML_2,  " &
                                                " GROUP_HTML_3 = @GROUP_HTML_3,  " &
                                                " GROUP_PAGE_TITLE = @GROUP_PAGE_TITLE,  " &
                                                " GROUP_META_DESCRIPTION = @GROUP_META_DESCRIPTION ,  " &
                                                " GROUP_META_KEYWORDS= @GROUP_META_KEYWORDS  " &
                                                " WHERE GROUP_CODE = @GROUP_CODE AND " &
                                                "   GROUP_LANGUAGE = @GROUP_LANGUAGE "

        Dim dtrReader As SqlDataReader
        Dim cmd As SqlCommand

        Try
            '----------------------------------------------------
            ' If mode is replace then delete existing work tables
            '----------------------------------------------------
            If DeProductGroups.Mode = "REPLACE" Then
                cmd = New SqlCommand(strDelete, conSql2005)
                cmd.ExecuteNonQuery()
                cmd = New SqlCommand(strDeleteLang, conSql2005)
                cmd.ExecuteNonQuery()
            End If

            For Each item As Object In DeProductGroups.ColProductGroup

                productGroup = CType(item, DEProductGroup)
                If productGroup.Details.Count > 0 Then
                    groupDetails = CType(productGroup.Details.Item(1), DEProductGroupDetails)
                End If

                groupExists = False
                '---------------------------------------
                ' REPLACE - Add the item to a work table
                '---------------------------------------
                If DeProductGroups.Mode = "REPLACE" Then
                    cmd = New SqlCommand(strInsertWork, conSql2005)

                    AccessDatabaseSQL2005_ProductNavigationLoad_SetParms(cmd.Parameters, "tbl_group_work", "INSERT", productGroup, groupDetails)

                    cmd.ExecuteNonQuery()
                    '--------------------------------
                    ' Check language specific entries
                    '--------------------------------
                    For Each groupDetails In productGroup.Details

                        cmd = New SqlCommand(strInsertLangWork, conSql2005)
                        AccessDatabaseSQL2005_ProductNavigationLoad_SetParms(cmd.Parameters, "tbl_group_lang_work", "INSERT", productGroup, groupDetails)

                        cmd.ExecuteNonQuery()
                    Next

                ElseIf productGroup.Mode = "ADD" Then
                    '---------------------------
                    ' check if it already exists
                    '---------------------------
                    cmd = New SqlCommand(strSelect1, conSql2005)
                    With cmd.Parameters
                        .Clear()
                        .Add(New SqlParameter("@GROUP_NAME", SqlDbType.NVarChar)).Value = productGroup.Code
                    End With
                    dtrReader = cmd.ExecuteReader
                    If dtrReader.HasRows Then
                        groupExists = True
                    End If
                    dtrReader.Close()

                    If Not groupExists Then

                        cmd = New SqlCommand(strInsert, conSql2005)

                        AccessDatabaseSQL2005_ProductNavigationLoad_SetParms(cmd.Parameters, "tbl_group", "INSERT", productGroup, groupDetails)

                        cmd.ExecuteNonQuery()
                    End If
                    '--------------------------------
                    ' Check language specific entries
                    '--------------------------------
                    For Each groupDetails In productGroup.Details
                        groupLangExists = False
                        '---------------------------
                        ' check if it already exists
                        '---------------------------
                        cmd = New SqlCommand(strSelectGroupLang, conSql2005)
                        With cmd.Parameters
                            .Clear()
                            .Add(New SqlParameter("@GROUP_CODE", SqlDbType.NVarChar)).Value = productGroup.Code
                            .Add(New SqlParameter("@GROUP_LANGUAGE", SqlDbType.NVarChar)).Value = groupDetails.Language
                        End With
                        dtrReader = cmd.ExecuteReader
                        If dtrReader.HasRows Then
                            groupLangExists = True
                        End If
                        dtrReader.Close()

                        If Not groupLangExists Then
                            cmd = New SqlCommand(strInsertLang, conSql2005)
                            AccessDatabaseSQL2005_ProductNavigationLoad_SetParms(cmd.Parameters, "tbl_group_lang", "INSERT", productGroup, groupDetails)

                            cmd.ExecuteNonQuery()
                        End If
                    Next

                    '----------------
                    ' Delete the item
                    '----------------
                ElseIf productGroup.Mode = "DELETE" Then
                    cmd = New SqlCommand(strDeleteGroupByGroup, conSql2005)

                    With cmd.Parameters
                        .Clear()
                        .Add(New SqlParameter("@GROUP_NAME", SqlDbType.NVarChar)).Value = productGroup.Code
                    End With
                    cmd.ExecuteNonQuery()
                    cmd = New SqlCommand(strDeleteGroupLangByGroup, conSql2005)
                    With cmd.Parameters
                        .Clear()
                        .Add(New SqlParameter("@GROUP_CODE", SqlDbType.NVarChar)).Value = productGroup.Code
                    End With
                    cmd.ExecuteNonQuery()

                    '-------------------------------
                    ' Update the item (if it exists)
                    '-------------------------------
                ElseIf productGroup.Mode = "UPDATE" Then
                    groupDetails = CType(productGroup.Details.Item(1), DEProductGroupDetails)
                    cmd = New SqlCommand(strUpdateGroupByGroup, conSql2005)
                    AccessDatabaseSQL2005_ProductNavigationLoad_SetParms(cmd.Parameters, "tbl_group", "UPDATE", productGroup, groupDetails)

                    cmd.ExecuteNonQuery()
                    '--------------------------------
                    ' Check language specific entries
                    '--------------------------------
                    For Each groupDetails In productGroup.Details
                        cmd = New SqlCommand(strUpdateGroupLangByGroup, conSql2005)
                        AccessDatabaseSQL2005_ProductNavigationLoad_SetParms(cmd.Parameters, "tbl_group_lang", "UPDATE", productGroup, groupDetails)

                        cmd.ExecuteNonQuery()
                    Next
                End If
            Next

            '----------------------------------------------
            ' If REPLACE then now need to call datatransfer
            ' to write from work files to main files
            '----------------------------------------------
            If DeProductGroups.Mode = "REPLACE" Then
                DataTransfer.SQLConnectionString = Settings.FrontEndConnectionString
                DataTransfer.DoUpdateTable_EBGROU()
                DataTransfer.DoUpdateTable_tbl_group_lang()
            End If
        Catch ex As Exception
            ResultDataSet = Nothing
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBPD-SQL2005-1"
                .HasError = True
            End With
        End Try

        '========================
        ' PROCESS GROUP HIERARCHY
        '========================
        If Not DeProductGroupHierarchies Is Nothing AndAlso Not DeProductGroupHierarchies.ProductGroupHierarchies Is Nothing Then
            'Const strDeleteGroup01 = "DELETE FROM TBL_GROUP_LEVEL_01_WORK WHERE GROUP_L01_BUSINESS_UNIT = @BUSINESS_UNIT AND GROUP_L01_PARTNER = @PARTNER"
            'Const strDeleteGroup02 = "DELETE FROM TBL_GROUP_LEVEL_02_WORK WHERE GROUP_L02_BUSINESS_UNIT = @BUSINESS_UNIT AND GROUP_L02_PARTNER = @PARTNER"
            'Const strDeleteGroup03 = "DELETE FROM TBL_GROUP_LEVEL_03_WORK WHERE GROUP_L03_BUSINESS_UNIT = @BUSINESS_UNIT AND GROUP_L03_PARTNER = @PARTNER"
            'Const strDeleteGroup04 = "DELETE FROM TBL_GROUP_LEVEL_04_WORK WHERE GROUP_L04_BUSINESS_UNIT = @BUSINESS_UNIT AND GROUP_L04_PARTNER = @PARTNER"
            'Const strDeleteGroup05 = "DELETE FROM TBL_GROUP_LEVEL_05_WORK WHERE GROUP_L05_BUSINESS_UNIT = @BUSINESS_UNIT AND GROUP_L05_PARTNER = @PARTNER"
            'Const strDeleteGroup06 = "DELETE FROM TBL_GROUP_LEVEL_06_WORK WHERE GROUP_L06_BUSINESS_UNIT = @BUSINESS_UNIT AND GROUP_L06_PARTNER = @PARTNER"
            'Const strDeleteGroup07 = "DELETE FROM TBL_GROUP_LEVEL_07_WORK WHERE GROUP_L07_BUSINESS_UNIT = @BUSINESS_UNIT AND GROUP_L07_PARTNER = @PARTNER"
            'Const strDeleteGroup08 = "DELETE FROM TBL_GROUP_LEVEL_08_WORK WHERE GROUP_L08_BUSINESS_UNIT = @BUSINESS_UNIT AND GROUP_L08_PARTNER = @PARTNER"
            'Const strDeleteGroup09 = "DELETE FROM TBL_GROUP_LEVEL_09_WORK WHERE GROUP_L09_BUSINESS_UNIT = @BUSINESS_UNIT AND GROUP_L09_PARTNER = @PARTNER"
            'Const strDeleteGroup10 = "DELETE FROM TBL_GROUP_LEVEL_10_WORK WHERE GROUP_L10_BUSINESS_UNIT = @BUSINESS_UNIT AND GROUP_L10_PARTNER = @PARTNER"
            'Const strDeleteGroupProduct = "DELETE FROM TBL_GROUP_PRODUCT_WORK WHERE GROUP_BUSINESS_UNIT = @BUSINESS_UNIT AND GROUP_PARTNER = @PARTNER"

            Const strDeleteGroup01 = "DELETE FROM TBL_GROUP_LEVEL_01_WORK "
            Const strDeleteGroup02 = "DELETE FROM TBL_GROUP_LEVEL_02_WORK "
            Const strDeleteGroup03 = "DELETE FROM TBL_GROUP_LEVEL_03_WORK "
            Const strDeleteGroup04 = "DELETE FROM TBL_GROUP_LEVEL_04_WORK "
            Const strDeleteGroup05 = "DELETE FROM TBL_GROUP_LEVEL_05_WORK "
            Const strDeleteGroup06 = "DELETE FROM TBL_GROUP_LEVEL_06_WORK "
            Const strDeleteGroup07 = "DELETE FROM TBL_GROUP_LEVEL_07_WORK "
            Const strDeleteGroup08 = "DELETE FROM TBL_GROUP_LEVEL_08_WORK "
            Const strDeleteGroup09 = "DELETE FROM TBL_GROUP_LEVEL_09_WORK "
            Const strDeleteGroup10 = "DELETE FROM TBL_GROUP_LEVEL_10_WORK "
            Const strDeleteGroupProduct = "DELETE FROM TBL_GROUP_PRODUCT_WORK "


            With groupFields
                .Add("GROUP_Lxx_BUSINESS_UNIT")
                .Add("GROUP_Lxx_PARTNER")
                .Add("GROUP_Lxx_L01_GROUP")
                .Add("GROUP_Lxx_L02_GROUP")
                .Add("GROUP_Lxx_L03_GROUP")
                .Add("GROUP_Lxx_L04_GROUP")
                .Add("GROUP_Lxx_L05_GROUP")
                .Add("GROUP_Lxx_L06_GROUP")
                .Add("GROUP_Lxx_L07_GROUP")
                .Add("GROUP_Lxx_L08_GROUP")
                .Add("GROUP_Lxx_L09_GROUP")
                .Add("GROUP_Lxx_L10_GROUP")
                .Add("GROUP_Lxx_SEQUENCE")
                .Add("GROUP_Lxx_DESCRIPTION_1")
                .Add("GROUP_Lxx_DESCRIPTION_2")
                .Add("GROUP_Lxx_HTML_1")
                .Add("GROUP_Lxx_HTML_2")
                .Add("GROUP_Lxx_HTML_3")
                .Add("GROUP_Lxx_PAGE_TITLE")
                .Add("GROUP_Lxx_META_DESCRIPTION")
                .Add("GROUP_Lxx_META_KEYWORDS")
                .Add("GROUP_Lxx_ADV_SEARCH_TEMPLATE")
                .Add("GROUP_Lxx_PRODUCT_PAGE_TEMPLATE")
                .Add("GROUP_Lxx_PRODUCT_LIST_TEMPLATE")
                .Add("GROUP_Lxx_SHOW_CHILDREN_AS_GROUPS")
                .Add("GROUP_Lxx_SHOW_PRODUCTS_AS_LIST")
                .Add("GROUP_Lxx_SHOW_IN_NAVIGATION")
                .Add("GROUP_Lxx_SHOW_IN_GROUPED_NAV")
                .Add("GROUP_Lxx_HTML_GROUP")
                .Add("GROUP_Lxx_HTML_GROUP_TYPE")
                .Add("GROUP_Lxx_SHOW_PRODUCT_DISPLAY")
                .Add("GROUP_Lxx_THEME")
            End With

            Dim productGroupHierarchy As DEProductGroupHierarchy
            Dim businessUnit As String = String.Empty
            Dim partner As String = String.Empty
            Dim mode As String = String.Empty
            Dim level01group, level02group, level03group, level04group, level05group,
                level06group, level07group, level08group, level09group, level10group As DEProductGroupHierarchyGroup
            Dim strInsertBuild As String = String.Empty
            Dim level01groupName As String = String.Empty

            Try
                For Each item As Object In DeProductGroupHierarchies.ProductGroupHierarchies


                    productGroupHierarchy = CType(item, DEProductGroupHierarchy)
                    businessUnit = productGroupHierarchy.BusinessUnit
                    partner = productGroupHierarchy.Partner
                    mode = productGroupHierarchy.Mode

                    Dim productsAfterLevel As Integer = CInt(productGroupHierarchy.LastProductAfterLevel)

                    '---------------------------------------------------
                    ' If replace then delete product group hierarchy for
                    ' current bu / partner on work tables
                    '---------------------------------------------------
                    If mode = "REPLACE" Then
                        cmd = New SqlCommand(strDeleteGroup01, conSql2005)
                        ' AccessDatabaseSQL2005_ProductNavigationLoad_SetParms(cmd.Parameters, "tbl_group_level_01_work", "DELETE", Nothing, Nothing, productGroupHierarchy)
                        cmd.ExecuteNonQuery()

                        cmd = New SqlCommand(strDeleteGroup02, conSql2005)
                        ' AccessDatabaseSQL2005_ProductNavigationLoad_SetParms(cmd.Parameters, "tbl_group_level_02_work", "DELETE", Nothing, Nothing, productGroupHierarchy)
                        cmd.ExecuteNonQuery()

                        cmd = New SqlCommand(strDeleteGroup03, conSql2005)
                        '  AccessDatabaseSQL2005_ProductNavigationLoad_SetParms(cmd.Parameters, "tbl_group_level_03_work", "DELETE", Nothing, Nothing, productGroupHierarchy)
                        cmd.ExecuteNonQuery()

                        cmd = New SqlCommand(strDeleteGroup04, conSql2005)
                        '  AccessDatabaseSQL2005_ProductNavigationLoad_SetParms(cmd.Parameters, "tbl_group_level_04_work", "DELETE", Nothing, Nothing, productGroupHierarchy)
                        cmd.ExecuteNonQuery()

                        cmd = New SqlCommand(strDeleteGroup05, conSql2005)
                        '  AccessDatabaseSQL2005_ProductNavigationLoad_SetParms(cmd.Parameters, "tbl_group_level_05_work", "DELETE", Nothing, Nothing, productGroupHierarchy)
                        cmd.ExecuteNonQuery()

                        cmd = New SqlCommand(strDeleteGroup06, conSql2005)
                        ' AccessDatabaseSQL2005_ProductNavigationLoad_SetParms(cmd.Parameters, "tbl_group_level_06_work", "DELETE", Nothing, Nothing, productGroupHierarchy)
                        cmd.ExecuteNonQuery()

                        cmd = New SqlCommand(strDeleteGroup07, conSql2005)
                        ' AccessDatabaseSQL2005_ProductNavigationLoad_SetParms(cmd.Parameters, "tbl_group_level_07_work", "DELETE", Nothing, Nothing, productGroupHierarchy)
                        cmd.ExecuteNonQuery()

                        cmd = New SqlCommand(strDeleteGroup08, conSql2005)
                        ' AccessDatabaseSQL2005_ProductNavigationLoad_SetParms(cmd.Parameters, "tbl_group_level_08_work", "DELETE", Nothing, Nothing, productGroupHierarchy)
                        cmd.ExecuteNonQuery()

                        cmd = New SqlCommand(strDeleteGroup09, conSql2005)
                        ' AccessDatabaseSQL2005_ProductNavigationLoad_SetParms(cmd.Parameters, "tbl_group_level_09_work", "DELETE", Nothing, Nothing, productGroupHierarchy)
                        cmd.ExecuteNonQuery()

                        cmd = New SqlCommand(strDeleteGroup10, conSql2005)
                        ' AccessDatabaseSQL2005_ProductNavigationLoad_SetParms(cmd.Parameters, "tbl_group_level_10_work", "DELETE", Nothing, Nothing, productGroupHierarchy)
                        cmd.ExecuteNonQuery()

                        cmd = New SqlCommand(strDeleteGroupProduct, conSql2005)
                        With cmd.Parameters
                            .Add(New SqlParameter("@BUSINESS_UNIT", SqlDbType.NVarChar)).Value = productGroupHierarchy.BusinessUnit
                            .Add(New SqlParameter("@PARTNER", SqlDbType.NVarChar)).Value = productGroupHierarchy.Partner
                        End With
                        cmd.ExecuteNonQuery()
                    End If

                    For Each item2 As Object In productGroupHierarchy.Level1Groups
                        level01group = CType(item2, DEProductGroupHierarchyGroup)
                        level01group.L01Group = level01group.Code
                        If mode = "REPLACE" Then
                            '---------------------
                            ' Add to level 1 table
                            '---------------------
                            strInsertBuild = AccessDatabaseSQL2005_ProductNavigationLoad_BuildSql("tbl_group_level_01_work", "INSERT", "L01", groupFields)
                            cmd = New SqlCommand(strInsertBuild, conSql2005)
                            AccessDatabaseSQL2005_ProductNavigationLoad_SetParms(cmd.Parameters, "tbl_group_level_01_work", "INSERT", Nothing, Nothing, productGroupHierarchy, "L01", level01group)

                            cmd.ExecuteNonQuery()

                            AccessDatabaseSQL2005_ProductNavigationLoad_ProcessProducts(level01group, "tbl_group_product_work", productsAfterLevel)

                            '-------------------
                            ' Build empty groups
                            '-------------------
                            AccessDatabaseSQL2005_ProductNavigationLoad_BuildEmptyGroups(level01group, 1, productsAfterLevel, "_work")

                            For Each item3 As Object In level01group.NextLevelGroups
                                level02group = CType(item3, DEProductGroupHierarchyGroup)
                                level02group.L01Group = level01group.Code
                                level02group.L02Group = level02group.Code
                                '---------------------
                                ' Add to level 2 table
                                '---------------------
                                strInsertBuild = AccessDatabaseSQL2005_ProductNavigationLoad_BuildSql("tbl_group_level_02_work", "INSERT", "L02", groupFields)
                                cmd = New SqlCommand(strInsertBuild, conSql2005)
                                AccessDatabaseSQL2005_ProductNavigationLoad_SetParms(cmd.Parameters, "tbl_group_level_02_work", "INSERT", Nothing, Nothing, productGroupHierarchy, "L02", level02group)

                                cmd.ExecuteNonQuery()

                                AccessDatabaseSQL2005_ProductNavigationLoad_ProcessProducts(level02group, "tbl_group_product_work", productsAfterLevel)
                                '-------------------
                                ' Build empty groups
                                '-------------------
                                AccessDatabaseSQL2005_ProductNavigationLoad_BuildEmptyGroups(level02group, 2, productsAfterLevel, "_work")

                                For Each item4 As Object In level02group.NextLevelGroups
                                    level03group = CType(item4, DEProductGroupHierarchyGroup)
                                    level03group.L01Group = level01group.Code
                                    level03group.L02Group = level02group.Code
                                    level03group.L03Group = level03group.Code
                                    '---------------------
                                    ' Add to level 3 table
                                    '---------------------
                                    strInsertBuild = AccessDatabaseSQL2005_ProductNavigationLoad_BuildSql("tbl_group_level_03_work", "INSERT", "L03", groupFields)
                                    cmd = New SqlCommand(strInsertBuild, conSql2005)
                                    AccessDatabaseSQL2005_ProductNavigationLoad_SetParms(cmd.Parameters, "tbl_group_level_03_work", "INSERT", Nothing, Nothing, productGroupHierarchy, "L03", level03group)

                                    cmd.ExecuteNonQuery()

                                    AccessDatabaseSQL2005_ProductNavigationLoad_ProcessProducts(level03group, "tbl_group_product_work", productsAfterLevel)
                                    '-------------------
                                    ' Build empty groups
                                    '-------------------
                                    AccessDatabaseSQL2005_ProductNavigationLoad_BuildEmptyGroups(level03group, 3, productsAfterLevel, "_work")

                                    For Each item5 As Object In level03group.NextLevelGroups
                                        level04group = CType(item5, DEProductGroupHierarchyGroup)
                                        level04group.L01Group = level01group.Code
                                        level04group.L02Group = level02group.Code
                                        level04group.L03Group = level03group.Code
                                        level04group.L04Group = level04group.Code
                                        '---------------------
                                        ' Add to level 4 table
                                        '---------------------
                                        strInsertBuild = AccessDatabaseSQL2005_ProductNavigationLoad_BuildSql("tbl_group_level_04_work", "INSERT", "L04", groupFields)
                                        cmd = New SqlCommand(strInsertBuild, conSql2005)
                                        AccessDatabaseSQL2005_ProductNavigationLoad_SetParms(cmd.Parameters, "tbl_group_level_04_work", "INSERT", Nothing, Nothing, productGroupHierarchy, "L04", level04group)

                                        cmd.ExecuteNonQuery()

                                        AccessDatabaseSQL2005_ProductNavigationLoad_ProcessProducts(level04group, "tbl_group_product_work", productsAfterLevel)
                                        '-------------------
                                        ' Build empty groups
                                        '-------------------
                                        AccessDatabaseSQL2005_ProductNavigationLoad_BuildEmptyGroups(level04group, 4, productsAfterLevel, "_work")

                                        For Each item6 As Object In level04group.NextLevelGroups
                                            level05group = CType(item6, DEProductGroupHierarchyGroup)
                                            level05group.L01Group = level01group.Code
                                            level05group.L02Group = level02group.Code
                                            level05group.L03Group = level03group.Code
                                            level05group.L04Group = level04group.Code
                                            level05group.L05Group = level05group.Code
                                            '---------------------
                                            ' Add to level 5 table
                                            '---------------------
                                            strInsertBuild = AccessDatabaseSQL2005_ProductNavigationLoad_BuildSql("tbl_group_level_05_work", "INSERT", "L05", groupFields)
                                            cmd = New SqlCommand(strInsertBuild, conSql2005)
                                            AccessDatabaseSQL2005_ProductNavigationLoad_SetParms(cmd.Parameters, "tbl_group_level_05_work", "INSERT", Nothing, Nothing, productGroupHierarchy, "L05", level05group)

                                            cmd.ExecuteNonQuery()

                                            AccessDatabaseSQL2005_ProductNavigationLoad_ProcessProducts(level05group, "tbl_group_product_work", productsAfterLevel)
                                            '-------------------
                                            ' Build empty groups
                                            '-------------------
                                            AccessDatabaseSQL2005_ProductNavigationLoad_BuildEmptyGroups(level05group, 5, productsAfterLevel, "_work")

                                            For Each item7 As Object In level05group.NextLevelGroups
                                                level06group = CType(item7, DEProductGroupHierarchyGroup)
                                                level06group.L01Group = level01group.Code
                                                level06group.L02Group = level02group.Code
                                                level06group.L03Group = level03group.Code
                                                level06group.L04Group = level04group.Code
                                                level06group.L05Group = level05group.Code
                                                level06group.L06Group = level06group.Code
                                                '---------------------
                                                ' Add to level 6 table
                                                '---------------------
                                                strInsertBuild = AccessDatabaseSQL2005_ProductNavigationLoad_BuildSql("tbl_group_level_06_work", "INSERT", "L06", groupFields)
                                                cmd = New SqlCommand(strInsertBuild, conSql2005)
                                                AccessDatabaseSQL2005_ProductNavigationLoad_SetParms(cmd.Parameters, "tbl_group_level_06_work", "INSERT", Nothing, Nothing, productGroupHierarchy, "L06", level06group)

                                                cmd.ExecuteNonQuery()

                                                AccessDatabaseSQL2005_ProductNavigationLoad_ProcessProducts(level06group, "tbl_group_product_work", productsAfterLevel)
                                                '-------------------
                                                ' Build empty groups
                                                '-------------------
                                                AccessDatabaseSQL2005_ProductNavigationLoad_BuildEmptyGroups(level06group, 6, productsAfterLevel, "_work")


                                                For Each item8 As Object In level06group.NextLevelGroups
                                                    level07group = CType(item8, DEProductGroupHierarchyGroup)
                                                    level07group.L01Group = level01group.Code
                                                    level07group.L02Group = level02group.Code
                                                    level07group.L03Group = level03group.Code
                                                    level07group.L04Group = level04group.Code
                                                    level07group.L05Group = level05group.Code
                                                    level07group.L06Group = level06group.Code
                                                    level07group.L07Group = level07group.Code
                                                    '---------------------
                                                    ' Add to level 7 table
                                                    '---------------------
                                                    strInsertBuild = AccessDatabaseSQL2005_ProductNavigationLoad_BuildSql("tbl_group_level_07_work", "INSERT", "L07", groupFields)
                                                    cmd = New SqlCommand(strInsertBuild, conSql2005)
                                                    AccessDatabaseSQL2005_ProductNavigationLoad_SetParms(cmd.Parameters, "tbl_group_level_07_work", "INSERT", Nothing, Nothing, productGroupHierarchy, "L07", level07group)

                                                    cmd.ExecuteNonQuery()

                                                    AccessDatabaseSQL2005_ProductNavigationLoad_ProcessProducts(level07group, "tbl_group_product_work", productsAfterLevel)
                                                    '-------------------
                                                    ' Build empty groups
                                                    '-------------------
                                                    AccessDatabaseSQL2005_ProductNavigationLoad_BuildEmptyGroups(level07group, 7, productsAfterLevel, "_work")


                                                    For Each item9 As Object In level07group.NextLevelGroups
                                                        level08group = CType(item9, DEProductGroupHierarchyGroup)
                                                        level08group.L01Group = level01group.Code
                                                        level08group.L02Group = level02group.Code
                                                        level08group.L03Group = level03group.Code
                                                        level08group.L04Group = level04group.Code
                                                        level08group.L05Group = level05group.Code
                                                        level08group.L06Group = level06group.Code
                                                        level08group.L07Group = level07group.Code
                                                        level08group.L08Group = level08group.Code
                                                        '---------------------
                                                        ' Add to level 8 table
                                                        '---------------------
                                                        strInsertBuild = AccessDatabaseSQL2005_ProductNavigationLoad_BuildSql("tbl_group_level_08_work", "INSERT", "L08", groupFields)
                                                        cmd = New SqlCommand(strInsertBuild, conSql2005)
                                                        AccessDatabaseSQL2005_ProductNavigationLoad_SetParms(cmd.Parameters, "tbl_group_level_08_work", "INSERT", Nothing, Nothing, productGroupHierarchy, "L08", level08group)

                                                        cmd.ExecuteNonQuery()

                                                        AccessDatabaseSQL2005_ProductNavigationLoad_ProcessProducts(level08group, "tbl_group_product_work", productsAfterLevel)
                                                        '-------------------
                                                        ' Build empty groups
                                                        '-------------------
                                                        AccessDatabaseSQL2005_ProductNavigationLoad_BuildEmptyGroups(level08group, 8, productsAfterLevel, "_work")


                                                        For Each item10 As Object In level08group.NextLevelGroups
                                                            level09group = CType(item10, DEProductGroupHierarchyGroup)
                                                            level09group.L01Group = level01group.Code
                                                            level09group.L02Group = level02group.Code
                                                            level09group.L03Group = level03group.Code
                                                            level09group.L04Group = level04group.Code
                                                            level09group.L05Group = level05group.Code
                                                            level09group.L06Group = level06group.Code
                                                            level09group.L07Group = level07group.Code
                                                            level09group.L08Group = level08group.Code
                                                            level09group.L09Group = level09group.Code
                                                            '---------------------
                                                            ' Add to level 9 table
                                                            '---------------------
                                                            strInsertBuild = AccessDatabaseSQL2005_ProductNavigationLoad_BuildSql("tbl_group_level_09_work", "INSERT", "L09", groupFields)
                                                            cmd = New SqlCommand(strInsertBuild, conSql2005)
                                                            AccessDatabaseSQL2005_ProductNavigationLoad_SetParms(cmd.Parameters, "tbl_group_level_09_work", "INSERT", Nothing, Nothing, productGroupHierarchy, "L09", level09group)

                                                            cmd.ExecuteNonQuery()

                                                            AccessDatabaseSQL2005_ProductNavigationLoad_ProcessProducts(level09group, "tbl_group_product_work", productsAfterLevel)
                                                            '-------------------
                                                            ' Build empty groups
                                                            '-------------------
                                                            AccessDatabaseSQL2005_ProductNavigationLoad_BuildEmptyGroups(level09group, 9, productsAfterLevel, "_work")

                                                            For Each item11 As Object In level09group.NextLevelGroups
                                                                level10group = CType(item11, DEProductGroupHierarchyGroup)
                                                                level10group.L01Group = level01group.Code
                                                                level10group.L02Group = level02group.Code
                                                                level10group.L03Group = level03group.Code
                                                                level10group.L04Group = level04group.Code
                                                                level10group.L05Group = level05group.Code
                                                                level10group.L06Group = level06group.Code
                                                                level10group.L07Group = level07group.Code
                                                                level10group.L08Group = level08group.Code
                                                                level10group.L09Group = level09group.Code
                                                                level10group.L10Group = level10group.Code
                                                                '----------------------
                                                                ' Add to level 10 table
                                                                '----------------------
                                                                strInsertBuild = AccessDatabaseSQL2005_ProductNavigationLoad_BuildSql("tbl_group_level_10_work", "INSERT", "L10", groupFields)
                                                                cmd = New SqlCommand(strInsertBuild, conSql2005)
                                                                AccessDatabaseSQL2005_ProductNavigationLoad_SetParms(cmd.Parameters, "tbl_group_level_10_work", "INSERT", Nothing, Nothing, productGroupHierarchy, "L10", level10group)

                                                                cmd.ExecuteNonQuery()

                                                                AccessDatabaseSQL2005_ProductNavigationLoad_ProcessProducts(level10group, "tbl_group_product_work", productsAfterLevel)
                                                                '-------------------
                                                                ' Build empty groups
                                                                '-------------------
                                                                AccessDatabaseSQL2005_ProductNavigationLoad_BuildEmptyGroups(level10group, 10, productsAfterLevel, "_work")

                                                            Next
                                                        Next
                                                    Next
                                                Next
                                            Next
                                        Next
                                    Next
                                Next
                            Next

                        Else
                            '-----------------------------------
                            ' Process group directly to database
                            '-----------------------------------
                            level01group.L01Group = level01group.Code
                            AccessDatabaseSQL2005_ProductNavigationLoad_ProcessGroup(level01group, 1, productsAfterLevel)
                        End If
                    Next
                    If mode = "REPLACE" Then
                        '--------------------------------------------------------------------
                        ' Work files now written to - call data transfer to update live files
                        '--------------------------------------------------------------------
                        DataTransfer.SQLConnectionString = Settings.FrontEndConnectionString
                        DataTransfer.DoUpdateTable_EBGL01(businessUnit, partner)
                        DataTransfer.DoUpdateTable_EBGL02(businessUnit, partner)
                        DataTransfer.DoUpdateTable_EBGL03(businessUnit, partner)
                        DataTransfer.DoUpdateTable_EBGL04(businessUnit, partner)
                        DataTransfer.DoUpdateTable_EBGL05(businessUnit, partner)
                        DataTransfer.DoUpdateTable_EBGL06(businessUnit, partner)
                        DataTransfer.DoUpdateTable_EBGL07(businessUnit, partner)
                        DataTransfer.DoUpdateTable_EBGL08(businessUnit, partner)
                        DataTransfer.DoUpdateTable_EBGL09(businessUnit, partner)
                        DataTransfer.DoUpdateTable_EBGL10(businessUnit, partner)
                        DataTransfer.DoUpdateTable_EBGPPR(businessUnit, partner)
                    End If
                Next

            Catch ex As Exception
                ResultDataSet = Nothing
                Const strError As String = "Error during database access"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TACDBPD-SQL2005-2"
                    .HasError = True
                End With
            End Try

        End If

        Return err

    End Function

    Private Function AccessDatabaseSQL2005_ProductStockLoad() As ErrorObj
        Dim err As New ErrorObj

        '==========================
        ' PROCESS GROUP DEFINITIONS
        '==========================
        Dim groupExists As Boolean = False
        Dim groupLangExists As Boolean = False

        Const strInsert As String = "INSERT INTO tbl_product_stock " &
                                     "       (PRODUCT, " &
                                     "        STOCK_LOCATION, " &
                                     "        QUANTITY, " &
                                     "        ALLOCATED_QUANTITY, " &
                                     "        AVAILABLE_QUANTITY, " &
                                     "        RESTOCK_CODE, " &
                                     "        WAREHOUSE) " &
                                     "   VALUES(@PRODUCT, " &
                                     "        @STOCK_LOCATION, " &
                                     "        @QUANTITY, " &
                                     "        @ALLOCATED_QUANTITY, " &
                                     "        @AVAILABLE_QUANTITY, " &
                                     "        @RESTOCK_CODE, " &
                                     "        @WAREHOUSE )"

        Const strInsertWork As String = "INSERT INTO TBL_PRODUCT_STOCK_WORK " &
                                     "       (PRODUCT, " &
                                     "        STOCK_LOCATION, " &
                                     "        QUANTITY, " &
                                     "        ALLOCATED_QUANTITY, " &
                                     "        AVAILABLE_QUANTITY, " &
                                     "        RESTOCK_CODE, " &
                                     "        WAREHOUSE) " &
                                     "   VALUES(@PRODUCT, " &
                                     "        @STOCK_LOCATION, " &
                                     "        @QUANTITY, " &
                                     "        @ALLOCATED_QUANTITY, " &
                                     "        @AVAILABLE_QUANTITY, " &
                                     "        @RESTOCK_CODE, " &
                                     "        @WAREHOUSE )"


        Const strSelect1 As String = "SELECT * FROM TBL_PRODUCT_STOCK WHERE PRODUCT = @PRODUCT"

        Const strDelete As String = "DELETE FROM TBL_PRODUCT_STOCK_WORK"
        Const strDeleteGroupByGroup As String = "DELETE FROM TBL_PRODUCT_STOCK WHERE PRODUCT = @PRODUCT"

        Const strUpdateGroupByGroup As String = "UPDATE TBL_PRODUCT_STOCK SET " &
                                                " STOCK_LOCATION = @STOCK_LOCATION,  " &
                                                " QUANTITY = @QUANTITY,  " &
                                                " ALLOCATED_QUANTITY = @ALLOCATED_QUANTITY ,  " &
                                                " AVAILABLE_QUANTITY = @AVAILABLE_QUANTITY,  " &
                                                " RESTOCK_CODE = @RESTOCK_CODE,  " &
                                                " WAREHOUSE = @WAREHOUSE WHERE PRODUCT = @PRODUCT"

        Dim dtrReader As SqlDataReader
        Dim cmd As SqlCommand

        Try
            '----------------------------------------------------
            ' If mode is replace then delete existing work tables
            '----------------------------------------------------
            If Stock.Products.Count > 0 Then
                For Each products As DEStockProducts In Stock.Products
                    If (products.Mode = "REPLACE") Then
                        cmd = New SqlCommand(strDelete, conSql2005)
                        cmd.ExecuteNonQuery()
                    End If
                    Exit For
                Next
            End If

            For Each products As DEStockProducts In Stock.Products
                For Each product As DEStockProduct In products.StockProducts
                    '---------------------------------------
                    ' REPLACE - Add the item to a work table
                    '---------------------------------------
                    If products.Mode = "REPLACE" Then
                        cmd = New SqlCommand(strInsertWork, conSql2005)

                        AccessDatabaseSQL2005_ProductStockLoad_SetParms(cmd.Parameters, "tbl_product_stock_work", "INSERT", product)

                        cmd.ExecuteNonQuery()

                    ElseIf products.Mode = "ADD" Then
                        groupExists = False
                        '---------------------------
                        ' check if it already exists
                        '---------------------------
                        cmd = New SqlCommand(strSelect1, conSql2005)
                        With cmd.Parameters
                            .Clear()
                            .Add(New SqlParameter("@PRODUCT", SqlDbType.NVarChar)).Value = product.SKU
                        End With
                        dtrReader = cmd.ExecuteReader
                        If dtrReader.HasRows Then
                            groupExists = True
                        End If
                        dtrReader.Close()

                        If Not groupExists Then

                            cmd = New SqlCommand(strInsert, conSql2005)

                            AccessDatabaseSQL2005_ProductStockLoad_SetParms(cmd.Parameters, "tbl_product_stock", "INSERT", product)

                            cmd.ExecuteNonQuery()
                        End If

                        '----------------
                        ' Delete the item
                        '----------------
                    ElseIf products.Mode = "DELETE" Then
                        cmd = New SqlCommand(strDeleteGroupByGroup, conSql2005)

                        With cmd.Parameters
                            .Clear()
                            .Add(New SqlParameter("@PRODUCT", SqlDbType.NVarChar)).Value = product.SKU
                        End With
                        cmd.ExecuteNonQuery()

                        '-------------------------------
                        ' Update the item (if it exists)
                        '-------------------------------
                    ElseIf products.Mode = "UPDATE" Then
                        Dim rowsUpdated As Integer = 0
                        cmd = New SqlCommand(strUpdateGroupByGroup, conSql2005)
                        AccessDatabaseSQL2005_ProductStockLoad_SetParms(cmd.Parameters, "tbl_product_stock", "UPDATE", product)

                        cmd.ExecuteNonQuery()
                        ' treat as add
                        If rowsUpdated = 0 Then
                            cmd = New SqlCommand(strInsert, conSql2005)

                            AccessDatabaseSQL2005_ProductStockLoad_SetParms(cmd.Parameters, "tbl_product_stock", "INSERT", product)

                            cmd.ExecuteNonQuery()
                        End If
                    End If
                Next
            Next
            '----------------------------------------------
            ' If REPLACE then now need to call datatransfer
            ' to write from work files to main files
            '----------------------------------------------            
            For Each products As DEStockProducts In Stock.Products
                If (products.Mode = "REPLACE") Then
                    DataTransfer.SQLConnectionString = Settings.FrontEndConnectionString
                    DataTransfer.DoUpdateTable_EBSTOCK()
                End If
                Exit For
            Next
        Catch ex As Exception
            ResultDataSet = Nothing
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError & "-" & ex.Message
                .ErrorNumber = "TACDBPD-SQL2005-1"
                .HasError = True
            End With
        End Try

        Return err

    End Function
    Private Sub AccessDatabaseSQL2005_ProductNavigationLoad_BuildEmptyGroups(ByVal group As DEProductGroupHierarchyGroup,
                                                                            ByVal currentLevel As Integer,
                                                                            ByVal productLevelAfter As Integer,
                                                                            ByVal fileExtension As String)
        Dim strInsertBuild As String = String.Empty
        Dim cmd As SqlCommand

        If productLevelAfter > currentLevel AndAlso group.NextLevelGroupsTotal = 0 Then
            If currentLevel = 1 AndAlso productLevelAfter > 1 Then
                '---------------------
                ' Write empty level 02
                '---------------------
                strInsertBuild = AccessDatabaseSQL2005_ProductNavigationLoad_BuildSql("tbl_group_level_02" & fileExtension, "INSERT", "L02", groupFields)

                cmd = New SqlCommand(strInsertBuild, conSql2005)
                AccessDatabaseSQL2005_ProductNavigationLoad_SetParms(cmd.Parameters, "tbl_group_level_02" & fileExtension, "INSERT", Nothing, Nothing, Nothing, "L02", group)

                cmd.ExecuteNonQuery()

            End If

            If currentLevel < 3 AndAlso productLevelAfter > 2 Then
                '---------------------
                ' Write empty level 03
                '---------------------
                strInsertBuild = AccessDatabaseSQL2005_ProductNavigationLoad_BuildSql("tbl_group_level_03" & fileExtension, "INSERT", "L03", groupFields)

                cmd = New SqlCommand(strInsertBuild, conSql2005)
                AccessDatabaseSQL2005_ProductNavigationLoad_SetParms(cmd.Parameters, "tbl_group_level_03" & fileExtension, "INSERT", Nothing, Nothing, Nothing, "L03", group)

                cmd.ExecuteNonQuery()

            End If
            If currentLevel < 4 AndAlso productLevelAfter > 3 Then
                '---------------------
                ' Write empty level 04
                '---------------------
                strInsertBuild = AccessDatabaseSQL2005_ProductNavigationLoad_BuildSql("tbl_group_level_04" & fileExtension, "INSERT", "L04", groupFields)

                cmd = New SqlCommand(strInsertBuild, conSql2005)
                AccessDatabaseSQL2005_ProductNavigationLoad_SetParms(cmd.Parameters, "tbl_group_level_04" & fileExtension, "INSERT", Nothing, Nothing, Nothing, "L04", group)

                cmd.ExecuteNonQuery()

            End If
            If currentLevel < 5 AndAlso productLevelAfter > 4 Then
                '---------------------
                ' Write empty level 05
                '---------------------
                strInsertBuild = AccessDatabaseSQL2005_ProductNavigationLoad_BuildSql("tbl_group_level_05" & fileExtension, "INSERT", "L05", groupFields)

                cmd = New SqlCommand(strInsertBuild, conSql2005)
                AccessDatabaseSQL2005_ProductNavigationLoad_SetParms(cmd.Parameters, "tbl_group_level_05" & fileExtension, "INSERT", Nothing, Nothing, Nothing, "L05", group)

                cmd.ExecuteNonQuery()

            End If
            If currentLevel < 6 AndAlso productLevelAfter > 5 Then
                '---------------------
                ' Write empty level 06
                '---------------------
                strInsertBuild = AccessDatabaseSQL2005_ProductNavigationLoad_BuildSql("tbl_group_level_06" & fileExtension, "INSERT", "L06", groupFields)

                cmd = New SqlCommand(strInsertBuild, conSql2005)
                AccessDatabaseSQL2005_ProductNavigationLoad_SetParms(cmd.Parameters, "tbl_group_level_06" & fileExtension, "INSERT", Nothing, Nothing, Nothing, "L06", group)

                cmd.ExecuteNonQuery()

            End If
            If currentLevel < 7 AndAlso productLevelAfter > 6 Then
                '---------------------
                ' Write empty level 07
                '---------------------
                strInsertBuild = AccessDatabaseSQL2005_ProductNavigationLoad_BuildSql("tbl_group_level_07" & fileExtension, "INSERT", "L07", groupFields)

                cmd = New SqlCommand(strInsertBuild, conSql2005)
                AccessDatabaseSQL2005_ProductNavigationLoad_SetParms(cmd.Parameters, "tbl_group_level_07" & fileExtension, "INSERT", Nothing, Nothing, Nothing, "L07", group)

                cmd.ExecuteNonQuery()

            End If
            If currentLevel < 8 AndAlso productLevelAfter > 7 Then
                '---------------------
                ' Write empty level 08
                '---------------------
                strInsertBuild = AccessDatabaseSQL2005_ProductNavigationLoad_BuildSql("tbl_group_level_08" & fileExtension, "INSERT", "L08", groupFields)

                cmd = New SqlCommand(strInsertBuild, conSql2005)
                AccessDatabaseSQL2005_ProductNavigationLoad_SetParms(cmd.Parameters, "tbl_group_level_08" & fileExtension, "INSERT", Nothing, Nothing, Nothing, "L07", group)

                cmd.ExecuteNonQuery()

            End If
            If currentLevel < 9 AndAlso productLevelAfter > 8 Then
                '---------------------
                ' Write empty level 09
                '---------------------
                strInsertBuild = AccessDatabaseSQL2005_ProductNavigationLoad_BuildSql("tbl_group_level_09" & fileExtension, "INSERT", "L09", groupFields)

                cmd = New SqlCommand(strInsertBuild, conSql2005)
                AccessDatabaseSQL2005_ProductNavigationLoad_SetParms(cmd.Parameters, "tbl_group_level_09" & fileExtension, "INSERT", Nothing, Nothing, Nothing, "L03", group)

                cmd.ExecuteNonQuery()

            End If
            If currentLevel < 10 AndAlso productLevelAfter > 9 Then
                '---------------------
                ' Write empty level 10
                '---------------------
                strInsertBuild = AccessDatabaseSQL2005_ProductNavigationLoad_BuildSql("tbl_group_level_10" & fileExtension, "INSERT", "L10", groupFields)

                cmd = New SqlCommand(strInsertBuild, conSql2005)
                AccessDatabaseSQL2005_ProductNavigationLoad_SetParms(cmd.Parameters, "tbl_group_level_10" & fileExtension, "INSERT", Nothing, Nothing, Nothing, "L10", group)

                cmd.ExecuteNonQuery()

            End If

        End If

    End Sub

    Private Sub AccessDatabaseSQL2005_ProductStockLoad_SetParms(ByRef cmdParameters As SqlParameterCollection, ByVal file As String, ByVal type As String, ByVal product As DEStockProduct)
        Select Case file
            Case Is = "tbl_product_stock", "tbl_product_stock_work"
                Select Case type
                    Case Is = "UPDATE", "INSERT"
                        With cmdParameters
                            .Add(New SqlParameter("@PRODUCT", SqlDbType.NVarChar)).Value = product.SKU
                            .Add(New SqlParameter("@STOCK_LOCATION", SqlDbType.NVarChar)).Value = product.QuantityWarehouse
                            .Add(New SqlParameter("@QUANTITY", SqlDbType.Int)).Value = product.Quantity
                            .Add(New SqlParameter("@ALLOCATED_QUANTITY", SqlDbType.NVarChar)).Value = 0
                            .Add(New SqlParameter("@AVAILABLE_QUANTITY", SqlDbType.NVarChar)).Value = product.Quantity
                            .Add(New SqlParameter("@RESTOCK_CODE", SqlDbType.NVarChar)).Value = product.QuantityReStockCode
                            .Add(New SqlParameter("@WAREHOUSE", SqlDbType.NVarChar)).Value = ""
                        End With
                End Select
        End Select
    End Sub

    Private Sub AccessDatabaseSQL2005_ProductNavigationLoad_SetParms(ByRef cmdParameters As SqlParameterCollection,
                                                                     ByVal file As String,
                                                                     ByVal type As String,
                                                                     Optional ByVal productGroup As DEProductGroup = Nothing,
                                                                     Optional ByVal groupDetails As DEProductGroupDetails = Nothing,
                                                                     Optional ByVal productGroupHierarchy As DEProductGroupHierarchy = Nothing,
                                                                     Optional ByVal level As String = "",
                                                                     Optional ByVal levelGroup As DEProductGroupHierarchyGroup = Nothing)

        Dim intLevel As Integer = 0
        If level <> String.Empty Then
            intLevel = CInt(level.Substring(1))
        End If

        cmdParameters.Clear()
        Select Case file
            Case Is = "tbl_group", "tbl_group_work"
                Select Case type
                    Case Is = "UPDATE", "INSERT"
                        With cmdParameters
                            .Add(New SqlParameter("@GROUP_NAME", SqlDbType.NVarChar)).Value = productGroup.Code
                            .Add(New SqlParameter("@GROUP_DESCRIPTION_1", SqlDbType.NVarChar)).Value = groupDetails.Description1
                            .Add(New SqlParameter("@GROUP_DESCRIPTION_2", SqlDbType.NVarChar)).Value = groupDetails.Description2
                            .Add(New SqlParameter("@GROUP_HTML_1", SqlDbType.NVarChar)).Value = groupDetails.Html1
                            .Add(New SqlParameter("@GROUP_HTML_2", SqlDbType.NVarChar)).Value = groupDetails.Html2
                            .Add(New SqlParameter("@GROUP_HTML_3", SqlDbType.NVarChar)).Value = groupDetails.Html3
                            .Add(New SqlParameter("@GROUP_PAGE_TITLE", SqlDbType.NVarChar)).Value = groupDetails.PageTitle
                            .Add(New SqlParameter("@GROUP_META_DESCRIPTION", SqlDbType.NVarChar)).Value = groupDetails.MetaDescription
                            .Add(New SqlParameter("@GROUP_META_KEYWORDS", SqlDbType.NVarChar)).Value = groupDetails.MetaKeywords
                        End With
                End Select
            Case Is = "tbl_group_lang", "tbl_group_lang_work"
                Select Case type
                    Case Is = "UPDATE", "INSERT"
                        With cmdParameters
                            .Add(New SqlParameter("@GROUP_CODE", SqlDbType.NVarChar)).Value = productGroup.Code
                            .Add(New SqlParameter("@GROUP_LANGUAGE", SqlDbType.NVarChar)).Value = groupDetails.Language
                            .Add(New SqlParameter("@GROUP_DESCRIPTION_1", SqlDbType.NVarChar)).Value = groupDetails.Description1
                            .Add(New SqlParameter("@GROUP_DESCRIPTION_2", SqlDbType.NVarChar)).Value = groupDetails.Description2
                            .Add(New SqlParameter("@GROUP_HTML_1", SqlDbType.NVarChar)).Value = groupDetails.Html1
                            .Add(New SqlParameter("@GROUP_HTML_2", SqlDbType.NVarChar)).Value = groupDetails.Html2
                            .Add(New SqlParameter("@GROUP_HTML_3", SqlDbType.NVarChar)).Value = groupDetails.Html3
                            .Add(New SqlParameter("@GROUP_PAGE_TITLE", SqlDbType.NVarChar)).Value = groupDetails.PageTitle
                            .Add(New SqlParameter("@GROUP_META_DESCRIPTION", SqlDbType.NVarChar)).Value = groupDetails.MetaDescription
                            .Add(New SqlParameter("@GROUP_META_KEYWORDS", SqlDbType.NVarChar)).Value = groupDetails.MetaKeywords
                        End With
                End Select

            Case Is = "tbl_group_level_01", "tbl_group_level_01_work",
                      "tbl_group_level_02", "tbl_group_level_02_work",
                      "tbl_group_level_03", "tbl_group_level_03_work",
                      "tbl_group_level_04", "tbl_group_level_04_work",
                      "tbl_group_level_05", "tbl_group_level_05_work",
                      "tbl_group_level_06", "tbl_group_level_06_work",
                      "tbl_group_level_07", "tbl_group_level_07_work",
                      "tbl_group_level_08", "tbl_group_level_08_work",
                      "tbl_group_level_09", "tbl_group_level_09_work",
                      "tbl_group_level_10", "tbl_group_level_10_work"
                Select Case type
                    Case Is = "DELETE"
                        With cmdParameters
                            .Add(New SqlParameter("@BUSINESS_UNIT", SqlDbType.NVarChar)).Value = productGroupHierarchy.BusinessUnit
                            .Add(New SqlParameter("@PARTNER", SqlDbType.NVarChar)).Value = productGroupHierarchy.Partner
                        End With
                    Case Is = "INSERT"
                        With cmdParameters
                            .Add(New SqlParameter("@GROUP_Lxx_BUSINESS_UNIT", SqlDbType.NVarChar)).Value = levelGroup.BusinessUnit
                            .Add(New SqlParameter("@GROUP_Lxx_PARTNER", SqlDbType.NVarChar)).Value = levelGroup.Partner
                            .Add(New SqlParameter("@GROUP_Lxx_L01_GROUP", SqlDbType.NVarChar)).Value = levelGroup.L01Group
                            If intLevel >= 2 Then
                                If levelGroup.L02Group = String.Empty Then
                                    levelGroup.L02Group = "*EMPTY"
                                End If
                                .Add(New SqlParameter("@GROUP_Lxx_L02_GROUP", SqlDbType.NVarChar)).Value = levelGroup.L02Group
                            End If
                            If intLevel >= 3 Then
                                If levelGroup.L03Group = String.Empty Then
                                    levelGroup.L03Group = "*EMPTY"
                                End If
                                .Add(New SqlParameter("@GROUP_Lxx_L03_GROUP", SqlDbType.NVarChar)).Value = levelGroup.L03Group
                            End If
                            If intLevel >= 4 Then
                                If levelGroup.L04Group = String.Empty Then
                                    levelGroup.L04Group = "*EMPTY"
                                End If
                                .Add(New SqlParameter("@GROUP_Lxx_L04_GROUP", SqlDbType.NVarChar)).Value = levelGroup.L04Group
                            End If
                            If intLevel >= 5 Then
                                If levelGroup.L05Group = String.Empty Then
                                    levelGroup.L05Group = "*EMPTY"
                                End If
                                .Add(New SqlParameter("@GROUP_Lxx_L05_GROUP", SqlDbType.NVarChar)).Value = levelGroup.L05Group
                            End If
                            If intLevel >= 6 Then
                                If levelGroup.L06Group = String.Empty Then
                                    levelGroup.L06Group = "*EMPTY"
                                End If
                                .Add(New SqlParameter("@GROUP_Lxx_L06_GROUP", SqlDbType.NVarChar)).Value = levelGroup.L06Group
                            End If
                            If intLevel >= 7 Then
                                If levelGroup.L07Group = String.Empty Then
                                    levelGroup.L07Group = "*EMPTY"
                                End If
                                .Add(New SqlParameter("@GROUP_Lxx_L07_GROUP", SqlDbType.NVarChar)).Value = levelGroup.L07Group
                            End If
                            If intLevel >= 8 Then
                                If levelGroup.L08Group = String.Empty Then
                                    levelGroup.L08Group = "*EMPTY"
                                End If
                                .Add(New SqlParameter("@GROUP_Lxx_L08_GROUP", SqlDbType.NVarChar)).Value = levelGroup.L08Group
                            End If
                            If intLevel >= 9 Then
                                If levelGroup.L09Group = String.Empty Then
                                    levelGroup.L09Group = "*EMPTY"
                                End If
                                .Add(New SqlParameter("@GROUP_Lxx_L09_GROUP", SqlDbType.NVarChar)).Value = levelGroup.L09Group
                            End If
                            If intLevel >= 10 Then
                                If levelGroup.L10Group = String.Empty Then
                                    levelGroup.L10Group = "*EMPTY"
                                End If
                                .Add(New SqlParameter("@GROUP_Lxx_L10_GROUP", SqlDbType.NVarChar)).Value = levelGroup.L10Group
                            End If
                            .Add(New SqlParameter("@GROUP_Lxx_SEQUENCE", SqlDbType.NVarChar)).Value = levelGroup.DisplaySequence
                            .Add(New SqlParameter("@GROUP_Lxx_DESCRIPTION_1", SqlDbType.NVarChar)).Value = levelGroup.ProductGroupDetails.Description1
                            .Add(New SqlParameter("@GROUP_Lxx_DESCRIPTION_2", SqlDbType.NVarChar)).Value = levelGroup.ProductGroupDetails.Description2
                            .Add(New SqlParameter("@GROUP_Lxx_HTML_1", SqlDbType.NVarChar)).Value = levelGroup.ProductGroupDetails.Html1
                            .Add(New SqlParameter("@GROUP_Lxx_HTML_2", SqlDbType.NVarChar)).Value = levelGroup.ProductGroupDetails.Html2
                            .Add(New SqlParameter("@GROUP_Lxx_HTML_3", SqlDbType.NVarChar)).Value = levelGroup.ProductGroupDetails.Html3
                            .Add(New SqlParameter("@GROUP_Lxx_PAGE_TITLE", SqlDbType.NVarChar)).Value = levelGroup.ProductGroupDetails.PageTitle
                            .Add(New SqlParameter("@GROUP_Lxx_META_DESCRIPTION", SqlDbType.NVarChar)).Value = levelGroup.ProductGroupDetails.MetaDescription
                            .Add(New SqlParameter("@GROUP_Lxx_META_KEYWORDS", SqlDbType.NVarChar)).Value = levelGroup.ProductGroupDetails.MetaKeywords
                            .Add(New SqlParameter("@GROUP_Lxx_ADV_SEARCH_TEMPLATE", SqlDbType.NVarChar)).Value = levelGroup.AdvancedSearchTemplate
                            .Add(New SqlParameter("@GROUP_Lxx_PRODUCT_PAGE_TEMPLATE", SqlDbType.NVarChar)).Value = levelGroup.ProductPageTemplate
                            .Add(New SqlParameter("@GROUP_Lxx_PRODUCT_LIST_TEMPLATE", SqlDbType.NVarChar)).Value = levelGroup.ProductListTemplate
                            .Add(New SqlParameter("@GROUP_Lxx_SHOW_CHILDREN_AS_GROUPS", SqlDbType.Bit)).Value = convertToBool(levelGroup.ShowChildrenAsGroups)
                            .Add(New SqlParameter("@GROUP_Lxx_SHOW_PRODUCTS_AS_LIST", SqlDbType.Bit)).Value = convertToBool(levelGroup.ShowProductAsList)
                            .Add(New SqlParameter("@GROUP_Lxx_SHOW_IN_NAVIGATION", SqlDbType.Bit)).Value = convertToBool(levelGroup.ShowInNavigation)
                            .Add(New SqlParameter("@GROUP_Lxx_SHOW_IN_GROUPED_NAV", SqlDbType.Bit)).Value = convertToBool(levelGroup.ShowInGroupedNavigation)
                            .Add(New SqlParameter("@GROUP_Lxx_HTML_GROUP", SqlDbType.Bit)).Value = convertToBool(levelGroup.HtmlGroup)
                            .Add(New SqlParameter("@GROUP_Lxx_HTML_GROUP_TYPE", SqlDbType.NVarChar)).Value = levelGroup.HtmlGroupType
                            .Add(New SqlParameter("@GROUP_Lxx_SHOW_PRODUCT_DISPLAY", SqlDbType.Bit)).Value = convertToBool(levelGroup.ShowProductDisplay)
                            .Add(New SqlParameter("@GROUP_Lxx_THEME", SqlDbType.NVarChar)).Value = levelGroup.Theme


                        End With
                    Case Is = "SELECT"
                        With cmdParameters
                            .Add(New SqlParameter("@GROUP_Lxx_BUSINESS_UNIT", SqlDbType.NVarChar)).Value = levelGroup.BusinessUnit
                            .Add(New SqlParameter("@GROUP_Lxx_PARTNER", SqlDbType.NVarChar)).Value = levelGroup.Partner
                            .Add(New SqlParameter("@GROUP_Lxx_L01_GROUP", SqlDbType.NVarChar)).Value = levelGroup.L01Group
                            If intLevel >= 2 Then
                                .Add(New SqlParameter("@GROUP_Lxx_L02_GROUP", SqlDbType.NVarChar)).Value = levelGroup.L02Group
                            End If
                            If intLevel >= 3 Then
                                .Add(New SqlParameter("@GROUP_Lxx_L03_GROUP", SqlDbType.NVarChar)).Value = levelGroup.L03Group
                            End If
                            If intLevel >= 4 Then
                                .Add(New SqlParameter("@GROUP_Lxx_L04_GROUP", SqlDbType.NVarChar)).Value = levelGroup.L04Group
                            End If
                            If intLevel >= 5 Then
                                .Add(New SqlParameter("@GROUP_Lxx_L05_GROUP", SqlDbType.NVarChar)).Value = levelGroup.L05Group
                            End If
                            If intLevel >= 6 Then
                                .Add(New SqlParameter("@GROUP_Lxx_L06_GROUP", SqlDbType.NVarChar)).Value = levelGroup.L06Group
                            End If
                            If intLevel >= 7 Then
                                .Add(New SqlParameter("@GROUP_Lxx_L07_GROUP", SqlDbType.NVarChar)).Value = levelGroup.L07Group
                            End If
                            If intLevel >= 8 Then
                                .Add(New SqlParameter("@GROUP_Lxx_L08_GROUP", SqlDbType.NVarChar)).Value = levelGroup.L08Group
                            End If
                            If intLevel >= 9 Then
                                .Add(New SqlParameter("@GROUP_Lxx_L09_GROUP", SqlDbType.NVarChar)).Value = levelGroup.L09Group
                            End If
                            If intLevel >= 10 Then
                                .Add(New SqlParameter("@GROUP_Lxx_L10_GROUP", SqlDbType.NVarChar)).Value = levelGroup.L10Group
                            End If
                        End With

                    Case Is = "UPDATE"
                        With cmdParameters
                            .Add(New SqlParameter("@GROUP_Lxx_BUSINESS_UNIT", SqlDbType.NVarChar)).Value = levelGroup.BusinessUnit
                            .Add(New SqlParameter("@GROUP_Lxx_PARTNER", SqlDbType.NVarChar)).Value = levelGroup.Partner
                            .Add(New SqlParameter("@GROUP_Lxx_L01_GROUP", SqlDbType.NVarChar)).Value = levelGroup.L01Group
                            If intLevel >= 2 Then
                                .Add(New SqlParameter("@GROUP_Lxx_L02_GROUP", SqlDbType.NVarChar)).Value = levelGroup.L02Group
                            End If
                            If intLevel >= 3 Then
                                .Add(New SqlParameter("@GROUP_Lxx_L03_GROUP", SqlDbType.NVarChar)).Value = levelGroup.L03Group
                            End If
                            If intLevel >= 4 Then
                                .Add(New SqlParameter("@GROUP_Lxx_L04_GROUP", SqlDbType.NVarChar)).Value = levelGroup.L04Group
                            End If
                            If intLevel >= 5 Then
                                .Add(New SqlParameter("@GROUP_Lxx_L05_GROUP", SqlDbType.NVarChar)).Value = levelGroup.L05Group
                            End If
                            If intLevel >= 6 Then
                                .Add(New SqlParameter("@GROUP_Lxx_L06_GROUP", SqlDbType.NVarChar)).Value = levelGroup.L06Group
                            End If
                            If intLevel >= 7 Then
                                .Add(New SqlParameter("@GROUP_Lxx_L07_GROUP", SqlDbType.NVarChar)).Value = levelGroup.L07Group
                            End If
                            If intLevel >= 8 Then
                                .Add(New SqlParameter("@GROUP_Lxx_L08_GROUP", SqlDbType.NVarChar)).Value = levelGroup.L08Group
                            End If
                            If intLevel >= 9 Then
                                .Add(New SqlParameter("@GROUP_Lxx_L09_GROUP", SqlDbType.NVarChar)).Value = levelGroup.L09Group
                            End If
                            If intLevel >= 10 Then
                                .Add(New SqlParameter("@GROUP_Lxx_L10_GROUP", SqlDbType.NVarChar)).Value = levelGroup.L10Group
                            End If
                            .Add(New SqlParameter("@GROUP_Lxx_SEQUENCE", SqlDbType.NVarChar)).Value = levelGroup.DisplaySequence
                            .Add(New SqlParameter("@GROUP_Lxx_DESCRIPTION_1", SqlDbType.NVarChar)).Value = levelGroup.ProductGroupDetails.Description1
                            .Add(New SqlParameter("@GROUP_Lxx_DESCRIPTION_2", SqlDbType.NVarChar)).Value = levelGroup.ProductGroupDetails.Description2
                            .Add(New SqlParameter("@GROUP_Lxx_HTML_1", SqlDbType.NVarChar)).Value = levelGroup.ProductGroupDetails.Html1
                            .Add(New SqlParameter("@GROUP_Lxx_HTML_2", SqlDbType.NVarChar)).Value = levelGroup.ProductGroupDetails.Html2
                            .Add(New SqlParameter("@GROUP_Lxx_HTML_3", SqlDbType.NVarChar)).Value = levelGroup.ProductGroupDetails.Html3
                            .Add(New SqlParameter("@GROUP_Lxx_PAGE_TITLE", SqlDbType.NVarChar)).Value = levelGroup.ProductGroupDetails.PageTitle
                            .Add(New SqlParameter("@GROUP_Lxx_META_DESCRIPTION", SqlDbType.NVarChar)).Value = levelGroup.ProductGroupDetails.MetaDescription
                            .Add(New SqlParameter("@GROUP_Lxx_META_KEYWORDS", SqlDbType.NVarChar)).Value = levelGroup.ProductGroupDetails.MetaKeywords
                            .Add(New SqlParameter("@GROUP_Lxx_ADV_SEARCH_TEMPLATE", SqlDbType.NVarChar)).Value = levelGroup.AdvancedSearchTemplate
                            .Add(New SqlParameter("@GROUP_Lxx_PRODUCT_PAGE_TEMPLATE", SqlDbType.NVarChar)).Value = levelGroup.ProductPageTemplate
                            .Add(New SqlParameter("@GROUP_Lxx_PRODUCT_LIST_TEMPLATE", SqlDbType.NVarChar)).Value = levelGroup.ProductListTemplate
                            .Add(New SqlParameter("@GROUP_Lxx_SHOW_CHILDREN_AS_GROUPS", SqlDbType.Bit)).Value = convertToBool(levelGroup.ShowChildrenAsGroups)
                            .Add(New SqlParameter("@GROUP_Lxx_SHOW_PRODUCTS_AS_LIST", SqlDbType.Bit)).Value = convertToBool(levelGroup.ShowProductAsList)
                            .Add(New SqlParameter("@GROUP_Lxx_SHOW_IN_NAVIGATION", SqlDbType.Bit)).Value = convertToBool(levelGroup.ShowInNavigation)
                            .Add(New SqlParameter("@GROUP_Lxx_SHOW_IN_GROUPED_NAV", SqlDbType.Bit)).Value = convertToBool(levelGroup.ShowInGroupedNavigation)
                            .Add(New SqlParameter("@GROUP_Lxx_HTML_GROUP", SqlDbType.Bit)).Value = convertToBool(levelGroup.HtmlGroup)
                            .Add(New SqlParameter("@GROUP_Lxx_HTML_GROUP_TYPE", SqlDbType.NVarChar)).Value = levelGroup.HtmlGroupType
                            .Add(New SqlParameter("@GROUP_Lxx_SHOW_PRODUCT_DISPLAY", SqlDbType.Bit)).Value = convertToBool(levelGroup.ShowProductDisplay)
                            .Add(New SqlParameter("@GROUP_Lxx_THEME", SqlDbType.NVarChar)).Value = levelGroup.Theme

                        End With
                    Case Is = "DELETE2"
                        With cmdParameters
                            .Add(New SqlParameter("@GROUP_Lxx_BUSINESS_UNIT", SqlDbType.NVarChar)).Value = levelGroup.BusinessUnit
                            .Add(New SqlParameter("@GROUP_Lxx_PARTNER", SqlDbType.NVarChar)).Value = levelGroup.Partner
                            .Add(New SqlParameter("@GROUP_Lxx_L01_GROUP", SqlDbType.NVarChar)).Value = levelGroup.L01Group
                            If intLevel >= 2 Then
                                .Add(New SqlParameter("@GROUP_Lxx_L02_GROUP", SqlDbType.NVarChar)).Value = levelGroup.L02Group
                            End If
                            If intLevel >= 3 Then
                                .Add(New SqlParameter("@GROUP_Lxx_L03_GROUP", SqlDbType.NVarChar)).Value = levelGroup.L03Group
                            End If
                            If intLevel >= 4 Then
                                .Add(New SqlParameter("@GROUP_Lxx_L04_GROUP", SqlDbType.NVarChar)).Value = levelGroup.L04Group
                            End If
                            If intLevel >= 5 Then
                                .Add(New SqlParameter("@GROUP_Lxx_L05_GROUP", SqlDbType.NVarChar)).Value = levelGroup.L05Group
                            End If
                            If intLevel >= 6 Then
                                .Add(New SqlParameter("@GROUP_Lxx_L06_GROUP", SqlDbType.NVarChar)).Value = levelGroup.L06Group
                            End If
                            If intLevel >= 7 Then
                                .Add(New SqlParameter("@GROUP_Lxx_L07_GROUP", SqlDbType.NVarChar)).Value = levelGroup.L07Group
                            End If
                            If intLevel >= 8 Then
                                .Add(New SqlParameter("@GROUP_Lxx_L08_GROUP", SqlDbType.NVarChar)).Value = levelGroup.L08Group
                            End If
                            If intLevel >= 9 Then
                                .Add(New SqlParameter("@GROUP_Lxx_L09_GROUP", SqlDbType.NVarChar)).Value = levelGroup.L09Group
                            End If
                            If intLevel >= 10 Then
                                .Add(New SqlParameter("@GROUP_Lxx_L10_GROUP", SqlDbType.NVarChar)).Value = levelGroup.L10Group
                            End If
                        End With
                End Select
            Case Is = "tbl_group_product"
                Select Case type
                    Case Is = "DELETE"

                        With cmdParameters
                            .Add(New SqlParameter("@GROUP_BUSINESS_UNIT", SqlDbType.NVarChar)).Value = levelGroup.BusinessUnit
                            .Add(New SqlParameter("@GROUP_PARTNER", SqlDbType.NVarChar)).Value = levelGroup.Partner
                            .Add(New SqlParameter("@GROUP_L01_GROUP", SqlDbType.NVarChar)).Value = levelGroup.L01Group
                            If intLevel >= 2 Then
                                .Add(New SqlParameter("@GROUP_L02_GROUP", SqlDbType.NVarChar)).Value = levelGroup.L02Group
                            End If
                            If intLevel >= 3 Then
                                .Add(New SqlParameter("@GROUP_L03_GROUP", SqlDbType.NVarChar)).Value = levelGroup.L03Group
                            End If
                            If intLevel >= 4 Then
                                .Add(New SqlParameter("@GROUP_L04_GROUP", SqlDbType.NVarChar)).Value = levelGroup.L04Group
                            End If
                            If intLevel >= 5 Then
                                .Add(New SqlParameter("@GROUP_L05_GROUP", SqlDbType.NVarChar)).Value = levelGroup.L05Group
                            End If
                            If intLevel >= 6 Then
                                .Add(New SqlParameter("@GROUP_L06_GROUP", SqlDbType.NVarChar)).Value = levelGroup.L06Group
                            End If
                            If intLevel >= 7 Then
                                .Add(New SqlParameter("@GROUP_L07_GROUP", SqlDbType.NVarChar)).Value = levelGroup.L07Group
                            End If
                            If intLevel >= 8 Then
                                .Add(New SqlParameter("@GROUP_L08_GROUP", SqlDbType.NVarChar)).Value = levelGroup.L08Group
                            End If
                            If intLevel >= 9 Then
                                .Add(New SqlParameter("@GROUP_L09_GROUP", SqlDbType.NVarChar)).Value = levelGroup.L09Group
                            End If
                            If intLevel >= 10 Then
                                .Add(New SqlParameter("@GROUP_L10_GROUP", SqlDbType.NVarChar)).Value = levelGroup.L10Group
                            End If
                        End With

                End Select


        End Select

    End Sub
    Private Function AccessDatabaseSQL2005_ProductNavigationLoad_BuildSql(ByVal file As String,
                                                                          ByVal type As String,
                                                                          ByVal level As String,
                                                                          ByVal fields As Collection)
        Dim sb As New StringBuilder
        Dim count As Integer = 0
        Dim intLevel As Integer = CInt(level.Substring(1))
        Dim includeField As Boolean = False
        Select Case type
            Case Is = "INSERT"
                Select Case file
                    Case Is = "tbl_group_level_01_work", "tbl_group_level_01",
                                "tbl_group_level_02_work", "tbl_group_level_02",
                                "tbl_group_level_03_work", "tbl_group_level_03",
                                "tbl_group_level_04_work", "tbl_group_level_04",
                                "tbl_group_level_05_work", "tbl_group_level_05",
                                "tbl_group_level_06_work", "tbl_group_level_06",
                                "tbl_group_level_07_work", "tbl_group_level_07",
                                "tbl_group_level_08_work", "tbl_group_level_08",
                                "tbl_group_level_09_work", "tbl_group_level_09",
                                "tbl_group_level_10_work", "tbl_group_level_10"
                        With sb
                            .Append("INSERT INTO  ")
                            .Append(file)
                            .Append(" ( ")
                            count = 0
                            For Each field As String In fields
                                count += 1
                                includeField = True
                                '-------------------------------
                                ' Only insert appropriate groups
                                '-------------------------------
                                If (field = "GROUP_Lxx_L02_GROUP" AndAlso intLevel < 2) OrElse
                                   (field = "GROUP_Lxx_L03_GROUP" AndAlso intLevel < 3) OrElse
                                   (field = "GROUP_Lxx_L04_GROUP" AndAlso intLevel < 4) OrElse
                                   (field = "GROUP_Lxx_L05_GROUP" AndAlso intLevel < 5) OrElse
                                   (field = "GROUP_Lxx_L06_GROUP" AndAlso intLevel < 6) OrElse
                                   (field = "GROUP_Lxx_L07_GROUP" AndAlso intLevel < 7) OrElse
                                   (field = "GROUP_Lxx_L08_GROUP" AndAlso intLevel < 8) OrElse
                                   (field = "GROUP_Lxx_L09_GROUP" AndAlso intLevel < 9) OrElse
                                   (field = "GROUP_Lxx_L10_GROUP" AndAlso intLevel < 10) Then

                                    includeField = False
                                End If
                                If includeField Then

                                    .Append((Replace(field, "Lxx", level)))
                                    If count < fields.Count Then
                                        .Append(",")
                                    End If
                                End If

                            Next
                            .Append(") VALUES (")
                            count = 0
                            For Each field As String In fields
                                count += 1
                                includeField = True
                                '-------------------------------
                                ' Only insert appropriate groups
                                '-------------------------------
                                If (field = "GROUP_Lxx_L02_GROUP" AndAlso intLevel < 2) OrElse
                                   (field = "GROUP_Lxx_L03_GROUP" AndAlso intLevel < 3) OrElse
                                   (field = "GROUP_Lxx_L04_GROUP" AndAlso intLevel < 4) OrElse
                                   (field = "GROUP_Lxx_L05_GROUP" AndAlso intLevel < 5) OrElse
                                   (field = "GROUP_Lxx_L06_GROUP" AndAlso intLevel < 6) OrElse
                                   (field = "GROUP_Lxx_L07_GROUP" AndAlso intLevel < 7) OrElse
                                   (field = "GROUP_Lxx_L08_GROUP" AndAlso intLevel < 8) OrElse
                                   (field = "GROUP_Lxx_L09_GROUP" AndAlso intLevel < 9) OrElse
                                   (field = "GROUP_Lxx_L10_GROUP" AndAlso intLevel < 10) Then

                                    includeField = False
                                End If
                                If includeField Then

                                    .Append("@").Append(field)
                                    If count < fields.Count Then
                                        .Append(",")
                                    End If
                                End If

                            Next
                            .Append(")")
                        End With
                End Select

            Case Is = "SELECT"
                Select Case file
                    Case Is = "tbl_group_level_01_work", "tbl_group_level_01",
                                   "tbl_group_level_02_work", "tbl_group_level_02",
                                   "tbl_group_level_03_work", "tbl_group_level_03",
                                   "tbl_group_level_04_work", "tbl_group_level_04",
                                   "tbl_group_level_05_work", "tbl_group_level_05",
                                   "tbl_group_level_06_work", "tbl_group_level_06",
                                   "tbl_group_level_07_work", "tbl_group_level_07",
                                   "tbl_group_level_08_work", "tbl_group_level_08",
                                   "tbl_group_level_09_work", "tbl_group_level_09",
                                   "tbl_group_level_10_work", "tbl_group_level_10"
                        With sb
                            .Append("SELECT * FROM ")
                            .Append(file)
                            .Append(" WHERE ")
                            .Append(Replace("GROUP_Lxx_BUSINESS_UNIT", "Lxx", level)).Append(" = @GROUP_Lxx_BUSINESS_UNIT")
                            .Append(" AND ")
                            .Append(Replace("GROUP_Lxx_PARTNER", "Lxx", level)).Append(" = @GROUP_Lxx_PARTNER")
                            .Append(" AND ")
                            .Append(Replace("GROUP_Lxx_L01_GROUP", "Lxx", level)).Append(" = @GROUP_Lxx_L01_GROUP")
                            If intLevel >= 2 Then
                                .Append(" AND ")
                                .Append(Replace("GROUP_Lxx_L02_GROUP", "Lxx", level)).Append(" = @GROUP_Lxx_L02_GROUP")
                            End If
                            If intLevel >= 3 Then
                                .Append(" AND ")
                                .Append(Replace("GROUP_Lxx_L03_GROUP", "Lxx", level)).Append(" = @GROUP_Lxx_L03_GROUP")
                            End If
                            If intLevel >= 4 Then
                                .Append(" AND ")
                                .Append(Replace("GROUP_Lxx_L04_GROUP", "Lxx", level)).Append(" = @GROUP_Lxx_L04_GROUP")
                            End If
                            If intLevel >= 5 Then
                                .Append(" AND ")
                                .Append(Replace("GROUP_Lxx_L05_GROUP", "Lxx", level)).Append(" = @GROUP_Lxx_L05_GROUP")
                            End If
                            If intLevel >= 6 Then
                                .Append(" AND ")
                                .Append(Replace("GROUP_Lxx_L06_GROUP", "Lxx", level)).Append(" = @GROUP_Lxx_L06_GROUP")
                            End If
                            If intLevel >= 7 Then
                                .Append(" AND ")
                                .Append(Replace("GROUP_Lxx_L07_GROUP", "Lxx", level)).Append(" = @GROUP_Lxx_L07_GROUP")
                            End If
                            If intLevel >= 8 Then
                                .Append(" AND ")
                                .Append(Replace("GROUP_Lxx_L08_GROUP", "Lxx", level)).Append(" = @GROUP_Lxx_L08_GROUP")
                            End If
                            If intLevel >= 9 Then
                                .Append(" AND ")
                                .Append(Replace("GROUP_Lxx_L09_GROUP", "Lxx", level)).Append(" = @GROUP_Lxx_L09_GROUP")
                            End If
                            If intLevel >= 10 Then
                                .Append(" AND ")
                                .Append(Replace("GROUP_Lxx_L10_GROUP", "Lxx", level)).Append(" = @GROUP_Lxx_L10_GROUP")
                            End If

                        End With
                End Select
            Case Is = "UPDATE"
                Select Case file
                    Case Is = "tbl_group_level_01_work", "tbl_group_level_01",
                                   "tbl_group_level_02_work", "tbl_group_level_02",
                                   "tbl_group_level_03_work", "tbl_group_level_03",
                                   "tbl_group_level_04_work", "tbl_group_level_04",
                                   "tbl_group_level_05_work", "tbl_group_level_05",
                                   "tbl_group_level_06_work", "tbl_group_level_06",
                                   "tbl_group_level_07_work", "tbl_group_level_07",
                                   "tbl_group_level_08_work", "tbl_group_level_08",
                                   "tbl_group_level_09_work", "tbl_group_level_09",
                                   "tbl_group_level_10_work", "tbl_group_level_10"
                        With sb
                            .Append("UPDATE ").Append(file).Append(" SET ")
                            .Append(Replace(" GROUP_Lxx_SEQUENCE = ", "Lxx", level)).Append("@GROUP_Lxx_SEQUENCE,")
                            .Append(Replace(" GROUP_Lxx_ADV_SEARCH_TEMPLATE = ", "Lxx", level)).Append("@GROUP_Lxx_ADV_SEARCH_TEMPLATE,")
                            .Append(Replace(" GROUP_Lxx_PRODUCT_PAGE_TEMPLATE = ", "Lxx", level)).Append("@GROUP_Lxx_PRODUCT_PAGE_TEMPLATE,")
                            .Append(Replace(" GROUP_Lxx_PRODUCT_LIST_TEMPLATE= ", "Lxx", level)).Append("@GROUP_Lxx_PRODUCT_LIST_TEMPLATE,")
                            .Append(Replace(" GROUP_Lxx_SHOW_CHILDREN_AS_GROUPS = ", "Lxx", level)).Append("@GROUP_Lxx_SHOW_CHILDREN_AS_GROUPS,")
                            .Append(Replace(" GROUP_Lxx_SHOW_PRODUCTS_AS_LIST = ", "Lxx", level)).Append("@GROUP_Lxx_SHOW_PRODUCTS_AS_LIST,")
                            .Append(Replace(" GROUP_Lxx_SHOW_IN_NAVIGATION = ", "Lxx", level)).Append("@GROUP_Lxx_SHOW_IN_NAVIGATION,")
                            .Append(Replace(" GROUP_Lxx_HTML_GROUP = ", "Lxx", level)).Append("@GROUP_Lxx_HTML_GROUP,")
                            .Append(Replace(" GROUP_Lxx_HTML_GROUP_TYPE = ", "Lxx", level)).Append("@GROUP_Lxx_HTML_GROUP_TYPE,")
                            .Append(Replace(" GROUP_Lxx_SHOW_PRODUCT_DISPLAY = ", "Lxx", level)).Append("@GROUP_Lxx_SHOW_PRODUCT_DISPLAY, ")
                            .Append(Replace(" GROUP_Lxx_THEME = ", "Lxx", level)).Append("@GROUP_Lxx_THEME ")
                            .Append(" WHERE ")
                            .Append(Replace("GROUP_Lxx_BUSINESS_UNIT", "Lxx", level)).Append(" = @GROUP_Lxx_BUSINESS_UNIT")
                            .Append(" AND ")
                            .Append(Replace("GROUP_Lxx_PARTNER", "Lxx", level)).Append(" = @GROUP_Lxx_PARTNER")
                            .Append(" AND ")
                            .Append(Replace("GROUP_Lxx_L01_GROUP", "Lxx", level)).Append(" = @GROUP_Lxx_L01_GROUP")
                            If intLevel >= 2 Then
                                .Append(" AND ")
                                .Append(Replace("GROUP_Lxx_L02_GROUP", "Lxx", level)).Append(" = @GROUP_Lxx_L02_GROUP")
                            End If
                            If intLevel >= 3 Then
                                .Append(" AND ")
                                .Append(Replace("GROUP_Lxx_L03_GROUP", "Lxx", level)).Append(" = @GROUP_Lxx_L03_GROUP")
                            End If
                            If intLevel >= 4 Then
                                .Append(" AND ")
                                .Append(Replace("GROUP_Lxx_L04_GROUP", "Lxx", level)).Append(" = @GROUP_Lxx_L04_GROUP")
                            End If
                            If intLevel >= 5 Then
                                .Append(" AND ")
                                .Append(Replace("GROUP_Lxx_L05_GROUP", "Lxx", level)).Append(" = @GROUP_Lxx_L05_GROUP")
                            End If
                            If intLevel >= 6 Then
                                .Append(" AND ")
                                .Append(Replace("GROUP_Lxx_L06_GROUP", "Lxx", level)).Append(" = @GROUP_Lxx_L06_GROUP")
                            End If
                            If intLevel >= 7 Then
                                .Append(" AND ")
                                .Append(Replace("GROUP_Lxx_L07_GROUP", "Lxx", level)).Append(" = @GROUP_Lxx_L07_GROUP")
                            End If
                            If intLevel >= 8 Then
                                .Append(" AND ")
                                .Append(Replace("GROUP_Lxx_L08_GROUP", "Lxx", level)).Append(" = @GROUP_Lxx_L08_GROUP")
                            End If
                            If intLevel >= 9 Then
                                .Append(" AND ")
                                .Append(Replace("GROUP_Lxx_L09_GROUP", "Lxx", level)).Append(" = @GROUP_Lxx_L09_GROUP")
                            End If
                            If intLevel >= 10 Then
                                .Append(" AND ")
                                .Append(Replace("GROUP_Lxx_L10_GROUP", "Lxx", level)).Append(" = @GROUP_Lxx_L10_GROUP")
                            End If

                        End With
                End Select
            Case Is = "DELETE"
                Select Case file
                    Case Is = "tbl_group_product"
                        With sb
                            .Append("DELETE FROM TBL_GROUP_PRODUCT WHERE ")
                            .Append(" GROUP_BUSINESS_UNIT = @GROUP_BUSINESS_UNIT AND")
                            .Append(" GROUP_PARTNER = @GROUP_PARTNER AND ")
                            .Append(" GROUP_L01_GROUP = @GROUP_L01_GROUP  ")
                            If intLevel >= 2 Then
                                .Append(" AND GROUP_L02_GROUP = @GROUP_L02_GROUP  ")
                            End If
                            If intLevel >= 3 Then
                                .Append(" AND GROUP_L03_GROUP = @GROUP_L03_GROUP ")
                            End If
                            If intLevel >= 3 Then
                                .Append(" AND GROUP_L04_GROUP = @GROUP_L04_GROUP ")
                            End If
                            If intLevel >= 3 Then
                                .Append(" AND GROUP_L05_GROUP = @GROUP_L05_GROUP ")
                            End If
                            If intLevel >= 3 Then
                                .Append(" AND GROUP_L06_GROUP = @GROUP_L06_GROUP ")
                            End If
                            If intLevel >= 3 Then
                                .Append(" AND GROUP_L07_GROUP = @GROUP_L07_GROUP ")
                            End If
                            If intLevel >= 3 Then
                                .Append(" AND GROUP_L08_GROUP = @GROUP_L08_GROUP ")
                            End If
                            If intLevel >= 3 Then
                                .Append(" AND GROUP_L09_GROUP = @GROUP_L09_GROUP ")
                            End If
                            If intLevel >= 3 Then
                                .Append(" AND GROUP_L10_GROUP = @GROUP_L10_GROUP ")
                            End If
                        End With
                End Select

            Case Is = "DELETE2"
                Select Case file
                    Case Is = "tbl_group_level_01_work", "tbl_group_level_01",
                                   "tbl_group_level_02_work", "tbl_group_level_02",
                                   "tbl_group_level_03_work", "tbl_group_level_03",
                                   "tbl_group_level_04_work", "tbl_group_level_04",
                                   "tbl_group_level_05_work", "tbl_group_level_05",
                                   "tbl_group_level_06_work", "tbl_group_level_06",
                                   "tbl_group_level_07_work", "tbl_group_level_07",
                                   "tbl_group_level_08_work", "tbl_group_level_08",
                                   "tbl_group_level_09_work", "tbl_group_level_09",
                                   "tbl_group_level_10_work", "tbl_group_level_10"
                        With sb
                            .Append("DELETE FROM ")
                            .Append(file)
                            .Append(" WHERE ")
                            .Append(Replace("GROUP_Lxx_BUSINESS_UNIT", "Lxx", level)).Append(" = @GROUP_Lxx_BUSINESS_UNIT")
                            .Append(" AND ")
                            .Append(Replace("GROUP_Lxx_PARTNER", "Lxx", level)).Append(" = @GROUP_Lxx_PARTNER")
                            .Append(" AND ")
                            .Append(Replace("GROUP_Lxx_L01_GROUP", "Lxx", level)).Append(" = @GROUP_Lxx_L01_GROUP")
                            If intLevel >= 2 Then
                                .Append(" AND ")
                                .Append(Replace("GROUP_Lxx_L02_GROUP", "Lxx", level)).Append(" = @GROUP_Lxx_L02_GROUP")
                            End If
                            If intLevel >= 3 Then
                                .Append(" AND ")
                                .Append(Replace("GROUP_Lxx_L03_GROUP", "Lxx", level)).Append(" = @GROUP_Lxx_L03_GROUP")
                            End If
                            If intLevel >= 4 Then
                                .Append(" AND ")
                                .Append(Replace("GROUP_Lxx_L04_GROUP", "Lxx", level)).Append(" = @GROUP_Lxx_L04_GROUP")
                            End If
                            If intLevel >= 5 Then
                                .Append(" AND ")
                                .Append(Replace("GROUP_Lxx_L05_GROUP", "Lxx", level)).Append(" = @GROUP_Lxx_L05_GROUP")
                            End If
                            If intLevel >= 6 Then
                                .Append(" AND ")
                                .Append(Replace("GROUP_Lxx_L06_GROUP", "Lxx", level)).Append(" = @GROUP_Lxx_L06_GROUP")
                            End If
                            If intLevel >= 7 Then
                                .Append(" AND ")
                                .Append(Replace("GROUP_Lxx_L07_GROUP", "Lxx", level)).Append(" = @GROUP_Lxx_L07_GROUP")
                            End If
                            If intLevel >= 8 Then
                                .Append(" AND ")
                                .Append(Replace("GROUP_Lxx_L08_GROUP", "Lxx", level)).Append(" = @GROUP_Lxx_L08_GROUP")
                            End If
                            If intLevel >= 9 Then
                                .Append(" AND ")
                                .Append(Replace("GROUP_Lxx_L09_GROUP", "Lxx", level)).Append(" = @GROUP_Lxx_L09_GROUP")
                            End If
                            If intLevel >= 10 Then
                                .Append(" AND ")
                                .Append(Replace("GROUP_Lxx_L10_GROUP", "Lxx", level)).Append(" = @GROUP_Lxx_L10_GROUP")
                            End If

                        End With

                End Select

        End Select

        Return sb.ToString
    End Function
    Private Function convertToBool(ByVal str As String) As Boolean
        Dim returnVal As Boolean
        If str = "1" OrElse str = "Y" OrElse str.ToLower = "true" Then
            returnVal = True
        Else
            returnVal = False
        End If

        Return returnVal

    End Function

    Private Sub AccessDatabaseSQL2005_ProductNavigationLoad_ProcessGroup(ByVal group As DEProductGroupHierarchyGroup, ByVal intLevel As Integer, ByVal productsAfterLevel As Integer)
        Dim strLevel As String = String.Empty
        Dim strTable As String = String.Empty
        Dim strSelect As String = String.Empty
        Dim strUpdateBuild As String = String.Empty
        Dim strInsertBuild As String = String.Empty
        Dim strDeleteBuild As String = String.Empty
        Dim groupExists As Boolean = False
        Dim dtrSelect As SqlDataReader
        Dim cmd As SqlCommand
        Select Case intLevel
            Case Is = 1
                strLevel = "L01"
                strTable = "tbl_group_level_01"
            Case Is = 2
                strLevel = "L02"
                strTable = "tbl_group_level_02"
            Case Is = 3
                strLevel = "L03"
                strTable = "tbl_group_level_03"
            Case Is = 4
                strLevel = "L04"
                strTable = "tbl_group_level_04"
            Case Is = 5
                strLevel = "L05"
                strTable = "tbl_group_level_05"
            Case Is = 6
                strLevel = "L06"
                strTable = "tbl_group_level_06"
            Case Is = 7
                strLevel = "L07"
                strTable = "tbl_group_level_07"
            Case Is = 8
                strLevel = "L08"
                strTable = "tbl_group_level_08"
            Case Is = 9
                strLevel = "L09"
                strTable = "tbl_group_level_09"
            Case Is = 10
                strLevel = "L10"
                strTable = "tbl_group_level_10"
        End Select
        Select Case group.Mode
            Case Is = "ADD"
                '------------------------
                ' Check if already exists
                '------------------------
                strSelect = AccessDatabaseSQL2005_ProductNavigationLoad_BuildSql(strTable, "SELECT", strLevel, Nothing)
                cmd = New SqlCommand(strSelect, conSql2005)
                AccessDatabaseSQL2005_ProductNavigationLoad_SetParms(cmd.Parameters, strTable, "SELECT", Nothing, Nothing, Nothing, strLevel, group)

                dtrSelect = cmd.ExecuteReader

                If dtrSelect.HasRows Then
                    groupExists = True
                End If

                dtrSelect.Close()

                If Not groupExists Then
                    strInsertBuild = AccessDatabaseSQL2005_ProductNavigationLoad_BuildSql(strTable, "INSERT", strLevel, groupFields)
                    cmd = New SqlCommand(strInsertBuild, conSql2005)
                    AccessDatabaseSQL2005_ProductNavigationLoad_SetParms(cmd.Parameters, strTable, "INSERT", Nothing, Nothing, Nothing, strLevel, group)

                    cmd.ExecuteNonQuery()
                    '------------------------
                    ' Add associated products
                    '------------------------
                    AccessDatabaseSQL2005_ProductNavigationLoad_ProcessProducts(group, "tbl_group_product", productsAfterLevel)
                    '-------------------------
                    ' Add Empty Product groups
                    '-------------------------
                    AccessDatabaseSQL2005_ProductNavigationLoad_BuildEmptyGroups(group, intLevel, productsAfterLevel, "")
                Else
                    ' do the products anyway
                    AccessDatabaseSQL2005_ProductNavigationLoad_ProcessProducts(group, "tbl_group_product", productsAfterLevel)

                End If

            Case Is = "UPDATE"
                '------------------------
                ' Check it already exists
                '------------------------
                strSelect = AccessDatabaseSQL2005_ProductNavigationLoad_BuildSql(strTable, "SELECT", strLevel, Nothing)
                cmd = New SqlCommand(strSelect, conSql2005)
                AccessDatabaseSQL2005_ProductNavigationLoad_SetParms(cmd.Parameters, strTable, "SELECT", Nothing, Nothing, Nothing, strLevel, group)

                dtrSelect = cmd.ExecuteReader

                If dtrSelect.HasRows Then
                    groupExists = True
                End If
                dtrSelect.Close()

                If groupExists Then
                    strUpdateBuild = AccessDatabaseSQL2005_ProductNavigationLoad_BuildSql(strTable, "UPDATE", strLevel, Nothing)
                    cmd = New SqlCommand(strUpdateBuild, conSql2005)
                    AccessDatabaseSQL2005_ProductNavigationLoad_SetParms(cmd.Parameters, strTable, "UPDATE", Nothing, Nothing, Nothing, strLevel, group)

                    cmd.ExecuteNonQuery()

                    '------------------------
                    ' Add associated products
                    '------------------------
                    AccessDatabaseSQL2005_ProductNavigationLoad_ProcessProducts(group, "tbl_group_product", productsAfterLevel)

                End If
            Case Is = "DELETE"
                strDeleteBuild = AccessDatabaseSQL2005_ProductNavigationLoad_BuildSql(strTable, "DELETE2", strLevel, Nothing)
                cmd = New SqlCommand(strDeleteBuild, conSql2005)
                AccessDatabaseSQL2005_ProductNavigationLoad_SetParms(cmd.Parameters, strTable, "DELETE2", Nothing, Nothing, Nothing, strLevel, group)

                cmd.ExecuteNonQuery()

                '--------------------------------------
                ' Delete any child groups (inc EMPTY's)
                '--------------------------------------
                If intLevel < 2 Then
                    AccessDatabaseSQL2005_ProductNavigationLoad_DeleteChildGroups(group, intLevel, "L02", "tbl_group_Level_02")
                End If
                If intLevel < 3 Then
                    AccessDatabaseSQL2005_ProductNavigationLoad_DeleteChildGroups(group, intLevel, "L03", "tbl_group_Level_03")
                End If
                If intLevel < 4 Then
                    AccessDatabaseSQL2005_ProductNavigationLoad_DeleteChildGroups(group, intLevel, "L04", "tbl_group_Level_04")
                End If
                If intLevel < 5 Then
                    AccessDatabaseSQL2005_ProductNavigationLoad_DeleteChildGroups(group, intLevel, "L05", "tbl_group_Level_05")
                End If
                If intLevel < 6 Then
                    AccessDatabaseSQL2005_ProductNavigationLoad_DeleteChildGroups(group, intLevel, "L06", "tbl_group_Level_06")
                End If
                If intLevel < 7 Then
                    AccessDatabaseSQL2005_ProductNavigationLoad_DeleteChildGroups(group, intLevel, "L07", "tbl_group_Level_07")
                End If
                If intLevel < 8 Then
                    AccessDatabaseSQL2005_ProductNavigationLoad_DeleteChildGroups(group, intLevel, "L08", "tbl_group_Level_08")
                End If
                If intLevel < 9 Then
                    AccessDatabaseSQL2005_ProductNavigationLoad_DeleteChildGroups(group, intLevel, "L09", "tbl_group_Level_09")
                End If
                If intLevel < 10 Then
                    AccessDatabaseSQL2005_ProductNavigationLoad_DeleteChildGroups(group, intLevel, "L10", "tbl_group_Level_10")
                End If
                '-------------------------------------
                ' Delete all associated group products
                '-------------------------------------
                If productsAfterLevel > 1 AndAlso group.L02Group = String.Empty Then
                    group.L02Group = "*EMPTY"
                End If
                If productsAfterLevel > 2 AndAlso group.L03Group = String.Empty Then
                    group.L03Group = "*EMPTY"
                End If
                If productsAfterLevel > 3 AndAlso group.L04Group = String.Empty Then
                    group.L04Group = "*EMPTY"
                End If
                If productsAfterLevel > 4 AndAlso group.L05Group = String.Empty Then
                    group.L05Group = "*EMPTY"
                End If
                If productsAfterLevel > 5 AndAlso group.L06Group = String.Empty Then
                    group.L06Group = "*EMPTY"
                End If
                If productsAfterLevel > 6 AndAlso group.L07Group = String.Empty Then
                    group.L07Group = "*EMPTY"
                End If
                If productsAfterLevel > 7 AndAlso group.L08Group = String.Empty Then
                    group.L08Group = "*EMPTY"
                End If
                If productsAfterLevel > 8 AndAlso group.L09Group = String.Empty Then
                    group.L09Group = "*EMPTY"
                End If
                If productsAfterLevel > 9 AndAlso group.L10Group = String.Empty Then
                    group.L10Group = "*EMPTY"
                End If
                strDeleteBuild = String.Empty
                strDeleteBuild = AccessDatabaseSQL2005_ProductNavigationLoad_BuildSql("tbl_group_product", "DELETE", strLevel, Nothing)
                cmd = New SqlCommand(strDeleteBuild, conSql2005)
                AccessDatabaseSQL2005_ProductNavigationLoad_SetParms(cmd.Parameters, "tbl_group_product", "DELETE", Nothing, Nothing, Nothing, strLevel, group)

                cmd.ExecuteNonQuery()

            Case Is = String.Empty
                '----------------------
                ' just process products
                '----------------------
                AccessDatabaseSQL2005_ProductNavigationLoad_ProcessProducts(group, "tbl_group_product", productsAfterLevel)
        End Select

        '----------------------------------------------
        ' Recursively process all the next level groups
        '----------------------------------------------
        For Each nextLevelgroup As DEProductGroupHierarchyGroup In group.NextLevelGroups
            Select Case intLevel
                Case Is = 1
                    nextLevelgroup.L01Group = group.L01Group
                    nextLevelgroup.L02Group = nextLevelgroup.Code
                Case Is = 2
                    nextLevelgroup.L01Group = group.L01Group
                    nextLevelgroup.L02Group = group.L02Group
                    nextLevelgroup.L03Group = nextLevelgroup.Code
                Case Is = 3
                    nextLevelgroup.L01Group = group.L01Group
                    nextLevelgroup.L02Group = group.L02Group
                    nextLevelgroup.L03Group = group.L03Group
                    nextLevelgroup.L04Group = nextLevelgroup.Code
                Case Is = 4
                    nextLevelgroup.L01Group = group.L01Group
                    nextLevelgroup.L02Group = group.L02Group
                    nextLevelgroup.L03Group = group.L03Group
                    nextLevelgroup.L04Group = group.L04Group
                    nextLevelgroup.L05Group = nextLevelgroup.Code
                Case Is = 5
                    nextLevelgroup.L01Group = group.L01Group
                    nextLevelgroup.L02Group = group.L02Group
                    nextLevelgroup.L03Group = group.L03Group
                    nextLevelgroup.L04Group = group.L04Group
                    nextLevelgroup.L05Group = group.L05Group
                    nextLevelgroup.L06Group = nextLevelgroup.Code
                Case Is = 6
                    nextLevelgroup.L01Group = group.L01Group
                    nextLevelgroup.L02Group = group.L02Group
                    nextLevelgroup.L03Group = group.L03Group
                    nextLevelgroup.L04Group = group.L04Group
                    nextLevelgroup.L05Group = group.L05Group
                    nextLevelgroup.L06Group = group.L06Group
                    nextLevelgroup.L07Group = nextLevelgroup.Code
                Case Is = 7
                    nextLevelgroup.L01Group = group.L01Group
                    nextLevelgroup.L02Group = group.L02Group
                    nextLevelgroup.L03Group = group.L03Group
                    nextLevelgroup.L04Group = group.L04Group
                    nextLevelgroup.L05Group = group.L05Group
                    nextLevelgroup.L06Group = group.L06Group
                    nextLevelgroup.L07Group = group.L07Group
                    nextLevelgroup.L08Group = nextLevelgroup.Code
                Case Is = 8
                    nextLevelgroup.L01Group = group.L01Group
                    nextLevelgroup.L02Group = group.L02Group
                    nextLevelgroup.L03Group = group.L03Group
                    nextLevelgroup.L04Group = group.L04Group
                    nextLevelgroup.L05Group = group.L05Group
                    nextLevelgroup.L06Group = group.L06Group
                    nextLevelgroup.L07Group = group.L07Group
                    nextLevelgroup.L08Group = group.L08Group
                    nextLevelgroup.L09Group = nextLevelgroup.Code
                Case Is = 9
                    nextLevelgroup.L01Group = group.L01Group
                    nextLevelgroup.L02Group = group.L02Group
                    nextLevelgroup.L03Group = group.L03Group
                    nextLevelgroup.L04Group = group.L04Group
                    nextLevelgroup.L05Group = group.L05Group
                    nextLevelgroup.L06Group = group.L06Group
                    nextLevelgroup.L07Group = group.L07Group
                    nextLevelgroup.L08Group = group.L08Group
                    nextLevelgroup.L09Group = group.L09Group
                    nextLevelgroup.L10Group = nextLevelgroup.Code
                Case Is = 10
            End Select

            nextLevelgroup.L01Group = group.L01Group
            AccessDatabaseSQL2005_ProductNavigationLoad_ProcessGroup(nextLevelgroup, intLevel + 1, productsAfterLevel)
        Next

    End Sub

    Private Sub AccessDatabaseSQL2005_ProductNavigationLoad_DeleteChildGroups(ByVal group As DEProductGroupHierarchyGroup,
                                                                              ByVal currentLevel As Integer,
                                                                              ByVal strLevel As String,
                                                                              ByVal file As String)

        Dim strDeleteBuild As String = String.Empty
        Dim cmd As SqlCommand
        Dim sb As New StringBuilder
        With sb
            .Append("DELETE FROM ").Append(file).Append(" WHERE ")
            .Append(Replace(" GROUP_Lxx_BUSINESS_UNIT", "Lxx", strLevel)).Append(" = @GROUP_Lxx_BUSINESS_UNIT AND")
            .Append(Replace(" GROUP_Lxx_PARTNER", "Lxx", strLevel)).Append(" = @GROUP_Lxx_PARTNER AND ")
            .Append(Replace(" GROUP_Lxx_L01_GROUP", "Lxx", strLevel)).Append(" = @GROUP_Lxx_L01_GROUP ")
            If currentLevel >= 2 Then
                .Append(" AND ")
                .Append(Replace("GROUP_Lxx_L02_GROUP", "Lxx", strLevel)).Append(" = @GROUP_Lxx_L02_GROUP ")
            End If
            If currentLevel >= 3 Then
                .Append(" AND ")
                .Append(Replace("GROUP_Lxx_L03_GROUP", "Lxx", strLevel)).Append(" = @GROUP_Lxx_L03_GROUP ")
            End If
            If currentLevel >= 4 Then
                .Append(" AND ")
                .Append(Replace("GROUP_Lxx_L04_GROUP", "Lxx", strLevel)).Append(" = @GROUP_Lxx_L04_GROUP ")
            End If
            If currentLevel >= 5 Then
                .Append(" AND ")
                .Append(Replace("GROUP_Lxx_L05_GROUP", "Lxx", strLevel)).Append(" = @GROUP_Lxx_L05_GROUP ")
            End If
            If currentLevel >= 6 Then
                .Append(" AND ")
                .Append(Replace("GROUP_Lxx_L06_GROUP", "Lxx", strLevel)).Append(" = @GROUP_Lxx_L06_GROUP ")
            End If
            If currentLevel >= 7 Then
                .Append(" AND ")
                .Append(Replace("GROUP_Lxx_L07_GROUP", "Lxx", strLevel)).Append(" = @GROUP_Lxx_L07_GROUP ")
            End If
            If currentLevel >= 8 Then
                .Append(" AND ")
                .Append(Replace("GROUP_Lxx_L08_GROUP", "Lxx", strLevel)).Append(" = @GROUP_Lxx_L08_GROUP ")
            End If
            If currentLevel >= 9 Then
                .Append(" AND ")
                .Append(Replace("GROUP_Lxx_L09_GROUP", "Lxx", strLevel)).Append(" = @GROUP_Lxx_L09_GROUP ")
            End If

        End With
        strDeleteBuild = sb.ToString
        cmd = New SqlCommand(strDeleteBuild, conSql2005)

        With cmd.Parameters
            .Add(New SqlParameter("@GROUP_Lxx_BUSINESS_UNIT", SqlDbType.NVarChar)).Value = group.BusinessUnit
            .Add(New SqlParameter("@GROUP_Lxx_PARTNER", SqlDbType.NVarChar)).Value = group.Partner
            .Add(New SqlParameter("@GROUP_Lxx_L01_GROUP", SqlDbType.NVarChar)).Value = group.L01Group
            If currentLevel >= 2 Then
                .Add(New SqlParameter("@GROUP_Lxx_L02_GROUP", SqlDbType.NVarChar)).Value = group.L02Group
            End If
            If currentLevel >= 3 Then
                .Add(New SqlParameter("@GROUP_Lxx_L03_GROUP", SqlDbType.NVarChar)).Value = group.L03Group
            End If
            If currentLevel >= 4 Then
                .Add(New SqlParameter("@GROUP_Lxx_L04_GROUP", SqlDbType.NVarChar)).Value = group.L04Group
            End If
            If currentLevel >= 5 Then
                .Add(New SqlParameter("@GROUP_Lxx_L05_GROUP", SqlDbType.NVarChar)).Value = group.L05Group
            End If
            If currentLevel >= 6 Then
                .Add(New SqlParameter("@GROUP_Lxx_L06_GROUP", SqlDbType.NVarChar)).Value = group.L06Group
            End If
            If currentLevel >= 7 Then
                .Add(New SqlParameter("@GROUP_Lxx_L07_GROUP", SqlDbType.NVarChar)).Value = group.L07Group
            End If
            If currentLevel >= 8 Then
                .Add(New SqlParameter("@GROUP_Lxx_L08_GROUP", SqlDbType.NVarChar)).Value = group.L08Group
            End If
            If currentLevel >= 9 Then
                .Add(New SqlParameter("@GROUP_Lxx_L09_GROUP", SqlDbType.NVarChar)).Value = group.L09Group
            End If
        End With

        cmd.ExecuteNonQuery()

    End Sub

    Private Sub AccessDatabaseSQL2005_ProductNavigationLoad_ProcessProducts(ByVal group As DEProductGroupHierarchyGroup, ByVal file As String, ByVal productsAfterLevel As Integer)
        Dim strUpdateBuild As String = String.Empty
        Dim strInsertBuild As String = String.Empty
        Dim strSelectBuild As String = String.Empty
        Dim strDeleteBuild As String = String.Empty
        Dim dtrReader As SqlDataReader
        Dim groupExists As Boolean = False
        Dim cmd As SqlCommand

        For Each product As DEProductEcommerceDetails In group.Products
            '-----------------
            ' Set empty levels 
            '-----------------
            If group.L02Group = String.Empty AndAlso productsAfterLevel > 1 Then
                group.L02Group = "*EMPTY"
            End If
            If group.L03Group = String.Empty AndAlso productsAfterLevel > 2 Then
                group.L03Group = "*EMPTY"
            End If
            If group.L04Group = String.Empty AndAlso productsAfterLevel > 3 Then
                group.L04Group = "*EMPTY"
            End If
            If group.L05Group = String.Empty AndAlso productsAfterLevel > 4 Then
                group.L05Group = "*EMPTY"
            End If
            If group.L06Group = String.Empty AndAlso productsAfterLevel > 5 Then
                group.L06Group = "*EMPTY"
            End If
            If group.L07Group = String.Empty AndAlso productsAfterLevel > 6 Then
                group.L07Group = "*EMPTY"
            End If
            If group.L08Group = String.Empty AndAlso productsAfterLevel > 7 Then
                group.L08Group = "*EMPTY"
            End If
            If group.L09Group = String.Empty AndAlso productsAfterLevel > 8 Then
                group.L09Group = "*EMPTY"
            End If
            If group.L10Group = String.Empty AndAlso productsAfterLevel > 9 Then
                group.L10Group = "*EMPTY"
            End If

            '-----------------------------------------------------------------------
            ' If we're doing a REPLACE (so using work files) then mode is always add
            '-----------------------------------------------------------------------
            If file = "tbl_group_product_work" Then
                product.Mode = "ADD"
            End If

            If product.Mode.Equals(String.Empty) Then
                product.Mode = "ADD"
            End If
            Select Case product.Mode
                Case Is = "ADD"
                    ' Check if it already exists
                    strSelectBuild = "SELECT * FROM " &
                                        file &
                                        " WHERE " &
                                          " GROUP_BUSINESS_UNIT = @GROUP_BUSINESS_UNIT AND " &
                                                         " GROUP_PARTNER = @GROUP_PARTNER AND " &
                                                         " GROUP_L01_GROUP = @GROUP_L01_GROUP AND " &
                                                         " GROUP_L02_GROUP = @GROUP_L02_GROUP AND " &
                                                         " GROUP_L03_GROUP = @GROUP_L03_GROUP AND " &
                                                         " GROUP_L04_GROUP = @GROUP_L04_GROUP AND " &
                                                         " GROUP_L05_GROUP = @GROUP_L05_GROUP AND " &
                                                         " GROUP_L06_GROUP = @GROUP_L06_GROUP AND " &
                                                         " GROUP_L07_GROUP = @GROUP_L07_GROUP AND " &
                                                         " GROUP_L08_GROUP = @GROUP_L08_GROUP AND " &
                                                         " GROUP_L09_GROUP = @GROUP_L09_GROUP AND " &
                                                         " GROUP_L10_GROUP = @GROUP_L10_GROUP AND " &
                                                         " PRODUCT = @PRODUCT "
                    cmd = New SqlCommand(strSelectBuild, conSql2005)
                    With cmd.Parameters
                        .Add(New SqlParameter("@GROUP_BUSINESS_UNIT", SqlDbType.NVarChar)).Value = group.BusinessUnit
                        .Add(New SqlParameter("@GROUP_PARTNER", SqlDbType.NVarChar)).Value = group.Partner
                        .Add(New SqlParameter("@GROUP_L01_GROUP", SqlDbType.NVarChar)).Value = group.L01Group
                        .Add(New SqlParameter("@GROUP_L02_GROUP", SqlDbType.NVarChar)).Value = group.L02Group
                        .Add(New SqlParameter("@GROUP_L03_GROUP", SqlDbType.NVarChar)).Value = group.L03Group
                        .Add(New SqlParameter("@GROUP_L04_GROUP", SqlDbType.NVarChar)).Value = group.L04Group
                        .Add(New SqlParameter("@GROUP_L05_GROUP", SqlDbType.NVarChar)).Value = group.L05Group
                        .Add(New SqlParameter("@GROUP_L06_GROUP", SqlDbType.NVarChar)).Value = group.L06Group
                        .Add(New SqlParameter("@GROUP_L07_GROUP", SqlDbType.NVarChar)).Value = group.L07Group
                        .Add(New SqlParameter("@GROUP_L08_GROUP", SqlDbType.NVarChar)).Value = group.L08Group
                        .Add(New SqlParameter("@GROUP_L09_GROUP", SqlDbType.NVarChar)).Value = group.L09Group
                        .Add(New SqlParameter("@GROUP_L10_GROUP", SqlDbType.NVarChar)).Value = group.L10Group
                        .Add(New SqlParameter("@PRODUCT", SqlDbType.NVarChar)).Value = product.Sku
                        .Add(New SqlParameter("@SEQUENCE", SqlDbType.NVarChar)).Value = product.DisplaySequence
                    End With
                    dtrReader = cmd.ExecuteReader
                    If Not dtrReader.HasRows Then

                        dtrReader.Close()

                        strInsertBuild = "INSERT INTO " &
                                          file &
                                          "(GROUP_BUSINESS_UNIT, GROUP_PARTNER, GROUP_L01_GROUP, " &
                                          " GROUP_L02_GROUP, GROUP_L03_GROUP, GROUP_L04_GROUP, " &
                                          " GROUP_L05_GROUP, " &
                                          " GROUP_L06_GROUP, GROUP_L07_GROUP, GROUP_L08_GROUP, " &
                                          " GROUP_L09_GROUP, GROUP_L10_GROUP, PRODUCT, SEQUENCE) " &
                                          " VALUES (" &
                                          " @GROUP_BUSINESS_UNIT, @GROUP_PARTNER, @GROUP_L01_GROUP, " &
                                          " @GROUP_L02_GROUP, @GROUP_L03_GROUP, @GROUP_L04_GROUP, " &
                                          " @GROUP_L05_GROUP, " &
                                          " @GROUP_L06_GROUP, @GROUP_L07_GROUP, @GROUP_L08_GROUP, " &
                                          " @GROUP_L09_GROUP, @GROUP_L10_GROUP, @PRODUCT, @SEQUENCE) "
                        cmd = New SqlCommand(strInsertBuild, conSql2005)
                        With cmd.Parameters
                            .Add(New SqlParameter("@GROUP_BUSINESS_UNIT", SqlDbType.NVarChar)).Value = group.BusinessUnit
                            .Add(New SqlParameter("@GROUP_PARTNER", SqlDbType.NVarChar)).Value = group.Partner
                            .Add(New SqlParameter("@GROUP_L01_GROUP", SqlDbType.NVarChar)).Value = group.L01Group
                            .Add(New SqlParameter("@GROUP_L02_GROUP", SqlDbType.NVarChar)).Value = group.L02Group
                            .Add(New SqlParameter("@GROUP_L03_GROUP", SqlDbType.NVarChar)).Value = group.L03Group
                            .Add(New SqlParameter("@GROUP_L04_GROUP", SqlDbType.NVarChar)).Value = group.L04Group
                            .Add(New SqlParameter("@GROUP_L05_GROUP", SqlDbType.NVarChar)).Value = group.L05Group
                            .Add(New SqlParameter("@GROUP_L06_GROUP", SqlDbType.NVarChar)).Value = group.L06Group
                            .Add(New SqlParameter("@GROUP_L07_GROUP", SqlDbType.NVarChar)).Value = group.L07Group
                            .Add(New SqlParameter("@GROUP_L08_GROUP", SqlDbType.NVarChar)).Value = group.L08Group
                            .Add(New SqlParameter("@GROUP_L09_GROUP", SqlDbType.NVarChar)).Value = group.L09Group
                            .Add(New SqlParameter("@GROUP_L10_GROUP", SqlDbType.NVarChar)).Value = group.L10Group
                            .Add(New SqlParameter("@PRODUCT", SqlDbType.NVarChar)).Value = product.Sku
                            .Add(New SqlParameter("@SEQUENCE", SqlDbType.NVarChar)).Value = product.DisplaySequence
                        End With

                        cmd.ExecuteNonQuery()
                    Else
                        dtrReader.Close()
                    End If

                Case Is = "UPDATE"
                    strUpdateBuild = "UPDATE " &
                                                        file & " SET " &
                                                        " SEQUENCE = @SEQUENCE " &
                                                        " WHERE " &
                                                        " GROUP_BUSINESS_UNIT = @GROUP_BUSINESS_UNIT AND " &
                                                         " GROUP_PARTNER = @GROUP_PARTNER AND " &
                                                         " GROUP_L01_GROUP = @GROUP_L01_GROUP AND " &
                                                         " GROUP_L02_GROUP = @GROUP_L02_GROUP AND " &
                                                         " GROUP_L03_GROUP = @GROUP_L03_GROUP AND " &
                                                         " GROUP_L04_GROUP = @GROUP_L04_GROUP AND " &
                                                         " GROUP_L05_GROUP = @GROUP_L05_GROUP AND " &
                                                         " GROUP_L06_GROUP = @GROUP_L06_GROUP AND " &
                                                         " GROUP_L07_GROUP = @GROUP_L07_GROUP AND " &
                                                         " GROUP_L08_GROUP = @GROUP_L08_GROUP AND " &
                                                         " GROUP_L09_GROUP = @GROUP_L09_GROUP AND " &
                                                         " GROUP_L10_GROUP = @GROUP_L10_GROUP AND " &
                                                         " PRODUCT = @PRODUCT "
                    cmd = New SqlCommand(strUpdateBuild, conSql2005)
                    With cmd.Parameters
                        .Add(New SqlParameter("@GROUP_BUSINESS_UNIT", SqlDbType.NVarChar)).Value = group.BusinessUnit
                        .Add(New SqlParameter("@GROUP_PARTNER", SqlDbType.NVarChar)).Value = group.Partner
                        .Add(New SqlParameter("@GROUP_L01_GROUP", SqlDbType.NVarChar)).Value = group.L01Group
                        .Add(New SqlParameter("@GROUP_L02_GROUP", SqlDbType.NVarChar)).Value = group.L02Group
                        .Add(New SqlParameter("@GROUP_L03_GROUP", SqlDbType.NVarChar)).Value = group.L03Group
                        .Add(New SqlParameter("@GROUP_L04_GROUP", SqlDbType.NVarChar)).Value = group.L04Group
                        .Add(New SqlParameter("@GROUP_L05_GROUP", SqlDbType.NVarChar)).Value = group.L05Group
                        .Add(New SqlParameter("@GROUP_L06_GROUP", SqlDbType.NVarChar)).Value = group.L06Group
                        .Add(New SqlParameter("@GROUP_L07_GROUP", SqlDbType.NVarChar)).Value = group.L07Group
                        .Add(New SqlParameter("@GROUP_L08_GROUP", SqlDbType.NVarChar)).Value = group.L08Group
                        .Add(New SqlParameter("@GROUP_L09_GROUP", SqlDbType.NVarChar)).Value = group.L09Group
                        .Add(New SqlParameter("@GROUP_L10_GROUP", SqlDbType.NVarChar)).Value = group.L10Group
                        .Add(New SqlParameter("@PRODUCT", SqlDbType.NVarChar)).Value = product.Sku
                        .Add(New SqlParameter("@SEQUENCE", SqlDbType.NVarChar)).Value = product.DisplaySequence
                    End With
                    cmd.ExecuteNonQuery()
                Case Is = "DELETE"
                    strDeleteBuild = "DELETE FROM " &
                                                        file &
                                                        " WHERE " &
                                                        " GROUP_BUSINESS_UNIT = @GROUP_BUSINESS_UNIT AND " &
                                                         " GROUP_PARTNER = @GROUP_PARTNER AND " &
                                                         " GROUP_L01_GROUP = @GROUP_L01_GROUP AND " &
                                                         " GROUP_L02_GROUP = @GROUP_L02_GROUP AND " &
                                                         " GROUP_L03_GROUP = @GROUP_L03_GROUP AND " &
                                                         " GROUP_L04_GROUP = @GROUP_L04_GROUP AND " &
                                                         " GROUP_L05_GROUP = @GROUP_L05_GROUP AND " &
                                                         " GROUP_L06_GROUP = @GROUP_L06_GROUP AND " &
                                                         " GROUP_L07_GROUP = @GROUP_L07_GROUP AND " &
                                                         " GROUP_L08_GROUP = @GROUP_L08_GROUP AND " &
                                                         " GROUP_L09_GROUP = @GROUP_L09_GROUP AND " &
                                                         " GROUP_L10_GROUP = @GROUP_L10_GROUP AND " &
                                                         " PRODUCT = @PRODUCT "
                    cmd = New SqlCommand(strDeleteBuild, conSql2005)
                    With cmd.Parameters
                        .Add(New SqlParameter("@GROUP_BUSINESS_UNIT", SqlDbType.NVarChar)).Value = group.BusinessUnit
                        .Add(New SqlParameter("@GROUP_PARTNER", SqlDbType.NVarChar)).Value = group.Partner
                        .Add(New SqlParameter("@GROUP_L01_GROUP", SqlDbType.NVarChar)).Value = group.L01Group
                        .Add(New SqlParameter("@GROUP_L02_GROUP", SqlDbType.NVarChar)).Value = group.L02Group
                        .Add(New SqlParameter("@GROUP_L03_GROUP", SqlDbType.NVarChar)).Value = group.L03Group
                        .Add(New SqlParameter("@GROUP_L04_GROUP", SqlDbType.NVarChar)).Value = group.L04Group
                        .Add(New SqlParameter("@GROUP_L05_GROUP", SqlDbType.NVarChar)).Value = group.L05Group
                        .Add(New SqlParameter("@GROUP_L06_GROUP", SqlDbType.NVarChar)).Value = group.L06Group
                        .Add(New SqlParameter("@GROUP_L07_GROUP", SqlDbType.NVarChar)).Value = group.L07Group
                        .Add(New SqlParameter("@GROUP_L08_GROUP", SqlDbType.NVarChar)).Value = group.L08Group
                        .Add(New SqlParameter("@GROUP_L09_GROUP", SqlDbType.NVarChar)).Value = group.L09Group
                        .Add(New SqlParameter("@GROUP_L10_GROUP", SqlDbType.NVarChar)).Value = group.L10Group
                        .Add(New SqlParameter("@PRODUCT", SqlDbType.NVarChar)).Value = product.Sku
                    End With
                    cmd.ExecuteNonQuery()
            End Select

        Next
    End Sub

#Region "AccessDatabaseSQL2005_ProductLoad"


    Private Function AccessDatabaseSQL2005_ProductLoad() As ErrorObj
        Dim err As New ErrorObj

        Dim cmd As SqlCommand

        Try

            ' Categories Definitions
            If Dep.CollDECategoryDefinitions.Count > 0 Then

                Select Case Dep.CategoryMode
                    Case Is = "UPDATE"

                        For Each cd As DECategoryDetails In Dep.CollDECategoryDefinitions
                            For Each pd As DEProductDescriptions In cd.CollDEProductDescriptions

                                Select Case cd.Mode
                                    Case Is = "ADD"

                                        Dim strInsert As String = "IF NOT EXISTS " &
                                            "(SELECT * FROM tbl_category_lang WHERE " &
                                            "CATEGORY_CODE = @CATEGORY_CODE1 AND LANGUAGE = @LANGUAGE1)" &
                                            "INSERT INTO tbl_category_lang VALUES " &
                                            "(@CATEGORY_CODE2, @SEQUENCE, @LANGUAGE2, @DESCRIPTION) "

                                        cmd = New SqlCommand(strInsert, conSql2005)
                                        With cmd.Parameters
                                            .Clear()
                                            .Add(New SqlParameter("@CATEGORY_CODE1", SqlDbType.NVarChar)).Value = cd.Code
                                            .Add(New SqlParameter("@SEQUENCE", SqlDbType.NVarChar)).Value = cd.DisplaySequence
                                            .Add(New SqlParameter("@LANGUAGE1", SqlDbType.NVarChar)).Value = pd.Language
                                            .Add(New SqlParameter("@DESCRIPTION", SqlDbType.NVarChar)).Value = pd.Description1
                                            .Add(New SqlParameter("@CATEGORY_CODE2", SqlDbType.NVarChar)).Value = cd.Code
                                            .Add(New SqlParameter("@LANGUAGE2", SqlDbType.NVarChar)).Value = pd.Language
                                        End With
                                        Try
                                            cmd.ExecuteNonQuery()
                                        Catch ex As Exception
                                            ResultDataSet = Nothing
                                            Const strError As String = "Error during database access"
                                            With err
                                                .ErrorMessage = ex.Message
                                                .ErrorStatus = strError
                                                .ErrorNumber = "TACDBPD-SQL2005-AG"
                                                .HasError = True
                                            End With
                                        End Try

                                    Case Is = "UPDATE"

                                        Dim strUpdate As String = "IF EXISTS " &
                                            "(SELECT * FROM tbl_category_lang WHERE " &
                                            "CATEGORY_CODE = @CATEGORY_CODE1 AND LANGUAGE = @LANGUAGE1) " &
                                            "UPDATE tbl_category_lang SET SEQUENCE = @SEQUENCE, DESCRIPTION = @DESCRIPTION " &
                                            "WHERE CATEGORY_CODE = @CATEGORY_CODE2 AND LANGUAGE = @LANGUAGE2"

                                        cmd = New SqlCommand(strUpdate, conSql2005)
                                        With cmd.Parameters
                                            .Clear()
                                            .Add(New SqlParameter("@CATEGORY_CODE1", SqlDbType.NVarChar)).Value = cd.Code
                                            .Add(New SqlParameter("@SEQUENCE", SqlDbType.NVarChar)).Value = cd.DisplaySequence
                                            .Add(New SqlParameter("@LANGUAGE1", SqlDbType.NVarChar)).Value = pd.Language
                                            .Add(New SqlParameter("@DESCRIPTION", SqlDbType.NVarChar)).Value = pd.Description1
                                            .Add(New SqlParameter("@CATEGORY_CODE2", SqlDbType.NVarChar)).Value = cd.Code
                                            .Add(New SqlParameter("@LANGUAGE2", SqlDbType.NVarChar)).Value = pd.Language
                                        End With
                                        Try
                                            cmd.ExecuteNonQuery()
                                        Catch ex As Exception
                                            ResultDataSet = Nothing
                                            Const strError As String = "Error during database access"
                                            With err
                                                .ErrorMessage = ex.Message
                                                .ErrorStatus = strError
                                                .ErrorNumber = "TACDBPD-SQL2005-AF"
                                                .HasError = True
                                            End With
                                        End Try

                                    Case Is = "DELETE"

                                        Dim strDelete As String = "DELETE FROM tbl_category_lang WHERE " &
                                            "CATEGORY_CODE = @CATEGORY_CODE"

                                        cmd = New SqlCommand(strDelete, conSql2005)
                                        With cmd.Parameters
                                            .Clear()
                                            .Add(New SqlParameter("@CATEGORY_CODE", SqlDbType.NVarChar)).Value = cd.Code
                                        End With
                                        Try
                                            cmd.ExecuteNonQuery()
                                        Catch ex As Exception
                                            ResultDataSet = Nothing
                                            Const strError As String = "Error during database access"
                                            With err
                                                .ErrorMessage = ex.Message
                                                .ErrorStatus = strError
                                                .ErrorNumber = "TACDBPD-SQL2005-AE"
                                                .HasError = True
                                            End With
                                        End Try

                                End Select

                            Next
                        Next

                    Case Is = "REPLACE"

                        Dim strDelete As String = String.Empty
                        Dim strUpdate As String = String.Empty
                        Dim strInsert As String = String.Empty

                        ' Delete all records from the work table
                        strDelete = "DELETE FROM tbl_category_lang_work"
                        cmd = New SqlCommand(strDelete, conSql2005)
                        Try
                            cmd.ExecuteNonQuery()
                        Catch ex As Exception
                            ResultDataSet = Nothing
                            Const strError As String = "Error during database access"
                            With err
                                .ErrorMessage = ex.Message
                                .ErrorStatus = strError
                                .ErrorNumber = "TACDBPD-SQL2005-AD"
                                .HasError = True
                            End With
                        End Try

                        ' Populate the work table 
                        For Each cd As DECategoryDetails In Dep.CollDECategoryDefinitions
                            For Each pd As DEProductDescriptions In cd.CollDEProductDescriptions

                                strInsert = "INSERT INTO tbl_category_lang_work VALUES " &
                                    "(@CATEGORY_CODE, @SEQUENCE, @LANGUAGE, @DESCRIPTION) "

                                cmd = New SqlCommand(strInsert, conSql2005)
                                With cmd.Parameters
                                    .Clear()
                                    .Add(New SqlParameter("@CATEGORY_CODE", SqlDbType.NVarChar)).Value = cd.Code
                                    .Add(New SqlParameter("@SEQUENCE", SqlDbType.NVarChar)).Value = cd.DisplaySequence
                                    .Add(New SqlParameter("@LANGUAGE", SqlDbType.NVarChar)).Value = pd.Language
                                    .Add(New SqlParameter("@DESCRIPTION", SqlDbType.NVarChar)).Value = pd.Description1
                                End With
                                Try
                                    cmd.ExecuteNonQuery()
                                Catch ex As Exception
                                    ResultDataSet = Nothing
                                    Const strError As String = "Error during database access"
                                    With err
                                        .ErrorMessage = ex.Message
                                        .ErrorStatus = strError
                                        .ErrorNumber = "TACDBPD-SQL2005-AC"
                                        .HasError = True
                                    End With
                                End Try

                            Next
                        Next

                        ' Now Insert/Update/Delete
                        strUpdate = "UPDATE tbl_category_lang " &
                            "SET SEQUENCE = tbl_category_lang_work.SEQUENCE, DESCRIPTION = tbl_category_lang_work.DESCRIPTION " &
                            "FROM tbl_category_lang_work " &
                            "WHERE tbl_category_lang_work.CATEGORY_CODE = tbl_category_lang.CATEGORY_CODE " &
                            "AND tbl_category_lang_work.LANGUAGE = tbl_category_lang.LANGUAGE " &
                            "AND EXISTS " &
                            "(SELECT * FROM tbl_category_lang " &
                            "WHERE tbl_category_lang.CATEGORY_CODE = tbl_category_lang_work.CATEGORY_CODE " &
                            "AND tbl_category_lang.LANGUAGE = tbl_category_lang_work.LANGUAGE)"

                        strInsert = "INSERT INTO tbl_category_lang " &
                            "(CATEGORY_CODE, SEQUENCE, LANGUAGE, DESCRIPTION) " &
                            "SELECT CATEGORY_CODE, SEQUENCE, LANGUAGE, DESCRIPTION " &
                            "FROM tbl_category_lang_work " &
                            "WHERE NOT EXISTS " &
                            "(SELECT * FROM tbl_category_lang " &
                            "WHERE tbl_category_lang.CATEGORY_CODE = tbl_category_lang_work.CATEGORY_CODE " &
                            "AND tbl_category_lang.LANGUAGE = tbl_category_lang_work.LANGUAGE)"

                        strDelete = "DELETE FROM tbl_category_lang " &
                            "WHERE NOT EXISTS (" &
                            "SELECT * FROM tbl_category_lang_work WHERE " &
                            "tbl_category_lang_work.CATEGORY_CODE = tbl_category_lang.CATEGORY_CODE AND " &
                            "tbl_category_lang_work.LANGUAGE = tbl_category_lang.LANGUAGE)"

                        Try
                            cmd = New SqlCommand(strUpdate, conSql2005)
                            cmd.ExecuteNonQuery()
                        Catch ex As Exception
                            ResultDataSet = Nothing
                            Const strError As String = "Error during database access"
                            With err
                                .ErrorMessage = ex.Message
                                .ErrorStatus = strError
                                .ErrorNumber = "TACDBPD-SQL2005-AB"
                                .HasError = True
                            End With
                        End Try

                        Try
                            cmd = New SqlCommand(strInsert, conSql2005)
                            cmd.ExecuteNonQuery()
                        Catch ex As Exception
                            ResultDataSet = Nothing
                            Const strError As String = "Error during database access"
                            With err
                                .ErrorMessage = ex.Message
                                .ErrorStatus = strError
                                .ErrorNumber = "TACDBPD-SQL2005-AA"
                                .HasError = True
                            End With
                        End Try

                        Try
                            cmd = New SqlCommand(strDelete, conSql2005)
                            cmd.ExecuteNonQuery()
                        Catch ex As Exception
                            ResultDataSet = Nothing
                            Const strError As String = "Error during database access"
                            With err
                                .ErrorMessage = ex.Message
                                .ErrorStatus = strError
                                .ErrorNumber = "TACDBPD-SQL2005-Z"
                                .HasError = True
                            End With
                        End Try

                End Select

            End If

            ' Attributes Definitions
            If Dep.CollDEAttributeDefinitions.Count > 0 Then

                Select Case Dep.AttributeMode
                    Case Is = "UPDATE"

                        For Each ad As DEAttributeDetails In Dep.CollDEAttributeDefinitions
                            For Each pd As DEProductDescriptions In ad.CollDEProductDescriptions

                                Select Case ad.Mode
                                    Case Is = "ADD"

                                        Dim strInsert As String = "IF NOT EXISTS " &
                                            "(SELECT * FROM tbl_attribute_lang WHERE " &
                                            "ATTRIBUTE_CODE = @ATTRIBUTE_CODE1 AND LANGUAGE = @LANGUAGE1)" &
                                            "INSERT INTO tbl_attribute_lang VALUES " &
                                            "(@ATTRIBUTE_CODE2, @SEQUENCE, @LANGUAGE2, @DESCRIPTION, @VALUE, @DATE, @BOOLEAN) "

                                        cmd = New SqlCommand(strInsert, conSql2005)
                                        With cmd.Parameters
                                            .Clear()
                                            .Add(New SqlParameter("@ATTRIBUTE_CODE1", SqlDbType.NVarChar)).Value = ad.Code
                                            .Add(New SqlParameter("@SEQUENCE", SqlDbType.NVarChar)).Value = ad.DisplaySequence
                                            .Add(New SqlParameter("@LANGUAGE1", SqlDbType.NVarChar)).Value = pd.Language
                                            .Add(New SqlParameter("@DESCRIPTION", SqlDbType.NVarChar)).Value = pd.Description1
                                            .Add(New SqlParameter("@ATTRIBUTE_CODE2", SqlDbType.NVarChar)).Value = ad.Code
                                            .Add(New SqlParameter("@LANGUAGE2", SqlDbType.NVarChar)).Value = pd.Language
                                            .Add(New SqlParameter("@VALUE", SqlDbType.Decimal)).Value = ad.AttributeValue
                                            .Add(New SqlParameter("@DATE", SqlDbType.DateTime)).Value = ad.AttributeDate
                                            .Add(New SqlParameter("@BOOLEAN", SqlDbType.Bit)).Value = ad.AttributeBoolean
                                        End With
                                        Try
                                            cmd.ExecuteNonQuery()
                                        Catch ex As Exception
                                            ResultDataSet = Nothing
                                            Const strError As String = "Error during database access"
                                            With err
                                                .ErrorMessage = ex.Message
                                                .ErrorStatus = strError
                                                .ErrorNumber = "TACDBPD-SQL2005-Y"
                                                .HasError = True
                                            End With
                                        End Try

                                    Case Is = "UPDATE"

                                        Dim strUpdate As String = "IF EXISTS " &
                                            "(SELECT * FROM tbl_attribute_lang WHERE " &
                                            "ATTRIBUTE_CODE = @ATTRIBUTE_CODE1 AND LANGUAGE = @LANGUAGE1) " &
                                            "UPDATE tbl_attribute_lang SET SEQUENCE = @SEQUENCE, DESCRIPTION = @DESCRIPTION, " &
                                            "VALUE = @VALUE, DATE = @DATE, BOOLEAN = @BOOLEAN " &
                                            "WHERE ATTRIBUTE_CODE = @ATTRIBUTE_CODE2 AND LANGUAGE = @LANGUAGE2"

                                        cmd = New SqlCommand(strUpdate, conSql2005)
                                        With cmd.Parameters
                                            .Clear()
                                            .Add(New SqlParameter("@ATTRIBUTE_CODE1", SqlDbType.NVarChar)).Value = ad.Code
                                            .Add(New SqlParameter("@SEQUENCE", SqlDbType.NVarChar)).Value = ad.DisplaySequence
                                            .Add(New SqlParameter("@LANGUAGE1", SqlDbType.NVarChar)).Value = pd.Language
                                            .Add(New SqlParameter("@DESCRIPTION", SqlDbType.NVarChar)).Value = pd.Description1
                                            .Add(New SqlParameter("@ATTRIBUTE_CODE2", SqlDbType.NVarChar)).Value = ad.Code
                                            .Add(New SqlParameter("@LANGUAGE2", SqlDbType.NVarChar)).Value = pd.Language
                                            .Add(New SqlParameter("@VALUE", SqlDbType.Decimal)).Value = ad.AttributeValue
                                            .Add(New SqlParameter("@DATE", SqlDbType.DateTime)).Value = ad.AttributeDate
                                            .Add(New SqlParameter("@BOOLEAN", SqlDbType.Bit)).Value = ad.AttributeBoolean
                                        End With
                                        Try
                                            cmd.ExecuteNonQuery()
                                        Catch ex As Exception
                                            ResultDataSet = Nothing
                                            Const strError As String = "Error during database access"
                                            With err
                                                .ErrorMessage = ex.Message
                                                .ErrorStatus = strError
                                                .ErrorNumber = "TACDBPD-SQL2005-X"
                                                .HasError = True
                                            End With
                                        End Try

                                    Case Is = "DELETE"

                                        Dim strDelete As String = "DELETE FROM tbl_attribute_lang WHERE " &
                                            "ATTRIBUTE_CODE = @ATTRIBUTE_CODE"

                                        cmd = New SqlCommand(strDelete, conSql2005)
                                        With cmd.Parameters
                                            .Clear()
                                            .Add(New SqlParameter("@ATTRIBUTE_CODE", SqlDbType.NVarChar)).Value = ad.Code
                                        End With
                                        Try
                                            cmd.ExecuteNonQuery()
                                        Catch ex As Exception
                                            ResultDataSet = Nothing
                                            Const strError As String = "Error during database access"
                                            With err
                                                .ErrorMessage = ex.Message
                                                .ErrorStatus = strError
                                                .ErrorNumber = "TACDBPD-SQL2005-W"
                                                .HasError = True
                                            End With
                                        End Try

                                End Select

                            Next
                        Next

                    Case Is = "REPLACE"

                        Dim strDelete As String = String.Empty
                        Dim strUpdate As String = String.Empty
                        Dim strInsert As String = String.Empty

                        ' Delete all records from the work table
                        strDelete = "DELETE FROM tbl_attribute_lang_work"
                        cmd = New SqlCommand(strDelete, conSql2005)
                        Try
                            cmd.ExecuteNonQuery()
                        Catch ex As Exception
                            ResultDataSet = Nothing
                            Const strError As String = "Error during database access"
                            With err
                                .ErrorMessage = ex.Message
                                .ErrorStatus = strError
                                .ErrorNumber = "TACDBPD-SQL2005-V"
                                .HasError = True
                            End With
                        End Try

                        ' Populate the work table 
                        For Each ad As DEAttributeDetails In Dep.CollDEAttributeDefinitions
                            For Each pd As DEProductDescriptions In ad.CollDEProductDescriptions

                                strInsert = "INSERT INTO tbl_attribute_lang_work VALUES " &
                                    "(@ATTRIBUTE_CODE, @SEQUENCE, @LANGUAGE, @DESCRIPTION, @VALUE, @DATE, @BOOLEAN) "

                                cmd = New SqlCommand(strInsert, conSql2005)
                                With cmd.Parameters
                                    .Clear()
                                    .Add(New SqlParameter("@ATTRIBUTE_CODE", SqlDbType.NVarChar)).Value = ad.Code
                                    .Add(New SqlParameter("@SEQUENCE", SqlDbType.NVarChar)).Value = ad.DisplaySequence
                                    .Add(New SqlParameter("@LANGUAGE", SqlDbType.NVarChar)).Value = pd.Language
                                    .Add(New SqlParameter("@DESCRIPTION", SqlDbType.NVarChar)).Value = pd.Description1
                                    .Add(New SqlParameter("@VALUE", SqlDbType.Decimal)).Value = ad.AttributeValue
                                    .Add(New SqlParameter("@DATE", SqlDbType.DateTime)).Value = ad.AttributeDate
                                    .Add(New SqlParameter("@BOOLEAN", SqlDbType.Bit)).Value = ad.AttributeBoolean
                                End With
                                Try
                                    cmd.ExecuteNonQuery()
                                Catch ex As Exception
                                    ResultDataSet = Nothing
                                    Const strError As String = "Error during database access"
                                    With err
                                        .ErrorMessage = ex.Message
                                        .ErrorStatus = strError
                                        .ErrorNumber = "TACDBPD-SQL2005-U"
                                        .HasError = True
                                    End With
                                End Try

                            Next
                        Next

                        ' Now Insert/Update/Delete
                        strUpdate = "UPDATE tbl_attribute_lang " &
                            "SET SEQUENCE = tbl_attribute_lang_work.SEQUENCE, DESCRIPTION = tbl_attribute_lang_work.DESCRIPTION, " &
                            "VALUE = tbl_attribute_lang_work.VALUE, DATE = tbl_attribute_lang_work.DATE, BOOLEAN = tbl_attribute_lang_work.BOOLEAN " &
                            "FROM tbl_attribute_lang_work " &
                            "WHERE tbl_attribute_lang_work.ATTRIBUTE_CODE = tbl_attribute_lang.ATTRIBUTE_CODE " &
                            "AND tbl_attribute_lang_work.LANGUAGE = tbl_attribute_lang.LANGUAGE " &
                            "AND EXISTS " &
                            "(SELECT * FROM tbl_attribute_lang " &
                            "WHERE tbl_attribute_lang.ATTRIBUTE_CODE = tbl_attribute_lang_work.ATTRIBUTE_CODE " &
                            "AND tbl_attribute_lang.LANGUAGE = tbl_attribute_lang_work.LANGUAGE)"

                        strInsert = "INSERT INTO tbl_attribute_lang " &
                            "(ATTRIBUTE_CODE, SEQUENCE, LANGUAGE, DESCRIPTION, VALUE, DATE, BOOLEAN) " &
                            "SELECT ATTRIBUTE_CODE, SEQUENCE, LANGUAGE, DESCRIPTION, VALUE, DATE, BOOLEAN " &
                            "FROM tbl_attribute_lang_work " &
                            "WHERE NOT EXISTS " &
                            "(SELECT * FROM tbl_attribute_lang " &
                            "WHERE tbl_attribute_lang.ATTRIBUTE_CODE = tbl_attribute_lang_work.ATTRIBUTE_CODE " &
                            "AND tbl_attribute_lang.LANGUAGE = tbl_attribute_lang_work.LANGUAGE)"

                        strDelete = "DELETE FROM tbl_attribute_lang " &
                            "WHERE NOT EXISTS (" &
                            "SELECT * FROM tbl_attribute_lang_work WHERE " &
                            "tbl_attribute_lang_work.ATTRIBUTE_CODE = tbl_attribute_lang.ATTRIBUTE_CODE AND " &
                            "tbl_attribute_lang_work.LANGUAGE = tbl_attribute_lang.LANGUAGE)"

                        Try
                            cmd = New SqlCommand(strUpdate, conSql2005)
                            cmd.ExecuteNonQuery()
                        Catch ex As Exception
                            ResultDataSet = Nothing
                            Const strError As String = "Error during database access"
                            With err
                                .ErrorMessage = ex.Message
                                .ErrorStatus = strError
                                .ErrorNumber = "TACDBPD-SQL2005-T"
                                .HasError = True
                            End With
                        End Try

                        Try
                            cmd = New SqlCommand(strInsert, conSql2005)
                            cmd.ExecuteNonQuery()
                        Catch ex As Exception
                            ResultDataSet = Nothing
                            Const strError As String = "Error during database access"
                            With err
                                .ErrorMessage = ex.Message
                                .ErrorStatus = strError
                                .ErrorNumber = "TACDBPD-SQL2005-S"
                                .HasError = True
                            End With
                        End Try

                        Try
                            cmd = New SqlCommand(strDelete, conSql2005)
                            cmd.ExecuteNonQuery()
                        Catch ex As Exception
                            ResultDataSet = Nothing
                            Const strError As String = "Error during database access"
                            With err
                                .ErrorMessage = ex.Message
                                .ErrorStatus = strError
                                .ErrorNumber = "TACDBPD-SQL2005-R"
                                .HasError = True
                            End With
                        End Try

                End Select

            End If

            ' Product Attributes
            If Dep.CollDEProducts.Count > 0 Then

                Select Case Dep.ProductMode
                    Case Is = "UPDATE", "ADD"

                        For Each pr As DEProductEcommerceDetails In Dep.CollDEProducts

                            Dim pd As DEProductDescriptions = pr.CollDEProductDescriptions(1)
                            Dim col As Collection = pr.CollDEProductAttributes

                            Dim pa As DEProductEcommerceAttribute = Nothing
                            Dim ad As attributeDetails = Nothing

                            Select Case pr.Mode
                                Case Is = "ADD"

                                    'Add to tbl_product
                                    Dim strInsert As String = "IF NOT EXISTS " &
                                        "(SELECT * FROM tbl_product WHERE " &
                                        "PRODUCT_CODE = @PRODUCT_CODE1) " &
                                        "INSERT INTO tbl_product VALUES " &
                                        "(@PRODUCT_CODE2, " &
                                        "@PRODUCT_DESCRIPTION1, " &
                                        "@PRODUCT_DESCRIPTION2, " &
                                        "@PRODUCT_DESCRIPTION3, " &
                                        "@PRODUCT_DESCRIPTION4, " &
                                        "@PRODUCT_DESCRIPTION5, " &
                                        "@PRODUCT_LENGTH, " &
                                        "@PRODUCT_LENGTH_UOM, " &
                                        "@PRODUCT_WIDTH, " &
                                        "@PRODUCT_WIDTH_UOM, " &
                                        "@PRODUCT_DEPTH, " &
                                        "@PRODUCT_DEPTH_UOM, " &
                                        "@PRODUCT_HEIGHT, " &
                                        "@PRODUCT_HEIGHT_UOM, " &
                                        "@PRODUCT_SIZE, " &
                                        "@PRODUCT_SIZE_UOM, " &
                                        "@PRODUCT_WEIGHT, " &
                                        "@PRODUCT_WEIGHT_UOM, " &
                                        "@PRODUCT_VOLUME, " &
                                        "@PRODUCT_VOLUME_UOM, " &
                                        "@PRODUCT_COLOUR, " &
                                        "@PRODUCT_PACK_SIZE, " &
                                        "@PRODUCT_PACK_SIZE_UOM, " &
                                        "@PRODUCT_SUPPLIER_PART_NO, " &
                                        "@PRODUCT_CUSTOMER_PART_NO, " &
                                        "@PRODUCT_TASTING_NOTES_1, " &
                                        "@PRODUCT_TASTING_NOTES_2, " &
                                        "@PRODUCT_ABV, " &
                                        "@PRODUCT_VINTAGE, " &
                                        "@PRODUCT_SUPPLIER, " &
                                        "@PRODUCT_COUNTRY, " &
                                        "@PRODUCT_REGION, " &
                                        "@PRODUCT_AREA, " &
                                        "@PRODUCT_GRAPE, " &
                                        "@PRODUCT_CLOSURE, " &
                                        "@PRODUCT_CATALOG_CODE, " &
                                        "@PRODUCT_VEGETARIAN, " &
                                        "@PRODUCT_VEGAN, " &
                                        "@PRODUCT_ORGANIC, " &
                                        "@PRODUCT_BIODYNAMIC, " &
                                        "@PRODUCT_LUTTE, " &
                                        "@PRODUCT_MINIMUM_AGE, " &
                                        "@PRODUCT_HTML_1, " &
                                        "@PRODUCT_HTML_2, " &
                                        "@PRODUCT_HTML_3, " &
                                        "@PRODUCT_SEARCH_KEYWORDS, " &
                                        "@PRODUCT_PAGE_TITLE, " &
                                        "@PRODUCT_META_DESCRIPTION, " &
                                        "@PRODUCT_META_KEYWORDS, " &
                                        "@PRODUCT_SEARCH_RANGE_01, " &
                                        "@PRODUCT_SEARCH_RANGE_02, " &
                                        "@PRODUCT_SEARCH_RANGE_03, " &
                                        "@PRODUCT_SEARCH_RANGE_04, " &
                                        "@PRODUCT_SEARCH_RANGE_05, " &
                                        "@PRODUCT_SEARCH_CRITERIA_01, " &
                                        "@PRODUCT_SEARCH_CRITERIA_02, " &
                                        "@PRODUCT_SEARCH_CRITERIA_03, " &
                                        "@PRODUCT_SEARCH_CRITERIA_04, " &
                                        "@PRODUCT_SEARCH_CRITERIA_05, " &
                                        "@PRODUCT_SEARCH_CRITERIA_06, " &
                                        "@PRODUCT_SEARCH_CRITERIA_07, " &
                                        "@PRODUCT_SEARCH_CRITERIA_08, " &
                                        "@PRODUCT_SEARCH_CRITERIA_09, " &
                                        "@PRODUCT_SEARCH_CRITERIA_10, " &
                                        "@PRODUCT_SEARCH_CRITERIA_11, " &
                                        "@PRODUCT_SEARCH_CRITERIA_12, " &
                                        "@PRODUCT_SEARCH_CRITERIA_13, " &
                                        "@PRODUCT_SEARCH_CRITERIA_14, " &
                                        "@PRODUCT_SEARCH_CRITERIA_15, " &
                                        "@PRODUCT_SEARCH_CRITERIA_16, " &
                                        "@PRODUCT_SEARCH_CRITERIA_17, " &
                                        "@PRODUCT_SEARCH_CRITERIA_18, " &
                                        "@PRODUCT_SEARCH_CRITERIA_19, " &
                                        "@PRODUCT_SEARCH_CRITERIA_20, " &
                                        "@PRODUCT_SEARCH_SWITCH_01, " &
                                        "@PRODUCT_SEARCH_SWITCH_02, " &
                                        "@PRODUCT_SEARCH_SWITCH_03, " &
                                        "@PRODUCT_SEARCH_SWITCH_04, " &
                                        "@PRODUCT_SEARCH_SWITCH_05, " &
                                        "@PRODUCT_SEARCH_SWITCH_06, " &
                                        "@PRODUCT_SEARCH_SWITCH_07, " &
                                        "@PRODUCT_SEARCH_SWITCH_08, " &
                                        "@PRODUCT_SEARCH_SWITCH_09, " &
                                        "@PRODUCT_SEARCH_SWITCH_10, " &
                                        "@PRODUCT_SEARCH_DATE_01, " &
                                        "@PRODUCT_SEARCH_DATE_02, " &
                                        "@PRODUCT_SEARCH_DATE_03, " &
                                        "@PRODUCT_SEARCH_DATE_04, " &
                                        "@PRODUCT_SEARCH_DATE_05, " &
                                        "@PRODUCT_TARIFF_CODE, " &
                                        "@PRODUCT_OPTION_MASTER, " &
                                        "@ALTERNATE_SKU, " &
                                        "@AVAILABLE_ONLINE, " &
                                        "@PERSONALISABLE, " &
                                        "@DISCONTINUED, " &
                                        "@PRODUCT_GLCODE_1, " &
                                        "@PRODUCT_GLCODE_2, " &
                                        "@PRODUCT_GLCODE_3, " &
                                        "@PRODUCT_GLCODE_4, " &
                                        "@PRODUCT_GLCODE_5, " &
                                        "@PRODUCT_HTML_4, " &
                                        "@PRODUCT_HTML_5, " &
                                        "@PRODUCT_HTML_6, " &
                                        "@PRODUCT_HTML_7, " &
                                        "@PRODUCT_HTML_8, " &
                                        "@PRODUCT_HTML_9 )"
                                    cmd = New SqlCommand(strInsert, conSql2005)
                                    With cmd.Parameters
                                        .Clear()
                                        .Add(New SqlParameter("@PRODUCT_CODE1", SqlDbType.NVarChar)).Value = pr.Sku
                                        .Add(New SqlParameter("@PRODUCT_CODE2", SqlDbType.NVarChar)).Value = pr.Sku
                                        .Add(New SqlParameter("@PRODUCT_DESCRIPTION1", SqlDbType.NVarChar)).Value = pd.Description1
                                        .Add(New SqlParameter("@PRODUCT_DESCRIPTION2", SqlDbType.NVarChar)).Value = pd.Description2
                                        .Add(New SqlParameter("@PRODUCT_DESCRIPTION3", SqlDbType.NVarChar)).Value = pd.Description3
                                        .Add(New SqlParameter("@PRODUCT_DESCRIPTION4", SqlDbType.NVarChar)).Value = pd.Description4
                                        .Add(New SqlParameter("@PRODUCT_DESCRIPTION5", SqlDbType.NVarChar)).Value = pd.Description5
                                        AddParameter(cmd, "PRODUCT_LENGTH", "VALUE", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_LENGTH_UOM", "DESCRIPTION", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_WIDTH", "VALUE", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_WIDTH_UOM", "DESCRIPTION", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_DEPTH", "VALUE", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_DEPTH_UOM", "DESCRIPTION", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_HEIGHT", "VALUE", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_HEIGHT_UOM", "DESCRIPTION", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_SIZE", "VALUE", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_SIZE_UOM", "DESCRIPTION", col, pd, pa, ad)
                                        'AddParameter(cmd, "PRODUCT_WEIGHT", "VALUE", col, pd, pa, ad)
                                        .Add(New SqlParameter("@PRODUCT_WEIGHT", SqlDbType.Decimal)).Value = pd.Weight
                                        AddParameter(cmd, "PRODUCT_WEIGHT_UOM", "DESCRIPTION", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_VOLUME", "VALUE", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_VOLUME_UOM", "DESCRIPTION", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_COLOUR", "DESCRIPTION", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_PACK_SIZE", "DESCRIPTION", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_PACK_SIZE_UOM", "DESCRIPTION", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_SUPPLIER_PART_NO", "DESCRIPTION", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_CUSTOMER_PART_NO", "DESCRIPTION", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_TASTING_NOTES_1", "DESCRIPTION", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_TASTING_NOTES_2", "DESCRIPTION", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_ABV", "DESCRIPTION", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_VINTAGE", "VALUE", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_SUPPLIER", "DESCRIPTION", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_COUNTRY", "DESCRIPTION", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_REGION", "DESCRIPTION", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_AREA", "DESCRIPTION", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_GRAPE", "DESCRIPTION", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_CLOSURE", "DESCRIPTION", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_CATALOG_CODE", "DESCRIPTION", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_VEGETARIAN", "BOOLEAN", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_VEGAN", "BOOLEAN", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_ORGANIC", "BOOLEAN", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_BIODYNAMIC", "BOOLEAN", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_LUTTE", "BOOLEAN", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_MINIMUM_AGE", "VALUE", col, pd, pa, ad)
                                        .Add(New SqlParameter("@PRODUCT_HTML_1", SqlDbType.NVarChar)).Value = pd.Html1
                                        .Add(New SqlParameter("@PRODUCT_HTML_2", SqlDbType.NVarChar)).Value = pd.Html2
                                        .Add(New SqlParameter("@PRODUCT_HTML_3", SqlDbType.NVarChar)).Value = pd.Html3
                                        .Add(New SqlParameter("@PRODUCT_SEARCH_KEYWORDS", SqlDbType.NVarChar)).Value = pd.SearchKeywords
                                        .Add(New SqlParameter("@PRODUCT_PAGE_TITLE", SqlDbType.NVarChar)).Value = pd.PageTitle
                                        .Add(New SqlParameter("@PRODUCT_META_DESCRIPTION", SqlDbType.NVarChar)).Value = pd.MetaDescription
                                        .Add(New SqlParameter("@PRODUCT_META_KEYWORDS", SqlDbType.NVarChar)).Value = pd.MetaKeywords
                                        AddParameter(cmd, "PRODUCT_SEARCH_RANGE_01", "VALUE", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_SEARCH_RANGE_02", "VALUE", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_SEARCH_RANGE_03", "VALUE", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_SEARCH_RANGE_04", "VALUE", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_SEARCH_RANGE_05", "VALUE", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_SEARCH_CRITERIA_01", "DESCRIPTION", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_SEARCH_CRITERIA_02", "DESCRIPTION", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_SEARCH_CRITERIA_03", "DESCRIPTION", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_SEARCH_CRITERIA_04", "DESCRIPTION", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_SEARCH_CRITERIA_05", "DESCRIPTION", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_SEARCH_CRITERIA_06", "DESCRIPTION", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_SEARCH_CRITERIA_07", "DESCRIPTION", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_SEARCH_CRITERIA_08", "DESCRIPTION", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_SEARCH_CRITERIA_09", "DESCRIPTION", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_SEARCH_CRITERIA_10", "DESCRIPTION", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_SEARCH_CRITERIA_11", "DESCRIPTION", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_SEARCH_CRITERIA_12", "DESCRIPTION", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_SEARCH_CRITERIA_13", "DESCRIPTION", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_SEARCH_CRITERIA_14", "DESCRIPTION", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_SEARCH_CRITERIA_15", "DESCRIPTION", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_SEARCH_CRITERIA_16", "DESCRIPTION", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_SEARCH_CRITERIA_17", "DESCRIPTION", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_SEARCH_CRITERIA_18", "DESCRIPTION", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_SEARCH_CRITERIA_19", "DESCRIPTION", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_SEARCH_CRITERIA_20", "DESCRIPTION", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_SEARCH_SWITCH_01", "BOOLEAN", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_SEARCH_SWITCH_02", "BOOLEAN", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_SEARCH_SWITCH_03", "BOOLEAN", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_SEARCH_SWITCH_04", "BOOLEAN", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_SEARCH_SWITCH_05", "BOOLEAN", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_SEARCH_SWITCH_06", "BOOLEAN", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_SEARCH_SWITCH_07", "BOOLEAN", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_SEARCH_SWITCH_08", "BOOLEAN", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_SEARCH_SWITCH_09", "BOOLEAN", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_SEARCH_SWITCH_10", "BOOLEAN", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_SEARCH_DATE_01", "DATE", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_SEARCH_DATE_02", "DATE", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_SEARCH_DATE_03", "DATE", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_SEARCH_DATE_04", "DATE", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_SEARCH_DATE_05", "DATE", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_TARIFF_CODE", "DESCRIPTION", col, pd, pa, ad)
                                        .Add(New SqlParameter("@PRODUCT_OPTION_MASTER", SqlDbType.Bit)).Value = pr.MasterProduct
                                        .Add(New SqlParameter("@ALTERNATE_SKU", SqlDbType.NVarChar)).Value = pr.AlternateSku
                                        .Add(New SqlParameter("@AVAILABLE_ONLINE", SqlDbType.Bit)).Value = pr.AvailableOnline
                                        .Add(New SqlParameter("@PERSONALISABLE", SqlDbType.Bit)).Value = False
                                        .Add(New SqlParameter("@DISCONTINUED", SqlDbType.Bit)).Value = False
                                        .Add(New SqlParameter("@PRODUCT_GLCODE_1", SqlDbType.NVarChar)).Value = pd.GLCode1
                                        .Add(New SqlParameter("@PRODUCT_GLCODE_2", SqlDbType.NVarChar)).Value = pd.GLCode2
                                        .Add(New SqlParameter("@PRODUCT_GLCODE_3", SqlDbType.NVarChar)).Value = pd.GLCode3
                                        .Add(New SqlParameter("@PRODUCT_GLCODE_4", SqlDbType.NVarChar)).Value = pd.GLCode4
                                        .Add(New SqlParameter("@PRODUCT_GLCODE_5", SqlDbType.NVarChar)).Value = pd.GLCode5
                                        .Add(New SqlParameter("@PRODUCT_HTML_4", SqlDbType.NVarChar)).Value = pd.Html4
                                        .Add(New SqlParameter("@PRODUCT_HTML_5", SqlDbType.NVarChar)).Value = pd.Html5
                                        .Add(New SqlParameter("@PRODUCT_HTML_6", SqlDbType.NVarChar)).Value = pd.Html6
                                        .Add(New SqlParameter("@PRODUCT_HTML_7", SqlDbType.NVarChar)).Value = pd.Html7
                                        .Add(New SqlParameter("@PRODUCT_HTML_8", SqlDbType.NVarChar)).Value = pd.Html8
                                        .Add(New SqlParameter("@PRODUCT_HTML_9", SqlDbType.NVarChar)).Value = pd.Html9
                                    End With

                                    Dim rowsAffected As Integer = 0
                                    Try
                                        rowsAffected = cmd.ExecuteNonQuery()
                                    Catch ex As Exception
                                        ResultDataSet = Nothing
                                        Const strError As String = "Error during database access"
                                        With err
                                            .ErrorMessage = ex.Message
                                            .ErrorStatus = strError
                                            .ErrorNumber = "TACDBPD-SQL2005-Q"
                                            .HasError = True
                                        End With
                                    End Try
                                    If rowsAffected < 1 Then
                                        ' process as update..
                                        'Update to tbl_product
                                        Dim strInsert2 As String = "IF EXISTS " &
                                            "(SELECT * FROM tbl_product WHERE " &
                                            "PRODUCT_CODE = @PRODUCT_CODE1) " &
                                            "UPDATE tbl_product SET "
                                        If Dep.UpdateDescriptions Then
                                            strInsert2 = strInsert2 & "PRODUCT_DESCRIPTION_1 = @PRODUCT_DESCRIPTION1, " &
                                            "PRODUCT_DESCRIPTION_2 = @PRODUCT_DESCRIPTION2, " &
                                            "PRODUCT_DESCRIPTION_3 = @PRODUCT_DESCRIPTION3, " &
                                            "PRODUCT_DESCRIPTION_4 = @PRODUCT_DESCRIPTION4, " &
                                            "PRODUCT_DESCRIPTION_5 = @PRODUCT_DESCRIPTION5, "
                                        End If
                                        strInsert2 = strInsert2 & "PRODUCT_LENGTH = @PRODUCT_LENGTH, " &
                                            "PRODUCT_LENGTH_UOM = @PRODUCT_LENGTH_UOM, " &
                                            "PRODUCT_WIDTH = @PRODUCT_WIDTH, " &
                                            "PRODUCT_WIDTH_UOM = @PRODUCT_WIDTH_UOM, " &
                                            "PRODUCT_DEPTH = @PRODUCT_DEPTH, " &
                                            "PRODUCT_DEPTH_UOM = @PRODUCT_DEPTH_UOM, " &
                                            "PRODUCT_HEIGHT = @PRODUCT_HEIGHT, " &
                                            "PRODUCT_HEIGHT_UOM = @PRODUCT_HEIGHT_UOM, " &
                                            "PRODUCT_SIZE = @PRODUCT_SIZE, " &
                                            "PRODUCT_SIZE_UOM = @PRODUCT_SIZE_UOM, " &
                                            "PRODUCT_WEIGHT = @PRODUCT_WEIGHT, " &
                                            "PRODUCT_WEIGHT_UOM = @PRODUCT_WEIGHT_UOM, " &
                                            "PRODUCT_VOLUME = @PRODUCT_VOLUME, " &
                                            "PRODUCT_VOLUME_UOM = @PRODUCT_VOLUME_UOM, " &
                                            "PRODUCT_COLOUR = @PRODUCT_COLOUR, " &
                                            "PRODUCT_PACK_SIZE = @PRODUCT_PACK_SIZE, " &
                                            "PRODUCT_PACK_SIZE_UOM = @PRODUCT_PACK_SIZE_UOM, " &
                                            "PRODUCT_SUPPLIER_PART_NO = @PRODUCT_SUPPLIER_PART_NO, " &
                                            "PRODUCT_CUSTOMER_PART_NO = @PRODUCT_CUSTOMER_PART_NO, " &
                                            "PRODUCT_TASTING_NOTES_1 = @PRODUCT_TASTING_NOTES_1, " &
                                            "PRODUCT_TASTING_NOTES_2 = @PRODUCT_TASTING_NOTES_2, " &
                                            "PRODUCT_ABV = @PRODUCT_ABV, " &
                                            "PRODUCT_VINTAGE = @PRODUCT_VINTAGE, " &
                                            "PRODUCT_SUPPLIER = @PRODUCT_SUPPLIER, " &
                                            "PRODUCT_COUNTRY = @PRODUCT_COUNTRY, " &
                                            "PRODUCT_REGION = @PRODUCT_REGION, " &
                                            "PRODUCT_AREA = @PRODUCT_AREA, " &
                                            "PRODUCT_GRAPE = @PRODUCT_GRAPE, " &
                                            "PRODUCT_CLOSURE = @PRODUCT_CLOSURE, " &
                                            "PRODUCT_CATALOG_CODE = @PRODUCT_CATALOG_CODE, " &
                                            "PRODUCT_VEGETARIAN = @PRODUCT_VEGETARIAN, " &
                                            "PRODUCT_VEGAN = @PRODUCT_VEGAN, " &
                                            "PRODUCT_ORGANIC = @PRODUCT_ORGANIC, " &
                                            "PRODUCT_BIODYNAMIC = @PRODUCT_BIODYNAMIC, " &
                                            "PRODUCT_LUTTE = @PRODUCT_LUTTE, " &
                                            "PRODUCT_MINIMUM_AGE = @PRODUCT_MINIMUM_AGE, "
                                        If Dep.UpdateDescriptions Then
                                            strInsert2 = strInsert2 & "PRODUCT_HTML_1 = @PRODUCT_HTML_1, " &
                                            "PRODUCT_HTML_2 = @PRODUCT_HTML_2, " &
                                            "PRODUCT_HTML_3 = @PRODUCT_HTML_3, " &
                                            "PRODUCT_HTML_4 = @PRODUCT_HTML_4, " &
                                            "PRODUCT_HTML_5 = @PRODUCT_HTML_5, " &
                                            "PRODUCT_HTML_6 = @PRODUCT_HTML_6, " &
                                            "PRODUCT_HTML_7 = @PRODUCT_HTML_7, " &
                                            "PRODUCT_HTML_8 = @PRODUCT_HTML_8, " &
                                            "PRODUCT_HTML_9 = @PRODUCT_HTML_9, "

                                        End If
                                        strInsert2 = strInsert2 & "PRODUCT_SEARCH_KEYWORDS = @PRODUCT_SEARCH_KEYWORDS, " &
                                            "PRODUCT_PAGE_TITLE = @PRODUCT_PAGE_TITLE, " &
                                            "PRODUCT_META_DESCRIPTION = @PRODUCT_META_DESCRIPTION, " &
                                            "PRODUCT_META_KEYWORDS = @PRODUCT_META_KEYWORDS, " &
                                            "PRODUCT_SEARCH_RANGE_01 = @PRODUCT_SEARCH_RANGE_01, " &
                                            "PRODUCT_SEARCH_RANGE_02 = @PRODUCT_SEARCH_RANGE_02, " &
                                            "PRODUCT_SEARCH_RANGE_03 = @PRODUCT_SEARCH_RANGE_03, " &
                                            "PRODUCT_SEARCH_RANGE_04 = @PRODUCT_SEARCH_RANGE_04, " &
                                            "PRODUCT_SEARCH_RANGE_05 = @PRODUCT_SEARCH_RANGE_05, " &
                                            "PRODUCT_SEARCH_CRITERIA_01 = @PRODUCT_SEARCH_CRITERIA_01, " &
                                            "PRODUCT_SEARCH_CRITERIA_02 = @PRODUCT_SEARCH_CRITERIA_02, " &
                                            "PRODUCT_SEARCH_CRITERIA_03 = @PRODUCT_SEARCH_CRITERIA_03, " &
                                            "PRODUCT_SEARCH_CRITERIA_04 = @PRODUCT_SEARCH_CRITERIA_04, " &
                                            "PRODUCT_SEARCH_CRITERIA_05 = @PRODUCT_SEARCH_CRITERIA_05, " &
                                            "PRODUCT_SEARCH_CRITERIA_06 = @PRODUCT_SEARCH_CRITERIA_06, " &
                                            "PRODUCT_SEARCH_CRITERIA_07 = @PRODUCT_SEARCH_CRITERIA_07, " &
                                            "PRODUCT_SEARCH_CRITERIA_08 = @PRODUCT_SEARCH_CRITERIA_08, " &
                                            "PRODUCT_SEARCH_CRITERIA_09 = @PRODUCT_SEARCH_CRITERIA_09, " &
                                            "PRODUCT_SEARCH_CRITERIA_10 = @PRODUCT_SEARCH_CRITERIA_10, " &
                                            "PRODUCT_SEARCH_CRITERIA_11 = @PRODUCT_SEARCH_CRITERIA_11, " &
                                            "PRODUCT_SEARCH_CRITERIA_12 = @PRODUCT_SEARCH_CRITERIA_12, " &
                                            "PRODUCT_SEARCH_CRITERIA_13 = @PRODUCT_SEARCH_CRITERIA_13, " &
                                            "PRODUCT_SEARCH_CRITERIA_14 = @PRODUCT_SEARCH_CRITERIA_14, " &
                                            "PRODUCT_SEARCH_CRITERIA_15 = @PRODUCT_SEARCH_CRITERIA_15, " &
                                            "PRODUCT_SEARCH_CRITERIA_16 = @PRODUCT_SEARCH_CRITERIA_16, " &
                                            "PRODUCT_SEARCH_CRITERIA_17 = @PRODUCT_SEARCH_CRITERIA_17, " &
                                            "PRODUCT_SEARCH_CRITERIA_18 = @PRODUCT_SEARCH_CRITERIA_18, " &
                                            "PRODUCT_SEARCH_CRITERIA_19 = @PRODUCT_SEARCH_CRITERIA_19, " &
                                            "PRODUCT_SEARCH_CRITERIA_20 = @PRODUCT_SEARCH_CRITERIA_20, " &
                                            "PRODUCT_SEARCH_SWITCH_01 = @PRODUCT_SEARCH_SWITCH_01, " &
                                            "PRODUCT_SEARCH_SWITCH_02 = @PRODUCT_SEARCH_SWITCH_02, " &
                                            "PRODUCT_SEARCH_SWITCH_03 = @PRODUCT_SEARCH_SWITCH_03, " &
                                            "PRODUCT_SEARCH_SWITCH_04 = @PRODUCT_SEARCH_SWITCH_04, " &
                                            "PRODUCT_SEARCH_SWITCH_05 = @PRODUCT_SEARCH_SWITCH_05, " &
                                            "PRODUCT_SEARCH_SWITCH_06 = @PRODUCT_SEARCH_SWITCH_06, " &
                                            "PRODUCT_SEARCH_SWITCH_07 = @PRODUCT_SEARCH_SWITCH_07, " &
                                            "PRODUCT_SEARCH_SWITCH_08 = @PRODUCT_SEARCH_SWITCH_08, " &
                                            "PRODUCT_SEARCH_SWITCH_09 = @PRODUCT_SEARCH_SWITCH_09, " &
                                            "PRODUCT_SEARCH_SWITCH_10 = @PRODUCT_SEARCH_SWITCH_10, " &
                                            "PRODUCT_SEARCH_DATE_01 = @PRODUCT_SEARCH_DATE_01, " &
                                            "PRODUCT_SEARCH_DATE_02 = @PRODUCT_SEARCH_DATE_02, " &
                                            "PRODUCT_SEARCH_DATE_03 = @PRODUCT_SEARCH_DATE_03, " &
                                            "PRODUCT_SEARCH_DATE_04 = @PRODUCT_SEARCH_DATE_04, " &
                                            "PRODUCT_SEARCH_DATE_05 = @PRODUCT_SEARCH_DATE_05, " &
                                            "PRODUCT_TARIFF_CODE = @PRODUCT_TARIFF_CODE, " &
                                            "PRODUCT_OPTION_MASTER = @PRODUCT_OPTION_MASTER, " &
                                            "ALTERNATE_SKU = @ALTERNATE_SKU, " &
                                            "AVAILABLE_ONLINE = @AVAILABLE_ONLINE , " &
                                            "PRODUCT_GLCODE_1 = @PRODUCT_GLCODE_1, " &
                                            "PRODUCT_GLCODE_2 = @PRODUCT_GLCODE_2, " &
                                            "PRODUCT_GLCODE_3 = @PRODUCT_GLCODE_3, " &
                                            "PRODUCT_GLCODE_4 = @PRODUCT_GLCODE_4, " &
                                            "PRODUCT_GLCODE_5 = @PRODUCT_GLCODE_5 " &
                                            "WHERE PRODUCT_CODE = @PRODUCT_CODE2"

                                        cmd = New SqlCommand(strInsert2, conSql2005)
                                        With cmd.Parameters
                                            .Clear()
                                            .Add(New SqlParameter("@PRODUCT_CODE1", SqlDbType.NVarChar)).Value = pr.Sku
                                            .Add(New SqlParameter("@PRODUCT_CODE2", SqlDbType.NVarChar)).Value = pr.Sku
                                            If Dep.UpdateDescriptions Then
                                                .Add(New SqlParameter("@PRODUCT_DESCRIPTION1", SqlDbType.NVarChar)).Value = pd.Description1
                                                .Add(New SqlParameter("@PRODUCT_DESCRIPTION2", SqlDbType.NVarChar)).Value = pd.Description2
                                                .Add(New SqlParameter("@PRODUCT_DESCRIPTION3", SqlDbType.NVarChar)).Value = pd.Description3
                                                .Add(New SqlParameter("@PRODUCT_DESCRIPTION4", SqlDbType.NVarChar)).Value = pd.Description4
                                                .Add(New SqlParameter("@PRODUCT_DESCRIPTION5", SqlDbType.NVarChar)).Value = pd.Description5
                                            End If

                                            AddParameter(cmd, "PRODUCT_LENGTH", "VALUE", col, pd, pa, ad)
                                            AddParameter(cmd, "PRODUCT_LENGTH_UOM", "DESCRIPTION", col, pd, pa, ad)
                                            AddParameter(cmd, "PRODUCT_WIDTH", "VALUE", col, pd, pa, ad)
                                            AddParameter(cmd, "PRODUCT_WIDTH_UOM", "DESCRIPTION", col, pd, pa, ad)
                                            AddParameter(cmd, "PRODUCT_DEPTH", "VALUE", col, pd, pa, ad)
                                            AddParameter(cmd, "PRODUCT_DEPTH_UOM", "DESCRIPTION", col, pd, pa, ad)
                                            AddParameter(cmd, "PRODUCT_HEIGHT", "VALUE", col, pd, pa, ad)
                                            AddParameter(cmd, "PRODUCT_HEIGHT_UOM", "DESCRIPTION", col, pd, pa, ad)
                                            AddParameter(cmd, "PRODUCT_SIZE", "VALUE", col, pd, pa, ad)
                                            AddParameter(cmd, "PRODUCT_SIZE_UOM", "DESCRIPTION", col, pd, pa, ad)
                                            'AddParameter(cmd, "PRODUCT_WEIGHT", "VALUE", col, pd, pa, ad)
                                            .Add(New SqlParameter("@PRODUCT_WEIGHT", SqlDbType.Decimal)).Value = pd.Weight
                                            AddParameter(cmd, "PRODUCT_WEIGHT_UOM", "DESCRIPTION", col, pd, pa, ad)
                                            AddParameter(cmd, "PRODUCT_VOLUME", "VALUE", col, pd, pa, ad)
                                            AddParameter(cmd, "PRODUCT_VOLUME_UOM", "DESCRIPTION", col, pd, pa, ad)
                                            AddParameter(cmd, "PRODUCT_COLOUR", "DESCRIPTION", col, pd, pa, ad)
                                            AddParameter(cmd, "PRODUCT_PACK_SIZE", "DESCRIPTION", col, pd, pa, ad)
                                            AddParameter(cmd, "PRODUCT_PACK_SIZE_UOM", "DESCRIPTION", col, pd, pa, ad)
                                            AddParameter(cmd, "PRODUCT_SUPPLIER_PART_NO", "DESCRIPTION", col, pd, pa, ad)
                                            AddParameter(cmd, "PRODUCT_CUSTOMER_PART_NO", "DESCRIPTION", col, pd, pa, ad)
                                            AddParameter(cmd, "PRODUCT_TASTING_NOTES_1", "DESCRIPTION", col, pd, pa, ad)
                                            AddParameter(cmd, "PRODUCT_TASTING_NOTES_2", "DESCRIPTION", col, pd, pa, ad)
                                            AddParameter(cmd, "PRODUCT_ABV", "DESCRIPTION", col, pd, pa, ad)
                                            AddParameter(cmd, "PRODUCT_VINTAGE", "VALUE", col, pd, pa, ad)
                                            AddParameter(cmd, "PRODUCT_SUPPLIER", "DESCRIPTION", col, pd, pa, ad)
                                            AddParameter(cmd, "PRODUCT_COUNTRY", "DESCRIPTION", col, pd, pa, ad)
                                            AddParameter(cmd, "PRODUCT_REGION", "DESCRIPTION", col, pd, pa, ad)
                                            AddParameter(cmd, "PRODUCT_AREA", "DESCRIPTION", col, pd, pa, ad)
                                            AddParameter(cmd, "PRODUCT_GRAPE", "DESCRIPTION", col, pd, pa, ad)
                                            AddParameter(cmd, "PRODUCT_CLOSURE", "DESCRIPTION", col, pd, pa, ad)
                                            AddParameter(cmd, "PRODUCT_CATALOG_CODE", "DESCRIPTION", col, pd, pa, ad)
                                            AddParameter(cmd, "PRODUCT_VEGETARIAN", "BOOLEAN", col, pd, pa, ad)
                                            AddParameter(cmd, "PRODUCT_VEGAN", "BOOLEAN", col, pd, pa, ad)
                                            AddParameter(cmd, "PRODUCT_ORGANIC", "BOOLEAN", col, pd, pa, ad)
                                            AddParameter(cmd, "PRODUCT_BIODYNAMIC", "BOOLEAN", col, pd, pa, ad)
                                            AddParameter(cmd, "PRODUCT_LUTTE", "BOOLEAN", col, pd, pa, ad)
                                            AddParameter(cmd, "PRODUCT_MINIMUM_AGE", "VALUE", col, pd, pa, ad)
                                            If Dep.UpdateDescriptions Then
                                                .Add(New SqlParameter("@PRODUCT_HTML_1", SqlDbType.NVarChar)).Value = pd.Html1
                                                .Add(New SqlParameter("@PRODUCT_HTML_2", SqlDbType.NVarChar)).Value = pd.Html2
                                                .Add(New SqlParameter("@PRODUCT_HTML_3", SqlDbType.NVarChar)).Value = pd.Html3
                                                .Add(New SqlParameter("@PRODUCT_HTML_4", SqlDbType.NVarChar)).Value = pd.Html4
                                                .Add(New SqlParameter("@PRODUCT_HTML_5", SqlDbType.NVarChar)).Value = pd.Html5
                                                .Add(New SqlParameter("@PRODUCT_HTML_6", SqlDbType.NVarChar)).Value = pd.Html6
                                                .Add(New SqlParameter("@PRODUCT_HTML_7", SqlDbType.NVarChar)).Value = pd.Html7
                                                .Add(New SqlParameter("@PRODUCT_HTML_8", SqlDbType.NVarChar)).Value = pd.Html8
                                                .Add(New SqlParameter("@PRODUCT_HTML_9", SqlDbType.NVarChar)).Value = pd.Html9
                                            End If
                                            .Add(New SqlParameter("@PRODUCT_SEARCH_KEYWORDS", SqlDbType.NVarChar)).Value = pd.SearchKeywords
                                            .Add(New SqlParameter("@PRODUCT_PAGE_TITLE", SqlDbType.NVarChar)).Value = pd.PageTitle
                                            .Add(New SqlParameter("@PRODUCT_META_DESCRIPTION", SqlDbType.NVarChar)).Value = pd.MetaDescription
                                            .Add(New SqlParameter("@PRODUCT_META_KEYWORDS", SqlDbType.NVarChar)).Value = pd.MetaKeywords
                                            AddParameter(cmd, "PRODUCT_SEARCH_RANGE_01", "VALUE", col, pd, pa, ad)
                                            AddParameter(cmd, "PRODUCT_SEARCH_RANGE_02", "VALUE", col, pd, pa, ad)
                                            AddParameter(cmd, "PRODUCT_SEARCH_RANGE_03", "VALUE", col, pd, pa, ad)
                                            AddParameter(cmd, "PRODUCT_SEARCH_RANGE_04", "VALUE", col, pd, pa, ad)
                                            AddParameter(cmd, "PRODUCT_SEARCH_RANGE_05", "VALUE", col, pd, pa, ad)
                                            AddParameter(cmd, "PRODUCT_SEARCH_CRITERIA_01", "DESCRIPTION", col, pd, pa, ad)
                                            AddParameter(cmd, "PRODUCT_SEARCH_CRITERIA_02", "DESCRIPTION", col, pd, pa, ad)
                                            AddParameter(cmd, "PRODUCT_SEARCH_CRITERIA_03", "DESCRIPTION", col, pd, pa, ad)
                                            AddParameter(cmd, "PRODUCT_SEARCH_CRITERIA_04", "DESCRIPTION", col, pd, pa, ad)
                                            AddParameter(cmd, "PRODUCT_SEARCH_CRITERIA_05", "DESCRIPTION", col, pd, pa, ad)
                                            AddParameter(cmd, "PRODUCT_SEARCH_CRITERIA_06", "DESCRIPTION", col, pd, pa, ad)
                                            AddParameter(cmd, "PRODUCT_SEARCH_CRITERIA_07", "DESCRIPTION", col, pd, pa, ad)
                                            AddParameter(cmd, "PRODUCT_SEARCH_CRITERIA_08", "DESCRIPTION", col, pd, pa, ad)
                                            AddParameter(cmd, "PRODUCT_SEARCH_CRITERIA_09", "DESCRIPTION", col, pd, pa, ad)
                                            AddParameter(cmd, "PRODUCT_SEARCH_CRITERIA_10", "DESCRIPTION", col, pd, pa, ad)
                                            AddParameter(cmd, "PRODUCT_SEARCH_CRITERIA_11", "DESCRIPTION", col, pd, pa, ad)
                                            AddParameter(cmd, "PRODUCT_SEARCH_CRITERIA_12", "DESCRIPTION", col, pd, pa, ad)
                                            AddParameter(cmd, "PRODUCT_SEARCH_CRITERIA_13", "DESCRIPTION", col, pd, pa, ad)
                                            AddParameter(cmd, "PRODUCT_SEARCH_CRITERIA_14", "DESCRIPTION", col, pd, pa, ad)
                                            AddParameter(cmd, "PRODUCT_SEARCH_CRITERIA_15", "DESCRIPTION", col, pd, pa, ad)
                                            AddParameter(cmd, "PRODUCT_SEARCH_CRITERIA_16", "DESCRIPTION", col, pd, pa, ad)
                                            AddParameter(cmd, "PRODUCT_SEARCH_CRITERIA_17", "DESCRIPTION", col, pd, pa, ad)
                                            AddParameter(cmd, "PRODUCT_SEARCH_CRITERIA_18", "DESCRIPTION", col, pd, pa, ad)
                                            AddParameter(cmd, "PRODUCT_SEARCH_CRITERIA_19", "DESCRIPTION", col, pd, pa, ad)
                                            AddParameter(cmd, "PRODUCT_SEARCH_CRITERIA_20", "DESCRIPTION", col, pd, pa, ad)
                                            AddParameter(cmd, "PRODUCT_SEARCH_SWITCH_01", "BOOLEAN", col, pd, pa, ad)
                                            AddParameter(cmd, "PRODUCT_SEARCH_SWITCH_02", "BOOLEAN", col, pd, pa, ad)
                                            AddParameter(cmd, "PRODUCT_SEARCH_SWITCH_03", "BOOLEAN", col, pd, pa, ad)
                                            AddParameter(cmd, "PRODUCT_SEARCH_SWITCH_04", "BOOLEAN", col, pd, pa, ad)
                                            AddParameter(cmd, "PRODUCT_SEARCH_SWITCH_05", "BOOLEAN", col, pd, pa, ad)
                                            AddParameter(cmd, "PRODUCT_SEARCH_SWITCH_06", "BOOLEAN", col, pd, pa, ad)
                                            AddParameter(cmd, "PRODUCT_SEARCH_SWITCH_07", "BOOLEAN", col, pd, pa, ad)
                                            AddParameter(cmd, "PRODUCT_SEARCH_SWITCH_08", "BOOLEAN", col, pd, pa, ad)
                                            AddParameter(cmd, "PRODUCT_SEARCH_SWITCH_09", "BOOLEAN", col, pd, pa, ad)
                                            AddParameter(cmd, "PRODUCT_SEARCH_SWITCH_10", "BOOLEAN", col, pd, pa, ad)
                                            AddParameter(cmd, "PRODUCT_SEARCH_DATE_01", "DATE", col, pd, pa, ad)
                                            AddParameter(cmd, "PRODUCT_SEARCH_DATE_02", "DATE", col, pd, pa, ad)
                                            AddParameter(cmd, "PRODUCT_SEARCH_DATE_03", "DATE", col, pd, pa, ad)
                                            AddParameter(cmd, "PRODUCT_SEARCH_DATE_04", "DATE", col, pd, pa, ad)
                                            AddParameter(cmd, "PRODUCT_SEARCH_DATE_05", "DATE", col, pd, pa, ad)
                                            AddParameter(cmd, "PRODUCT_TARIFF_CODE", "DESCRIPTION", col, pd, pa, ad)
                                            .Add(New SqlParameter("@PRODUCT_OPTION_MASTER", SqlDbType.Bit)).Value = pr.MasterProduct
                                            .Add(New SqlParameter("@ALTERNATE_SKU", SqlDbType.NVarChar)).Value = pr.AlternateSku
                                            .Add(New SqlParameter("@AVAILABLE_ONLINE", SqlDbType.Bit)).Value = pr.AvailableOnline
                                            .Add(New SqlParameter("@PERSONALISABLE", SqlDbType.Bit)).Value = False
                                            .Add(New SqlParameter("@DISCONTINUED", SqlDbType.Bit)).Value = False
                                            .Add(New SqlParameter("@PRODUCT_GLCODE_1", SqlDbType.NVarChar)).Value = pd.GLCode1
                                            .Add(New SqlParameter("@PRODUCT_GLCODE_2", SqlDbType.NVarChar)).Value = pd.GLCode2
                                            .Add(New SqlParameter("@PRODUCT_GLCODE_3", SqlDbType.NVarChar)).Value = pd.GLCode3
                                            .Add(New SqlParameter("@PRODUCT_GLCODE_4", SqlDbType.NVarChar)).Value = pd.GLCode4
                                            .Add(New SqlParameter("@PRODUCT_GLCODE_5", SqlDbType.NVarChar)).Value = pd.GLCode5
                                        End With
                                        rowsAffected = 0
                                        Try
                                            rowsAffected = cmd.ExecuteNonQuery()
                                        Catch ex As Exception
                                            ResultDataSet = Nothing
                                            Const strError As String = "Error during database access"
                                            With err
                                                .ErrorMessage = ex.Message
                                                .ErrorStatus = strError
                                                .ErrorNumber = "TACDBPD-SQL2005-O"
                                                .HasError = True
                                            End With
                                        End Try

                                    End If
                                Case Is = "UPDATE"

                                    'Update to tbl_product
                                    Dim strInsert As String = "IF EXISTS " &
                                        "(SELECT * FROM tbl_product WHERE " &
                                        "PRODUCT_CODE = @PRODUCT_CODE1) " &
                                        "UPDATE tbl_product SET "

                                    If Dep.UpdateDescriptions Then
                                        strInsert = strInsert & "PRODUCT_DESCRIPTION_1 = @PRODUCT_DESCRIPTION1, " &
                                                                              "PRODUCT_DESCRIPTION_2 = @PRODUCT_DESCRIPTION2, " &
                                                                              "PRODUCT_DESCRIPTION_3 = @PRODUCT_DESCRIPTION3, " &
                                                                              "PRODUCT_DESCRIPTION_4 = @PRODUCT_DESCRIPTION4, " &
                                                                              "PRODUCT_DESCRIPTION_5 = @PRODUCT_DESCRIPTION5, "
                                    End If
                                    strInsert = strInsert & "PRODUCT_LENGTH = @PRODUCT_LENGTH, " &
                                        "PRODUCT_LENGTH_UOM = @PRODUCT_LENGTH_UOM, " &
                                        "PRODUCT_WIDTH = @PRODUCT_WIDTH, " &
                                        "PRODUCT_WIDTH_UOM = @PRODUCT_WIDTH_UOM, " &
                                        "PRODUCT_DEPTH = @PRODUCT_DEPTH, " &
                                        "PRODUCT_DEPTH_UOM = @PRODUCT_DEPTH_UOM, " &
                                        "PRODUCT_HEIGHT = @PRODUCT_HEIGHT, " &
                                        "PRODUCT_HEIGHT_UOM = @PRODUCT_HEIGHT_UOM, " &
                                        "PRODUCT_SIZE = @PRODUCT_SIZE, " &
                                        "PRODUCT_SIZE_UOM = @PRODUCT_SIZE_UOM, " &
                                        "PRODUCT_WEIGHT = @PRODUCT_WEIGHT, " &
                                        "PRODUCT_WEIGHT_UOM = @PRODUCT_WEIGHT_UOM, " &
                                        "PRODUCT_VOLUME = @PRODUCT_VOLUME, " &
                                        "PRODUCT_VOLUME_UOM = @PRODUCT_VOLUME_UOM, " &
                                        "PRODUCT_COLOUR = @PRODUCT_COLOUR, " &
                                        "PRODUCT_PACK_SIZE = @PRODUCT_PACK_SIZE, " &
                                        "PRODUCT_PACK_SIZE_UOM = @PRODUCT_PACK_SIZE_UOM, " &
                                        "PRODUCT_SUPPLIER_PART_NO = @PRODUCT_SUPPLIER_PART_NO, " &
                                        "PRODUCT_CUSTOMER_PART_NO = @PRODUCT_CUSTOMER_PART_NO, " &
                                        "PRODUCT_TASTING_NOTES_1 = @PRODUCT_TASTING_NOTES_1, " &
                                        "PRODUCT_TASTING_NOTES_2 = @PRODUCT_TASTING_NOTES_2, " &
                                        "PRODUCT_ABV = @PRODUCT_ABV, " &
                                        "PRODUCT_VINTAGE = @PRODUCT_VINTAGE, " &
                                        "PRODUCT_SUPPLIER = @PRODUCT_SUPPLIER, " &
                                        "PRODUCT_COUNTRY = @PRODUCT_COUNTRY, " &
                                        "PRODUCT_REGION = @PRODUCT_REGION, " &
                                        "PRODUCT_AREA = @PRODUCT_AREA, " &
                                        "PRODUCT_GRAPE = @PRODUCT_GRAPE, " &
                                        "PRODUCT_CLOSURE = @PRODUCT_CLOSURE, " &
                                        "PRODUCT_CATALOG_CODE = @PRODUCT_CATALOG_CODE, " &
                                        "PRODUCT_VEGETARIAN = @PRODUCT_VEGETARIAN, " &
                                        "PRODUCT_VEGAN = @PRODUCT_VEGAN, " &
                                        "PRODUCT_ORGANIC = @PRODUCT_ORGANIC, " &
                                        "PRODUCT_BIODYNAMIC = @PRODUCT_BIODYNAMIC, " &
                                        "PRODUCT_LUTTE = @PRODUCT_LUTTE, " &
                                        "PRODUCT_MINIMUM_AGE = @PRODUCT_MINIMUM_AGE, "
                                    If Dep.UpdateDescriptions Then
                                        strInsert = strInsert & "PRODUCT_HTML_1 = @PRODUCT_HTML_1, " &
                                        "PRODUCT_HTML_2 = @PRODUCT_HTML_2, " &
                                        "PRODUCT_HTML_3 = @PRODUCT_HTML_3, " &
                                        "PRODUCT_HTML_4 = @PRODUCT_HTML_4, " &
                                        "PRODUCT_HTML_5 = @PRODUCT_HTML_5, " &
                                        "PRODUCT_HTML_6 = @PRODUCT_HTML_6, " &
                                        "PRODUCT_HTML_7 = @PRODUCT_HTML_7, " &
                                        "PRODUCT_HTML_8 = @PRODUCT_HTML_8, " &
                                        "PRODUCT_HTML_9 = @PRODUCT_HTML_9, "
                                    End If
                                    strInsert = strInsert & "PRODUCT_SEARCH_KEYWORDS = @PRODUCT_SEARCH_KEYWORDS, " &
                                        "PRODUCT_PAGE_TITLE = @PRODUCT_PAGE_TITLE, " &
                                        "PRODUCT_META_DESCRIPTION = @PRODUCT_META_DESCRIPTION, " &
                                        "PRODUCT_META_KEYWORDS = @PRODUCT_META_KEYWORDS, " &
                                        "PRODUCT_SEARCH_RANGE_01 = @PRODUCT_SEARCH_RANGE_01, " &
                                        "PRODUCT_SEARCH_RANGE_02 = @PRODUCT_SEARCH_RANGE_02, " &
                                        "PRODUCT_SEARCH_RANGE_03 = @PRODUCT_SEARCH_RANGE_03, " &
                                        "PRODUCT_SEARCH_RANGE_04 = @PRODUCT_SEARCH_RANGE_04, " &
                                        "PRODUCT_SEARCH_RANGE_05 = @PRODUCT_SEARCH_RANGE_05, " &
                                        "PRODUCT_SEARCH_CRITERIA_01 = @PRODUCT_SEARCH_CRITERIA_01, " &
                                        "PRODUCT_SEARCH_CRITERIA_02 = @PRODUCT_SEARCH_CRITERIA_02, " &
                                        "PRODUCT_SEARCH_CRITERIA_03 = @PRODUCT_SEARCH_CRITERIA_03, " &
                                        "PRODUCT_SEARCH_CRITERIA_04 = @PRODUCT_SEARCH_CRITERIA_04, " &
                                        "PRODUCT_SEARCH_CRITERIA_05 = @PRODUCT_SEARCH_CRITERIA_05, " &
                                        "PRODUCT_SEARCH_CRITERIA_06 = @PRODUCT_SEARCH_CRITERIA_06, " &
                                        "PRODUCT_SEARCH_CRITERIA_07 = @PRODUCT_SEARCH_CRITERIA_07, " &
                                        "PRODUCT_SEARCH_CRITERIA_08 = @PRODUCT_SEARCH_CRITERIA_08, " &
                                        "PRODUCT_SEARCH_CRITERIA_09 = @PRODUCT_SEARCH_CRITERIA_09, " &
                                        "PRODUCT_SEARCH_CRITERIA_10 = @PRODUCT_SEARCH_CRITERIA_10, " &
                                        "PRODUCT_SEARCH_CRITERIA_11 = @PRODUCT_SEARCH_CRITERIA_11, " &
                                        "PRODUCT_SEARCH_CRITERIA_12 = @PRODUCT_SEARCH_CRITERIA_12, " &
                                        "PRODUCT_SEARCH_CRITERIA_13 = @PRODUCT_SEARCH_CRITERIA_13, " &
                                        "PRODUCT_SEARCH_CRITERIA_14 = @PRODUCT_SEARCH_CRITERIA_14, " &
                                        "PRODUCT_SEARCH_CRITERIA_15 = @PRODUCT_SEARCH_CRITERIA_15, " &
                                        "PRODUCT_SEARCH_CRITERIA_16 = @PRODUCT_SEARCH_CRITERIA_16, " &
                                        "PRODUCT_SEARCH_CRITERIA_17 = @PRODUCT_SEARCH_CRITERIA_17, " &
                                        "PRODUCT_SEARCH_CRITERIA_18 = @PRODUCT_SEARCH_CRITERIA_18, " &
                                        "PRODUCT_SEARCH_CRITERIA_19 = @PRODUCT_SEARCH_CRITERIA_19, " &
                                        "PRODUCT_SEARCH_CRITERIA_20 = @PRODUCT_SEARCH_CRITERIA_20, " &
                                        "PRODUCT_SEARCH_SWITCH_01 = @PRODUCT_SEARCH_SWITCH_01, " &
                                        "PRODUCT_SEARCH_SWITCH_02 = @PRODUCT_SEARCH_SWITCH_02, " &
                                        "PRODUCT_SEARCH_SWITCH_03 = @PRODUCT_SEARCH_SWITCH_03, " &
                                        "PRODUCT_SEARCH_SWITCH_04 = @PRODUCT_SEARCH_SWITCH_04, " &
                                        "PRODUCT_SEARCH_SWITCH_05 = @PRODUCT_SEARCH_SWITCH_05, " &
                                        "PRODUCT_SEARCH_SWITCH_06 = @PRODUCT_SEARCH_SWITCH_06, " &
                                        "PRODUCT_SEARCH_SWITCH_07 = @PRODUCT_SEARCH_SWITCH_07, " &
                                        "PRODUCT_SEARCH_SWITCH_08 = @PRODUCT_SEARCH_SWITCH_08, " &
                                        "PRODUCT_SEARCH_SWITCH_09 = @PRODUCT_SEARCH_SWITCH_09, " &
                                        "PRODUCT_SEARCH_SWITCH_10 = @PRODUCT_SEARCH_SWITCH_10, " &
                                        "PRODUCT_SEARCH_DATE_01 = @PRODUCT_SEARCH_DATE_01, " &
                                        "PRODUCT_SEARCH_DATE_02 = @PRODUCT_SEARCH_DATE_02, " &
                                        "PRODUCT_SEARCH_DATE_03 = @PRODUCT_SEARCH_DATE_03, " &
                                        "PRODUCT_SEARCH_DATE_04 = @PRODUCT_SEARCH_DATE_04, " &
                                        "PRODUCT_SEARCH_DATE_05 = @PRODUCT_SEARCH_DATE_05, " &
                                        "PRODUCT_TARIFF_CODE = @PRODUCT_TARIFF_CODE, " &
                                        "PRODUCT_OPTION_MASTER = @PRODUCT_OPTION_MASTER, " &
                                        "ALTERNATE_SKU = @ALTERNATE_SKU, " &
                                        "AVAILABLE_ONLINE = @AVAILABLE_ONLINE, " &
                                        "PRODUCT_GLCODE_1 = @PRODUCT_GLCODE_1, " &
                                        "PRODUCT_GLCODE_2 = @PRODUCT_GLCODE_2, " &
                                        "PRODUCT_GLCODE_3 = @PRODUCT_GLCODE_3, " &
                                        "PRODUCT_GLCODE_4 = @PRODUCT_GLCODE_4, " &
                                        "PRODUCT_GLCODE_5 = @PRODUCT_GLCODE_5 " &
                                        "WHERE PRODUCT_CODE = @PRODUCT_CODE2"

                                    cmd = New SqlCommand(strInsert, conSql2005)
                                    With cmd.Parameters
                                        .Clear()
                                        .Add(New SqlParameter("@PRODUCT_CODE1", SqlDbType.NVarChar)).Value = pr.Sku
                                        .Add(New SqlParameter("@PRODUCT_CODE2", SqlDbType.NVarChar)).Value = pr.Sku
                                        If Dep.UpdateDescriptions Then
                                            .Add(New SqlParameter("@PRODUCT_DESCRIPTION1", SqlDbType.NVarChar)).Value = pd.Description1
                                            .Add(New SqlParameter("@PRODUCT_DESCRIPTION2", SqlDbType.NVarChar)).Value = pd.Description2
                                            .Add(New SqlParameter("@PRODUCT_DESCRIPTION3", SqlDbType.NVarChar)).Value = pd.Description3
                                            .Add(New SqlParameter("@PRODUCT_DESCRIPTION4", SqlDbType.NVarChar)).Value = pd.Description4
                                            .Add(New SqlParameter("@PRODUCT_DESCRIPTION5", SqlDbType.NVarChar)).Value = pd.Description5
                                        End If
                                        AddParameter(cmd, "PRODUCT_LENGTH", "VALUE", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_LENGTH_UOM", "DESCRIPTION", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_WIDTH", "VALUE", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_WIDTH_UOM", "DESCRIPTION", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_DEPTH", "VALUE", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_DEPTH_UOM", "DESCRIPTION", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_HEIGHT", "VALUE", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_HEIGHT_UOM", "DESCRIPTION", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_SIZE", "VALUE", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_SIZE_UOM", "DESCRIPTION", col, pd, pa, ad)
                                        'AddParameter(cmd, "PRODUCT_WEIGHT", "VALUE", col, pd, pa, ad)
                                        .Add(New SqlParameter("@PRODUCT_WEIGHT", SqlDbType.Decimal)).Value = pd.Weight
                                        AddParameter(cmd, "PRODUCT_WEIGHT_UOM", "DESCRIPTION", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_VOLUME", "VALUE", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_VOLUME_UOM", "DESCRIPTION", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_COLOUR", "DESCRIPTION", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_PACK_SIZE", "DESCRIPTION", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_PACK_SIZE_UOM", "DESCRIPTION", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_SUPPLIER_PART_NO", "DESCRIPTION", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_CUSTOMER_PART_NO", "DESCRIPTION", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_TASTING_NOTES_1", "DESCRIPTION", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_TASTING_NOTES_2", "DESCRIPTION", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_ABV", "DESCRIPTION", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_VINTAGE", "VALUE", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_SUPPLIER", "DESCRIPTION", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_COUNTRY", "DESCRIPTION", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_REGION", "DESCRIPTION", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_AREA", "DESCRIPTION", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_GRAPE", "DESCRIPTION", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_CLOSURE", "DESCRIPTION", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_CATALOG_CODE", "DESCRIPTION", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_VEGETARIAN", "BOOLEAN", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_VEGAN", "BOOLEAN", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_ORGANIC", "BOOLEAN", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_BIODYNAMIC", "BOOLEAN", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_LUTTE", "BOOLEAN", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_MINIMUM_AGE", "VALUE", col, pd, pa, ad)
                                        If Dep.UpdateDescriptions Then
                                            .Add(New SqlParameter("@PRODUCT_HTML_1", SqlDbType.NVarChar)).Value = pd.Html1
                                            .Add(New SqlParameter("@PRODUCT_HTML_2", SqlDbType.NVarChar)).Value = pd.Html2
                                            .Add(New SqlParameter("@PRODUCT_HTML_3", SqlDbType.NVarChar)).Value = pd.Html3
                                            .Add(New SqlParameter("@PRODUCT_HTML_4", SqlDbType.NVarChar)).Value = pd.Html4
                                            .Add(New SqlParameter("@PRODUCT_HTML_5", SqlDbType.NVarChar)).Value = pd.Html5
                                            .Add(New SqlParameter("@PRODUCT_HTML_6", SqlDbType.NVarChar)).Value = pd.Html6
                                            .Add(New SqlParameter("@PRODUCT_HTML_7", SqlDbType.NVarChar)).Value = pd.Html7
                                            .Add(New SqlParameter("@PRODUCT_HTML_8", SqlDbType.NVarChar)).Value = pd.Html8
                                            .Add(New SqlParameter("@PRODUCT_HTML_9", SqlDbType.NVarChar)).Value = pd.Html9
                                        End If
                                        .Add(New SqlParameter("@PRODUCT_SEARCH_KEYWORDS", SqlDbType.NVarChar)).Value = pd.SearchKeywords
                                        .Add(New SqlParameter("@PRODUCT_PAGE_TITLE", SqlDbType.NVarChar)).Value = pd.PageTitle
                                        .Add(New SqlParameter("@PRODUCT_META_DESCRIPTION", SqlDbType.NVarChar)).Value = pd.MetaDescription
                                        .Add(New SqlParameter("@PRODUCT_META_KEYWORDS", SqlDbType.NVarChar)).Value = pd.MetaKeywords
                                        AddParameter(cmd, "PRODUCT_SEARCH_RANGE_01", "VALUE", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_SEARCH_RANGE_02", "VALUE", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_SEARCH_RANGE_03", "VALUE", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_SEARCH_RANGE_04", "VALUE", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_SEARCH_RANGE_05", "VALUE", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_SEARCH_CRITERIA_01", "DESCRIPTION", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_SEARCH_CRITERIA_02", "DESCRIPTION", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_SEARCH_CRITERIA_03", "DESCRIPTION", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_SEARCH_CRITERIA_04", "DESCRIPTION", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_SEARCH_CRITERIA_05", "DESCRIPTION", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_SEARCH_CRITERIA_06", "DESCRIPTION", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_SEARCH_CRITERIA_07", "DESCRIPTION", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_SEARCH_CRITERIA_08", "DESCRIPTION", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_SEARCH_CRITERIA_09", "DESCRIPTION", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_SEARCH_CRITERIA_10", "DESCRIPTION", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_SEARCH_CRITERIA_11", "DESCRIPTION", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_SEARCH_CRITERIA_12", "DESCRIPTION", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_SEARCH_CRITERIA_13", "DESCRIPTION", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_SEARCH_CRITERIA_14", "DESCRIPTION", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_SEARCH_CRITERIA_15", "DESCRIPTION", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_SEARCH_CRITERIA_16", "DESCRIPTION", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_SEARCH_CRITERIA_17", "DESCRIPTION", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_SEARCH_CRITERIA_18", "DESCRIPTION", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_SEARCH_CRITERIA_19", "DESCRIPTION", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_SEARCH_CRITERIA_20", "DESCRIPTION", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_SEARCH_SWITCH_01", "BOOLEAN", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_SEARCH_SWITCH_02", "BOOLEAN", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_SEARCH_SWITCH_03", "BOOLEAN", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_SEARCH_SWITCH_04", "BOOLEAN", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_SEARCH_SWITCH_05", "BOOLEAN", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_SEARCH_SWITCH_06", "BOOLEAN", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_SEARCH_SWITCH_07", "BOOLEAN", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_SEARCH_SWITCH_08", "BOOLEAN", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_SEARCH_SWITCH_09", "BOOLEAN", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_SEARCH_SWITCH_10", "BOOLEAN", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_SEARCH_DATE_01", "DATE", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_SEARCH_DATE_02", "DATE", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_SEARCH_DATE_03", "DATE", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_SEARCH_DATE_04", "DATE", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_SEARCH_DATE_05", "DATE", col, pd, pa, ad)
                                        AddParameter(cmd, "PRODUCT_TARIFF_CODE", "DESCRIPTION", col, pd, pa, ad)
                                        .Add(New SqlParameter("@PRODUCT_OPTION_MASTER", SqlDbType.Bit)).Value = pr.MasterProduct
                                        .Add(New SqlParameter("@ALTERNATE_SKU", SqlDbType.NVarChar)).Value = pr.AlternateSku
                                        .Add(New SqlParameter("@AVAILABLE_ONLINE", SqlDbType.Bit)).Value = pr.AvailableOnline
                                        .Add(New SqlParameter("@PERSONALISABLE", SqlDbType.Bit)).Value = False
                                        .Add(New SqlParameter("@DISCONTINUED", SqlDbType.Bit)).Value = False
                                        .Add(New SqlParameter("@PRODUCT_GLCODE_1", SqlDbType.NVarChar)).Value = pd.GLCode1
                                        .Add(New SqlParameter("@PRODUCT_GLCODE_2", SqlDbType.NVarChar)).Value = pd.GLCode2
                                        .Add(New SqlParameter("@PRODUCT_GLCODE_3", SqlDbType.NVarChar)).Value = pd.GLCode3
                                        .Add(New SqlParameter("@PRODUCT_GLCODE_4", SqlDbType.NVarChar)).Value = pd.GLCode4
                                        .Add(New SqlParameter("@PRODUCT_GLCODE_5", SqlDbType.NVarChar)).Value = pd.GLCode5
                                    End With
                                    Try
                                        cmd.ExecuteNonQuery()
                                    Catch ex As Exception
                                        ResultDataSet = Nothing
                                        Const strError As String = "Error during database access"
                                        With err
                                            .ErrorMessage = ex.Message
                                            .ErrorStatus = strError
                                            .ErrorNumber = "TACDBPD-SQL2005-O"
                                            .HasError = True
                                        End With
                                    End Try
                                Case Is = "DELETE"

                                    Dim strDelete As String = "DELETE FROM tbl_product WHERE " &
                                        "PRODUCT_CODE = @PRODUCT_CODE"

                                    cmd = New SqlCommand(strDelete, conSql2005)
                                    With cmd.Parameters
                                        .Clear()
                                        .Add(New SqlParameter("@PRODUCT_CODE", SqlDbType.NVarChar)).Value = pr.Sku
                                    End With
                                    Try
                                        cmd.ExecuteNonQuery()
                                    Catch ex As Exception
                                        ResultDataSet = Nothing
                                        Const strError As String = "Error during database access"
                                        With err
                                            .ErrorMessage = ex.Message
                                            .ErrorStatus = strError
                                            .ErrorNumber = "TACDBPD-SQL2005-M"
                                            .HasError = True
                                        End With
                                    End Try
                            End Select

                        Next

                    Case Is = "REPLACE"

                        Dim strDelete As String = String.Empty
                        Dim strUpdate As String = String.Empty
                        Dim strInsert As String = String.Empty

                        ' Delete all records from the work tables
                        strDelete = "DELETE FROM tbl_product_work"
                        cmd = New SqlCommand(strDelete, conSql2005)
                        Try
                            cmd.ExecuteNonQuery()
                        Catch ex As Exception
                            ResultDataSet = Nothing
                            Const strError As String = "Error during database access"
                            With err
                                .ErrorMessage = ex.Message
                                .ErrorStatus = strError
                                .ErrorNumber = "TACDBPD-SQL2005-K"
                                .HasError = True
                            End With
                        End Try

                        'Populate the product work table
                        For Each pr As DEProductEcommerceDetails In Dep.CollDEProducts

                            Dim pd As DEProductDescriptions = pr.CollDEProductDescriptions(1)
                            Dim col As Collection = pr.CollDEProductAttributes

                            Dim pa As DEProductEcommerceAttribute = Nothing
                            Dim ad As attributeDetails = Nothing

                            strInsert =
                                "INSERT INTO tbl_product_work(PRODUCT_CODE, PRODUCT_DESCRIPTION_1, PRODUCT_DESCRIPTION_2, " &
                                "PRODUCT_DESCRIPTION_3, PRODUCT_DESCRIPTION_4, PRODUCT_DESCRIPTION_5, PRODUCT_LENGTH, PRODUCT_LENGTH_UOM, " &
                                "PRODUCT_WIDTH, PRODUCT_WIDTH_UOM, PRODUCT_DEPTH, PRODUCT_DEPTH_UOM, PRODUCT_HEIGHT, PRODUCT_HEIGHT_UOM, " &
                                "PRODUCT_SIZE, PRODUCT_SIZE_UOM, PRODUCT_WEIGHT, PRODUCT_WEIGHT_UOM, PRODUCT_VOLUME, PRODUCT_VOLUME_UOM, " &
                                "PRODUCT_COLOUR, PRODUCT_PACK_SIZE, PRODUCT_PACK_SIZE_UOM, PRODUCT_SUPPLIER_PART_NO, PRODUCT_CUSTOMER_PART_NO, " &
                                "PRODUCT_TASTING_NOTES_1, PRODUCT_TASTING_NOTES_2, PRODUCT_ABV, PRODUCT_VINTAGE, PRODUCT_SUPPLIER, " &
                                "PRODUCT_COUNTRY, PRODUCT_REGION, PRODUCT_AREA, PRODUCT_GRAPE, PRODUCT_CLOSURE, PRODUCT_CATALOG_CODE, " &
                                "PRODUCT_VEGETARIAN, PRODUCT_VEGAN, PRODUCT_ORGANIC, PRODUCT_BIODYNAMIC, PRODUCT_LUTTE, PRODUCT_MINIMUM_AGE, " &
                                "PRODUCT_HTML_1, PRODUCT_HTML_2, PRODUCT_HTML_3, PRODUCT_SEARCH_KEYWORDS, PRODUCT_PAGE_TITLE, PRODUCT_META_DESCRIPTION, " &
                                "PRODUCT_META_KEYWORDS, PRODUCT_SEARCH_RANGE_01, PRODUCT_SEARCH_RANGE_02, PRODUCT_SEARCH_RANGE_03, PRODUCT_SEARCH_RANGE_04, " &
                                "PRODUCT_SEARCH_RANGE_05, PRODUCT_SEARCH_CRITERIA_01, PRODUCT_SEARCH_CRITERIA_02, PRODUCT_SEARCH_CRITERIA_03, " &
                                "PRODUCT_SEARCH_CRITERIA_04, PRODUCT_SEARCH_CRITERIA_05, PRODUCT_SEARCH_CRITERIA_06, PRODUCT_SEARCH_CRITERIA_07, " &
                                "PRODUCT_SEARCH_CRITERIA_08, PRODUCT_SEARCH_CRITERIA_09, PRODUCT_SEARCH_CRITERIA_10, PRODUCT_SEARCH_CRITERIA_11, " &
                                "PRODUCT_SEARCH_CRITERIA_12, PRODUCT_SEARCH_CRITERIA_13, PRODUCT_SEARCH_CRITERIA_14, PRODUCT_SEARCH_CRITERIA_15, " &
                                "PRODUCT_SEARCH_CRITERIA_16, PRODUCT_SEARCH_CRITERIA_17, PRODUCT_SEARCH_CRITERIA_18, PRODUCT_SEARCH_CRITERIA_19, " &
                                "PRODUCT_SEARCH_CRITERIA_20, PRODUCT_SEARCH_SWITCH_01, PRODUCT_SEARCH_SWITCH_02, PRODUCT_SEARCH_SWITCH_03, " &
                                "PRODUCT_SEARCH_SWITCH_04, PRODUCT_SEARCH_SWITCH_05, PRODUCT_SEARCH_SWITCH_06, PRODUCT_SEARCH_SWITCH_07, " &
                                "PRODUCT_SEARCH_SWITCH_08, PRODUCT_SEARCH_SWITCH_09, PRODUCT_SEARCH_SWITCH_10, PRODUCT_SEARCH_DATE_01,  " &
                                "PRODUCT_SEARCH_DATE_02, PRODUCT_SEARCH_DATE_03, PRODUCT_SEARCH_DATE_04, PRODUCT_SEARCH_DATE_05, PRODUCT_TARIFF_CODE, " &
                                "PRODUCT_OPTION_MASTER, ALTERNATE_SKU, AVAILABLE_ONLINE, PERSONALISABLE, DISCONTINUED, PRODUCT_GLCODE_1, " &
                                "PRODUCT_GLCODE_2, PRODUCT_GLCODE_3, PRODUCT_GLCODE_4, PRODUCT_GLCODE_5, PRODUCT_HTML_4, PRODUCT_HTML_5, " &
                                "PRODUCT_HTML_6, PRODUCT_HTML_7, PRODUCT_HTML_8, PRODUCT_HTML_9) VALUES (" &
                                "@PRODUCT_CODE, " &
                                "@PRODUCT_DESCRIPTION1, " &
                                "@PRODUCT_DESCRIPTION2, " &
                                "@PRODUCT_DESCRIPTION3, " &
                                "@PRODUCT_DESCRIPTION4, " &
                                "@PRODUCT_DESCRIPTION5, " &
                                "@PRODUCT_LENGTH, " &
                                "@PRODUCT_LENGTH_UOM, " &
                                "@PRODUCT_WIDTH, " &
                                "@PRODUCT_WIDTH_UOM, " &
                                "@PRODUCT_DEPTH, " &
                                "@PRODUCT_DEPTH_UOM, " &
                                "@PRODUCT_HEIGHT, " &
                                "@PRODUCT_HEIGHT_UOM, " &
                                "@PRODUCT_SIZE, " &
                                "@PRODUCT_SIZE_UOM, " &
                                "@PRODUCT_WEIGHT, " &
                                "@PRODUCT_WEIGHT_UOM, " &
                                "@PRODUCT_VOLUME, " &
                                "@PRODUCT_VOLUME_UOM, " &
                                "@PRODUCT_COLOUR, " &
                                "@PRODUCT_PACK_SIZE, " &
                                "@PRODUCT_PACK_SIZE_UOM, " &
                                "@PRODUCT_SUPPLIER_PART_NO, " &
                                "@PRODUCT_CUSTOMER_PART_NO, " &
                                "@PRODUCT_TASTING_NOTES_1, " &
                                "@PRODUCT_TASTING_NOTES_2, " &
                                "@PRODUCT_ABV, " &
                                "@PRODUCT_VINTAGE, " &
                                "@PRODUCT_SUPPLIER, " &
                                "@PRODUCT_COUNTRY, " &
                                "@PRODUCT_REGION, " &
                                "@PRODUCT_AREA, " &
                                "@PRODUCT_GRAPE, " &
                                "@PRODUCT_CLOSURE, " &
                                "@PRODUCT_CATALOG_CODE, " &
                                "@PRODUCT_VEGETARIAN, " &
                                "@PRODUCT_VEGAN, " &
                                "@PRODUCT_ORGANIC, " &
                                "@PRODUCT_BIODYNAMIC, " &
                                "@PRODUCT_LUTTE, " &
                                "@PRODUCT_MINIMUM_AGE, " &
                                "@PRODUCT_HTML_1, " &
                                "@PRODUCT_HTML_2, " &
                                "@PRODUCT_HTML_3, " &
                                "@PRODUCT_SEARCH_KEYWORDS, " &
                                "@PRODUCT_PAGE_TITLE, " &
                                "@PRODUCT_META_DESCRIPTION, " &
                                "@PRODUCT_META_KEYWORDS, " &
                                "@PRODUCT_SEARCH_RANGE_01, " &
                                "@PRODUCT_SEARCH_RANGE_02, " &
                                "@PRODUCT_SEARCH_RANGE_03, " &
                                "@PRODUCT_SEARCH_RANGE_04, " &
                                "@PRODUCT_SEARCH_RANGE_05, " &
                                "@PRODUCT_SEARCH_CRITERIA_01, " &
                                "@PRODUCT_SEARCH_CRITERIA_02, " &
                                "@PRODUCT_SEARCH_CRITERIA_03, " &
                                "@PRODUCT_SEARCH_CRITERIA_04, " &
                                "@PRODUCT_SEARCH_CRITERIA_05, " &
                                "@PRODUCT_SEARCH_CRITERIA_06, " &
                                "@PRODUCT_SEARCH_CRITERIA_07, " &
                                "@PRODUCT_SEARCH_CRITERIA_08, " &
                                "@PRODUCT_SEARCH_CRITERIA_09, " &
                                "@PRODUCT_SEARCH_CRITERIA_10, " &
                                "@PRODUCT_SEARCH_CRITERIA_11, " &
                                "@PRODUCT_SEARCH_CRITERIA_12, " &
                                "@PRODUCT_SEARCH_CRITERIA_13, " &
                                "@PRODUCT_SEARCH_CRITERIA_14, " &
                                "@PRODUCT_SEARCH_CRITERIA_15, " &
                                "@PRODUCT_SEARCH_CRITERIA_16, " &
                                "@PRODUCT_SEARCH_CRITERIA_17, " &
                                "@PRODUCT_SEARCH_CRITERIA_18, " &
                                "@PRODUCT_SEARCH_CRITERIA_19, " &
                                "@PRODUCT_SEARCH_CRITERIA_20, " &
                                "@PRODUCT_SEARCH_SWITCH_01, " &
                                "@PRODUCT_SEARCH_SWITCH_02, " &
                                "@PRODUCT_SEARCH_SWITCH_03, " &
                                "@PRODUCT_SEARCH_SWITCH_04, " &
                                "@PRODUCT_SEARCH_SWITCH_05, " &
                                "@PRODUCT_SEARCH_SWITCH_06, " &
                                "@PRODUCT_SEARCH_SWITCH_07, " &
                                "@PRODUCT_SEARCH_SWITCH_08, " &
                                "@PRODUCT_SEARCH_SWITCH_09, " &
                                "@PRODUCT_SEARCH_SWITCH_10, " &
                                "@PRODUCT_SEARCH_DATE_01, " &
                                "@PRODUCT_SEARCH_DATE_02, " &
                                "@PRODUCT_SEARCH_DATE_03, " &
                                "@PRODUCT_SEARCH_DATE_04, " &
                                "@PRODUCT_SEARCH_DATE_05, " &
                                "@PRODUCT_TARIFF_CODE, " &
                                "@PRODUCT_OPTION_MASTER, " &
                                "@ALTERNATE_SKU, " &
                                 "@AVAILABLE_ONLINE, " &
                                "@PERSONALISABLE, " &
                                "@DISCONTINUED, " &
                                "@PRODUCT_GLCODE_1, " &
                                "@PRODUCT_GLCODE_2, " &
                                "@PRODUCT_GLCODE_3, " &
                                "@PRODUCT_GLCODE_4, " &
                                "@PRODUCT_GLCODE_5, " &
                                "@PRODUCT_HTML_4, " &
                                "@PRODUCT_HTML_5, " &
                                "@PRODUCT_HTML_6, " &
                                "@PRODUCT_HTML_7, " &
                                "@PRODUCT_HTML_8, " &
                                "@PRODUCT_HTML_9 ) "
                            cmd = New SqlCommand(strInsert, conSql2005)
                            With cmd.Parameters
                                .Clear()
                                .Add(New SqlParameter("@PRODUCT_CODE", SqlDbType.NVarChar)).Value = pr.Sku
                                .Add(New SqlParameter("@PRODUCT_DESCRIPTION1", SqlDbType.NVarChar)).Value = pd.Description1
                                .Add(New SqlParameter("@PRODUCT_DESCRIPTION2", SqlDbType.NVarChar)).Value = pd.Description2
                                .Add(New SqlParameter("@PRODUCT_DESCRIPTION3", SqlDbType.NVarChar)).Value = pd.Description3
                                .Add(New SqlParameter("@PRODUCT_DESCRIPTION4", SqlDbType.NVarChar)).Value = pd.Description4
                                .Add(New SqlParameter("@PRODUCT_DESCRIPTION5", SqlDbType.NVarChar)).Value = pd.Description5
                                AddParameter(cmd, "PRODUCT_LENGTH", "VALUE", col, pd, pa, ad)
                                AddParameter(cmd, "PRODUCT_LENGTH_UOM", "DESCRIPTION", col, pd, pa, ad)
                                AddParameter(cmd, "PRODUCT_WIDTH", "VALUE", col, pd, pa, ad)
                                AddParameter(cmd, "PRODUCT_WIDTH_UOM", "DESCRIPTION", col, pd, pa, ad)
                                AddParameter(cmd, "PRODUCT_DEPTH", "VALUE", col, pd, pa, ad)
                                AddParameter(cmd, "PRODUCT_DEPTH_UOM", "DESCRIPTION", col, pd, pa, ad)
                                AddParameter(cmd, "PRODUCT_HEIGHT", "VALUE", col, pd, pa, ad)
                                AddParameter(cmd, "PRODUCT_HEIGHT_UOM", "DESCRIPTION", col, pd, pa, ad)
                                AddParameter(cmd, "PRODUCT_SIZE", "VALUE", col, pd, pa, ad)
                                AddParameter(cmd, "PRODUCT_SIZE_UOM", "DESCRIPTION", col, pd, pa, ad)
                                'AddParameter(cmd, "PRODUCT_WEIGHT", "VALUE", col, pd, pa, ad)
                                .Add(New SqlParameter("@PRODUCT_WEIGHT", SqlDbType.Decimal)).Value = pd.Weight
                                AddParameter(cmd, "PRODUCT_WEIGHT_UOM", "DESCRIPTION", col, pd, pa, ad)
                                AddParameter(cmd, "PRODUCT_VOLUME", "VALUE", col, pd, pa, ad)
                                AddParameter(cmd, "PRODUCT_VOLUME_UOM", "DESCRIPTION", col, pd, pa, ad)
                                AddParameter(cmd, "PRODUCT_COLOUR", "DESCRIPTION", col, pd, pa, ad)
                                AddParameter(cmd, "PRODUCT_PACK_SIZE", "DESCRIPTION", col, pd, pa, ad)
                                AddParameter(cmd, "PRODUCT_PACK_SIZE_UOM", "DESCRIPTION", col, pd, pa, ad)
                                AddParameter(cmd, "PRODUCT_SUPPLIER_PART_NO", "DESCRIPTION", col, pd, pa, ad)
                                AddParameter(cmd, "PRODUCT_CUSTOMER_PART_NO", "DESCRIPTION", col, pd, pa, ad)
                                AddParameter(cmd, "PRODUCT_TASTING_NOTES_1", "DESCRIPTION", col, pd, pa, ad)
                                AddParameter(cmd, "PRODUCT_TASTING_NOTES_2", "DESCRIPTION", col, pd, pa, ad)
                                AddParameter(cmd, "PRODUCT_ABV", "DESCRIPTION", col, pd, pa, ad)
                                AddParameter(cmd, "PRODUCT_VINTAGE", "VALUE", col, pd, pa, ad)
                                AddParameter(cmd, "PRODUCT_SUPPLIER", "DESCRIPTION", col, pd, pa, ad)
                                AddParameter(cmd, "PRODUCT_COUNTRY", "DESCRIPTION", col, pd, pa, ad)
                                AddParameter(cmd, "PRODUCT_REGION", "DESCRIPTION", col, pd, pa, ad)
                                AddParameter(cmd, "PRODUCT_AREA", "DESCRIPTION", col, pd, pa, ad)
                                AddParameter(cmd, "PRODUCT_GRAPE", "DESCRIPTION", col, pd, pa, ad)
                                AddParameter(cmd, "PRODUCT_CLOSURE", "DESCRIPTION", col, pd, pa, ad)
                                AddParameter(cmd, "PRODUCT_CATALOG_CODE", "DESCRIPTION", col, pd, pa, ad)
                                AddParameter(cmd, "PRODUCT_VEGETARIAN", "BOOLEAN", col, pd, pa, ad)
                                AddParameter(cmd, "PRODUCT_VEGAN", "BOOLEAN", col, pd, pa, ad)
                                AddParameter(cmd, "PRODUCT_ORGANIC", "BOOLEAN", col, pd, pa, ad)
                                AddParameter(cmd, "PRODUCT_BIODYNAMIC", "BOOLEAN", col, pd, pa, ad)
                                AddParameter(cmd, "PRODUCT_LUTTE", "BOOLEAN", col, pd, pa, ad)
                                AddParameter(cmd, "PRODUCT_MINIMUM_AGE", "VALUE", col, pd, pa, ad)
                                .Add(New SqlParameter("@PRODUCT_HTML_1", SqlDbType.NVarChar)).Value = pd.Html1
                                .Add(New SqlParameter("@PRODUCT_HTML_2", SqlDbType.NVarChar)).Value = pd.Html2
                                .Add(New SqlParameter("@PRODUCT_HTML_3", SqlDbType.NVarChar)).Value = pd.Html3
                                .Add(New SqlParameter("@PRODUCT_SEARCH_KEYWORDS", SqlDbType.NVarChar)).Value = pd.SearchKeywords
                                .Add(New SqlParameter("@PRODUCT_PAGE_TITLE", SqlDbType.NVarChar)).Value = pd.PageTitle
                                .Add(New SqlParameter("@PRODUCT_META_DESCRIPTION", SqlDbType.NVarChar)).Value = pd.MetaDescription
                                .Add(New SqlParameter("@PRODUCT_META_KEYWORDS", SqlDbType.NVarChar)).Value = pd.MetaKeywords
                                AddParameter(cmd, "PRODUCT_SEARCH_RANGE_01", "VALUE", col, pd, pa, ad)
                                AddParameter(cmd, "PRODUCT_SEARCH_RANGE_02", "VALUE", col, pd, pa, ad)
                                AddParameter(cmd, "PRODUCT_SEARCH_RANGE_03", "VALUE", col, pd, pa, ad)
                                AddParameter(cmd, "PRODUCT_SEARCH_RANGE_04", "VALUE", col, pd, pa, ad)
                                AddParameter(cmd, "PRODUCT_SEARCH_RANGE_05", "VALUE", col, pd, pa, ad)
                                AddParameter(cmd, "PRODUCT_SEARCH_CRITERIA_01", "DESCRIPTION", col, pd, pa, ad)
                                AddParameter(cmd, "PRODUCT_SEARCH_CRITERIA_02", "DESCRIPTION", col, pd, pa, ad)
                                AddParameter(cmd, "PRODUCT_SEARCH_CRITERIA_03", "DESCRIPTION", col, pd, pa, ad)
                                AddParameter(cmd, "PRODUCT_SEARCH_CRITERIA_04", "DESCRIPTION", col, pd, pa, ad)
                                AddParameter(cmd, "PRODUCT_SEARCH_CRITERIA_05", "DESCRIPTION", col, pd, pa, ad)
                                AddParameter(cmd, "PRODUCT_SEARCH_CRITERIA_06", "DESCRIPTION", col, pd, pa, ad)
                                AddParameter(cmd, "PRODUCT_SEARCH_CRITERIA_07", "DESCRIPTION", col, pd, pa, ad)
                                AddParameter(cmd, "PRODUCT_SEARCH_CRITERIA_08", "DESCRIPTION", col, pd, pa, ad)
                                AddParameter(cmd, "PRODUCT_SEARCH_CRITERIA_09", "DESCRIPTION", col, pd, pa, ad)
                                AddParameter(cmd, "PRODUCT_SEARCH_CRITERIA_10", "DESCRIPTION", col, pd, pa, ad)
                                AddParameter(cmd, "PRODUCT_SEARCH_CRITERIA_11", "DESCRIPTION", col, pd, pa, ad)
                                AddParameter(cmd, "PRODUCT_SEARCH_CRITERIA_12", "DESCRIPTION", col, pd, pa, ad)
                                AddParameter(cmd, "PRODUCT_SEARCH_CRITERIA_13", "DESCRIPTION", col, pd, pa, ad)
                                AddParameter(cmd, "PRODUCT_SEARCH_CRITERIA_14", "DESCRIPTION", col, pd, pa, ad)
                                AddParameter(cmd, "PRODUCT_SEARCH_CRITERIA_15", "DESCRIPTION", col, pd, pa, ad)
                                AddParameter(cmd, "PRODUCT_SEARCH_CRITERIA_16", "DESCRIPTION", col, pd, pa, ad)
                                AddParameter(cmd, "PRODUCT_SEARCH_CRITERIA_17", "DESCRIPTION", col, pd, pa, ad)
                                AddParameter(cmd, "PRODUCT_SEARCH_CRITERIA_18", "DESCRIPTION", col, pd, pa, ad)
                                AddParameter(cmd, "PRODUCT_SEARCH_CRITERIA_19", "DESCRIPTION", col, pd, pa, ad)
                                AddParameter(cmd, "PRODUCT_SEARCH_CRITERIA_20", "DESCRIPTION", col, pd, pa, ad)
                                AddParameter(cmd, "PRODUCT_SEARCH_SWITCH_01", "BOOLEAN", col, pd, pa, ad)
                                AddParameter(cmd, "PRODUCT_SEARCH_SWITCH_02", "BOOLEAN", col, pd, pa, ad)
                                AddParameter(cmd, "PRODUCT_SEARCH_SWITCH_03", "BOOLEAN", col, pd, pa, ad)
                                AddParameter(cmd, "PRODUCT_SEARCH_SWITCH_04", "BOOLEAN", col, pd, pa, ad)
                                AddParameter(cmd, "PRODUCT_SEARCH_SWITCH_05", "BOOLEAN", col, pd, pa, ad)
                                AddParameter(cmd, "PRODUCT_SEARCH_SWITCH_06", "BOOLEAN", col, pd, pa, ad)
                                AddParameter(cmd, "PRODUCT_SEARCH_SWITCH_07", "BOOLEAN", col, pd, pa, ad)
                                AddParameter(cmd, "PRODUCT_SEARCH_SWITCH_08", "BOOLEAN", col, pd, pa, ad)
                                AddParameter(cmd, "PRODUCT_SEARCH_SWITCH_09", "BOOLEAN", col, pd, pa, ad)
                                AddParameter(cmd, "PRODUCT_SEARCH_SWITCH_10", "BOOLEAN", col, pd, pa, ad)
                                AddParameter(cmd, "PRODUCT_SEARCH_DATE_01", "DATE", col, pd, pa, ad)
                                AddParameter(cmd, "PRODUCT_SEARCH_DATE_02", "DATE", col, pd, pa, ad)
                                AddParameter(cmd, "PRODUCT_SEARCH_DATE_03", "DATE", col, pd, pa, ad)
                                AddParameter(cmd, "PRODUCT_SEARCH_DATE_04", "DATE", col, pd, pa, ad)
                                AddParameter(cmd, "PRODUCT_SEARCH_DATE_05", "DATE", col, pd, pa, ad)
                                AddParameter(cmd, "PRODUCT_TARIFF_CODE", "DESCRIPTION", col, pd, pa, ad)
                                .Add(New SqlParameter("@PRODUCT_OPTION_MASTER", SqlDbType.Bit)).Value = pr.MasterProduct
                                .Add(New SqlParameter("@ALTERNATE_SKU", SqlDbType.NVarChar)).Value = pr.AlternateSku
                                .Add(New SqlParameter("@AVAILABLE_ONLINE", SqlDbType.Bit)).Value = pr.AvailableOnline
                                .Add(New SqlParameter("@PERSONALISABLE", SqlDbType.Bit)).Value = False
                                .Add(New SqlParameter("@DISCONTINUED", SqlDbType.Bit)).Value = False
                                .Add(New SqlParameter("@PRODUCT_GLCODE_1", SqlDbType.NVarChar)).Value = pd.GLCode1
                                .Add(New SqlParameter("@PRODUCT_GLCODE_2", SqlDbType.NVarChar)).Value = pd.GLCode2
                                .Add(New SqlParameter("@PRODUCT_GLCODE_3", SqlDbType.NVarChar)).Value = pd.GLCode3
                                .Add(New SqlParameter("@PRODUCT_GLCODE_4", SqlDbType.NVarChar)).Value = pd.GLCode4
                                .Add(New SqlParameter("@PRODUCT_GLCODE_5", SqlDbType.NVarChar)).Value = pd.GLCode5
                                .Add(New SqlParameter("@PRODUCT_HTML_4", SqlDbType.NVarChar)).Value = pd.Html4
                                .Add(New SqlParameter("@PRODUCT_HTML_5", SqlDbType.NVarChar)).Value = pd.Html5
                                .Add(New SqlParameter("@PRODUCT_HTML_6", SqlDbType.NVarChar)).Value = pd.Html6
                                .Add(New SqlParameter("@PRODUCT_HTML_7", SqlDbType.NVarChar)).Value = pd.Html7
                                .Add(New SqlParameter("@PRODUCT_HTML_8", SqlDbType.NVarChar)).Value = pd.Html8
                                .Add(New SqlParameter("@PRODUCT_HTML_9", SqlDbType.NVarChar)).Value = pd.Html9
                            End With
                            Try
                                cmd.ExecuteNonQuery()
                            Catch ex As Exception
                                ResultDataSet = Nothing
                                Const strError As String = "Error during database access"
                                With err
                                    .ErrorMessage = ex.Message
                                    .ErrorStatus = strError
                                    .ErrorNumber = "TACDBPD-SQL2005-I" & ex.Message
                                    .HasError = True
                                End With
                            End Try
                        Next

                        ' Now Insert/Update/Delete the product table
                        strUpdate = "UPDATE tbl_product SET "
                        If Dep.UpdateDescriptions Then
                            strUpdate = strUpdate &
                                "PRODUCT_DESCRIPTION_1 = tbl_product_work.PRODUCT_DESCRIPTION_1, " &
                                "PRODUCT_DESCRIPTION_2 = tbl_product_work.PRODUCT_DESCRIPTION_2, " &
                                "PRODUCT_DESCRIPTION_3 = tbl_product_work.PRODUCT_DESCRIPTION_3, " &
                                "PRODUCT_DESCRIPTION_4 = tbl_product_work.PRODUCT_DESCRIPTION_4, " &
                                "PRODUCT_DESCRIPTION_5 = tbl_product_work.PRODUCT_DESCRIPTION_5, "

                        End If
                        strUpdate = strUpdate &
                            "PRODUCT_LENGTH = tbl_product_work.PRODUCT_LENGTH, " &
                            "PRODUCT_LENGTH_UOM = tbl_product_work.PRODUCT_LENGTH_UOM, " &
                            "PRODUCT_WIDTH = tbl_product_work.PRODUCT_WIDTH, " &
                            "PRODUCT_WIDTH_UOM = tbl_product_work.PRODUCT_WIDTH_UOM, " &
                            "PRODUCT_DEPTH = tbl_product_work.PRODUCT_DEPTH, " &
                            "PRODUCT_DEPTH_UOM = tbl_product_work.PRODUCT_DEPTH_UOM, " &
                            "PRODUCT_HEIGHT = tbl_product_work.PRODUCT_HEIGHT, " &
                            "PRODUCT_HEIGHT_UOM = tbl_product_work.PRODUCT_HEIGHT_UOM, " &
                            "PRODUCT_SIZE = tbl_product_work.PRODUCT_SIZE, " &
                            "PRODUCT_SIZE_UOM = tbl_product_work.PRODUCT_SIZE_UOM, " &
                            "PRODUCT_WEIGHT = tbl_product_work.PRODUCT_WEIGHT, " &
                            "PRODUCT_WEIGHT_UOM = tbl_product_work.PRODUCT_WEIGHT_UOM, " &
                            "PRODUCT_VOLUME = tbl_product_work.PRODUCT_VOLUME, " &
                            "PRODUCT_VOLUME_UOM = tbl_product_work.PRODUCT_VOLUME_UOM, " &
                            "PRODUCT_COLOUR = tbl_product_work.PRODUCT_COLOUR, " &
                            "PRODUCT_PACK_SIZE = tbl_product_work.PRODUCT_PACK_SIZE, " &
                            "PRODUCT_PACK_SIZE_UOM = tbl_product_work.PRODUCT_PACK_SIZE_UOM, " &
                            "PRODUCT_SUPPLIER_PART_NO = tbl_product_work.PRODUCT_SUPPLIER_PART_NO, " &
                            "PRODUCT_CUSTOMER_PART_NO = tbl_product_work.PRODUCT_CUSTOMER_PART_NO, " &
                            "PRODUCT_TASTING_NOTES_1 = tbl_product_work.PRODUCT_TASTING_NOTES_1, " &
                            "PRODUCT_TASTING_NOTES_2 = tbl_product_work.PRODUCT_TASTING_NOTES_2, " &
                            "PRODUCT_ABV = tbl_product_work.PRODUCT_ABV, " &
                            "PRODUCT_VINTAGE = tbl_product_work.PRODUCT_VINTAGE, " &
                            "PRODUCT_SUPPLIER = tbl_product_work.PRODUCT_SUPPLIER, " &
                            "PRODUCT_COUNTRY = tbl_product_work.PRODUCT_COUNTRY, " &
                            "PRODUCT_REGION = tbl_product_work.PRODUCT_REGION, " &
                            "PRODUCT_AREA = tbl_product_work.PRODUCT_AREA, " &
                            "PRODUCT_GRAPE = tbl_product_work.PRODUCT_GRAPE, " &
                            "PRODUCT_CLOSURE = tbl_product_work.PRODUCT_CLOSURE, " &
                            "PRODUCT_CATALOG_CODE = tbl_product_work.PRODUCT_CATALOG_CODE, " &
                            "PRODUCT_VEGETARIAN = tbl_product_work.PRODUCT_VEGETARIAN, " &
                            "PRODUCT_VEGAN = tbl_product_work.PRODUCT_VEGAN, " &
                            "PRODUCT_ORGANIC = tbl_product_work.PRODUCT_ORGANIC, " &
                            "PRODUCT_BIODYNAMIC = tbl_product_work.PRODUCT_BIODYNAMIC, " &
                            "PRODUCT_LUTTE = tbl_product_work.PRODUCT_LUTTE, " &
                            "PRODUCT_MINIMUM_AGE = tbl_product_work.PRODUCT_MINIMUM_AGE, "
                        If Dep.UpdateDescriptions Then
                            strUpdate = strUpdate &
                                "PRODUCT_HTML_1 = tbl_product_work.PRODUCT_HTML_1, " &
                                "PRODUCT_HTML_2 = tbl_product_work.PRODUCT_HTML_2, " &
                                "PRODUCT_HTML_3 = tbl_product_work.PRODUCT_HTML_3, " &
                                "PRODUCT_HTML_4 = tbl_product_work.PRODUCT_HTML_4, " &
                                "PRODUCT_HTML_5 = tbl_product_work.PRODUCT_HTML_5, " &
                                "PRODUCT_HTML_6 = tbl_product_work.PRODUCT_HTML_6, " &
                                "PRODUCT_HTML_7 = tbl_product_work.PRODUCT_HTML_7, " &
                                "PRODUCT_HTML_8 = tbl_product_work.PRODUCT_HTML_8, " &
                                "PRODUCT_HTML_9 = tbl_product_work.PRODUCT_HTML_9, "
                        End If
                        strUpdate = strUpdate &
                            "PRODUCT_SEARCH_KEYWORDS = tbl_product_work.PRODUCT_SEARCH_KEYWORDS, " &
                            "PRODUCT_PAGE_TITLE = tbl_product_work.PRODUCT_PAGE_TITLE, " &
                            "PRODUCT_META_DESCRIPTION = tbl_product_work.PRODUCT_META_DESCRIPTION, " &
                            "PRODUCT_META_KEYWORDS = tbl_product_work.PRODUCT_META_KEYWORDS, " &
                            "PRODUCT_SEARCH_RANGE_01 = tbl_product_work.PRODUCT_SEARCH_RANGE_01, " &
                            "PRODUCT_SEARCH_RANGE_02 = tbl_product_work.PRODUCT_SEARCH_RANGE_02, " &
                            "PRODUCT_SEARCH_RANGE_03 = tbl_product_work.PRODUCT_SEARCH_RANGE_03, " &
                            "PRODUCT_SEARCH_RANGE_04 = tbl_product_work.PRODUCT_SEARCH_RANGE_04, " &
                            "PRODUCT_SEARCH_RANGE_05 = tbl_product_work.PRODUCT_SEARCH_RANGE_05, " &
                            "PRODUCT_SEARCH_CRITERIA_01 = tbl_product_work.PRODUCT_SEARCH_CRITERIA_01, " &
                            "PRODUCT_SEARCH_CRITERIA_02 = tbl_product_work.PRODUCT_SEARCH_CRITERIA_02, " &
                            "PRODUCT_SEARCH_CRITERIA_03 = tbl_product_work.PRODUCT_SEARCH_CRITERIA_03, " &
                            "PRODUCT_SEARCH_CRITERIA_04 = tbl_product_work.PRODUCT_SEARCH_CRITERIA_04, " &
                            "PRODUCT_SEARCH_CRITERIA_05 = tbl_product_work.PRODUCT_SEARCH_CRITERIA_05, " &
                            "PRODUCT_SEARCH_CRITERIA_06 = tbl_product_work.PRODUCT_SEARCH_CRITERIA_06, " &
                            "PRODUCT_SEARCH_CRITERIA_07 = tbl_product_work.PRODUCT_SEARCH_CRITERIA_07, " &
                            "PRODUCT_SEARCH_CRITERIA_08 = tbl_product_work.PRODUCT_SEARCH_CRITERIA_08, " &
                            "PRODUCT_SEARCH_CRITERIA_09 = tbl_product_work.PRODUCT_SEARCH_CRITERIA_09, " &
                            "PRODUCT_SEARCH_CRITERIA_10 = tbl_product_work.PRODUCT_SEARCH_CRITERIA_10, " &
                            "PRODUCT_SEARCH_CRITERIA_11 = tbl_product_work.PRODUCT_SEARCH_CRITERIA_11, " &
                            "PRODUCT_SEARCH_CRITERIA_12 = tbl_product_work.PRODUCT_SEARCH_CRITERIA_12, " &
                            "PRODUCT_SEARCH_CRITERIA_13 = tbl_product_work.PRODUCT_SEARCH_CRITERIA_13, " &
                            "PRODUCT_SEARCH_CRITERIA_14 = tbl_product_work.PRODUCT_SEARCH_CRITERIA_14, " &
                            "PRODUCT_SEARCH_CRITERIA_15 = tbl_product_work.PRODUCT_SEARCH_CRITERIA_15, " &
                            "PRODUCT_SEARCH_CRITERIA_16 = tbl_product_work.PRODUCT_SEARCH_CRITERIA_16, " &
                            "PRODUCT_SEARCH_CRITERIA_17 = tbl_product_work.PRODUCT_SEARCH_CRITERIA_17, " &
                            "PRODUCT_SEARCH_CRITERIA_18 = tbl_product_work.PRODUCT_SEARCH_CRITERIA_18, " &
                            "PRODUCT_SEARCH_CRITERIA_19 = tbl_product_work.PRODUCT_SEARCH_CRITERIA_19, " &
                            "PRODUCT_SEARCH_CRITERIA_20 = tbl_product_work.PRODUCT_SEARCH_CRITERIA_20, " &
                            "PRODUCT_SEARCH_SWITCH_01 = tbl_product_work.PRODUCT_SEARCH_SWITCH_01, " &
                            "PRODUCT_SEARCH_SWITCH_02 = tbl_product_work.PRODUCT_SEARCH_SWITCH_02, " &
                            "PRODUCT_SEARCH_SWITCH_03 = tbl_product_work.PRODUCT_SEARCH_SWITCH_03, " &
                            "PRODUCT_SEARCH_SWITCH_04 = tbl_product_work.PRODUCT_SEARCH_SWITCH_04, " &
                            "PRODUCT_SEARCH_SWITCH_05 = tbl_product_work.PRODUCT_SEARCH_SWITCH_05, " &
                            "PRODUCT_SEARCH_SWITCH_06 = tbl_product_work.PRODUCT_SEARCH_SWITCH_06, " &
                            "PRODUCT_SEARCH_SWITCH_07 = tbl_product_work.PRODUCT_SEARCH_SWITCH_07, " &
                            "PRODUCT_SEARCH_SWITCH_08 = tbl_product_work.PRODUCT_SEARCH_SWITCH_08, " &
                            "PRODUCT_SEARCH_SWITCH_09 = tbl_product_work.PRODUCT_SEARCH_SWITCH_09, " &
                            "PRODUCT_SEARCH_SWITCH_10 = tbl_product_work.PRODUCT_SEARCH_SWITCH_10, " &
                            "PRODUCT_SEARCH_DATE_01 = tbl_product_work.PRODUCT_SEARCH_DATE_01, " &
                            "PRODUCT_SEARCH_DATE_02 = tbl_product_work.PRODUCT_SEARCH_DATE_02, " &
                            "PRODUCT_SEARCH_DATE_03 = tbl_product_work.PRODUCT_SEARCH_DATE_03, " &
                            "PRODUCT_SEARCH_DATE_04 = tbl_product_work.PRODUCT_SEARCH_DATE_04, " &
                            "PRODUCT_SEARCH_DATE_05 = tbl_product_work.PRODUCT_SEARCH_DATE_05, " &
                            "PRODUCT_TARIFF_CODE = tbl_product_work.PRODUCT_TARIFF_CODE, " &
                            "PRODUCT_OPTION_MASTER = tbl_product_work.PRODUCT_OPTION_MASTER, " &
                            "ALTERNATE_SKU = tbl_product_work.ALTERNATE_SKU, " &
                            "AVAILABLE_ONLINE = tbl_product_work.AVAILABLE_ONLINE, " &
                            "PRODUCT_GLCODE_1 = tbl_product_work.PRODUCT_GLCODE_1, " &
                            "PRODUCT_GLCODE_2 = tbl_product_work.PRODUCT_GLCODE_2, " &
                            "PRODUCT_GLCODE_3 = tbl_product_work.PRODUCT_GLCODE_3, " &
                            "PRODUCT_GLCODE_4 = tbl_product_work.PRODUCT_GLCODE_4, " &
                            "PRODUCT_GLCODE_5 = tbl_product_work.PRODUCT_GLCODE_5 " &
                            "FROM tbl_product_work " &
                            "WHERE tbl_product_work.PRODUCT_CODE = tbl_product.PRODUCT_CODE " &
                            "AND EXISTS " &
                            "(Select * FROM tbl_product " &
                            "WHERE tbl_product.PRODUCT_CODE = tbl_product_work.PRODUCT_CODE) "

                        strInsert = "INSERT INTO tbl_product " &
                            "(PRODUCT_CODE, " &
                            "PRODUCT_DESCRIPTION_1, " &
                            "PRODUCT_DESCRIPTION_2, " &
                            "PRODUCT_DESCRIPTION_3, " &
                            "PRODUCT_DESCRIPTION_4, " &
                            "PRODUCT_DESCRIPTION_5, " &
                            "PRODUCT_LENGTH, " &
                            "PRODUCT_LENGTH_UOM, " &
                            "PRODUCT_WIDTH, " &
                            "PRODUCT_WIDTH_UOM, " &
                            "PRODUCT_DEPTH, " &
                            "PRODUCT_DEPTH_UOM, " &
                            "PRODUCT_HEIGHT, " &
                            "PRODUCT_HEIGHT_UOM, " &
                            "PRODUCT_SIZE, " &
                            "PRODUCT_SIZE_UOM, " &
                            "PRODUCT_WEIGHT, " &
                            "PRODUCT_WEIGHT_UOM, " &
                            "PRODUCT_VOLUME, " &
                            "PRODUCT_VOLUME_UOM, " &
                            "PRODUCT_COLOUR, " &
                            "PRODUCT_PACK_SIZE, " &
                            "PRODUCT_PACK_SIZE_UOM, " &
                            "PRODUCT_SUPPLIER_PART_NO, " &
                            "PRODUCT_CUSTOMER_PART_NO, " &
                            "PRODUCT_TASTING_NOTES_1, " &
                            "PRODUCT_TASTING_NOTES_2, " &
                            "PRODUCT_ABV, " &
                            "PRODUCT_VINTAGE, " &
                            "PRODUCT_SUPPLIER, " &
                            "PRODUCT_COUNTRY, " &
                            "PRODUCT_REGION, " &
                            "PRODUCT_AREA, " &
                            "PRODUCT_GRAPE, " &
                            "PRODUCT_CLOSURE, " &
                            "PRODUCT_CATALOG_CODE, " &
                            "PRODUCT_VEGETARIAN, " &
                            "PRODUCT_VEGAN, " &
                            "PRODUCT_ORGANIC, " &
                            "PRODUCT_BIODYNAMIC, " &
                            "PRODUCT_LUTTE, " &
                            "PRODUCT_MINIMUM_AGE, " &
                            "PRODUCT_HTML_1, " &
                            "PRODUCT_HTML_2, " &
                            "PRODUCT_HTML_3, " &
                            "PRODUCT_SEARCH_KEYWORDS, " &
                            "PRODUCT_PAGE_TITLE, " &
                            "PRODUCT_META_DESCRIPTION, " &
                            "PRODUCT_META_KEYWORDS, " &
                            "PRODUCT_SEARCH_RANGE_01, " &
                            "PRODUCT_SEARCH_RANGE_02, " &
                            "PRODUCT_SEARCH_RANGE_03, " &
                            "PRODUCT_SEARCH_RANGE_04, " &
                            "PRODUCT_SEARCH_RANGE_05, " &
                            "PRODUCT_SEARCH_CRITERIA_01, " &
                            "PRODUCT_SEARCH_CRITERIA_02, " &
                            "PRODUCT_SEARCH_CRITERIA_03, " &
                            "PRODUCT_SEARCH_CRITERIA_04, " &
                            "PRODUCT_SEARCH_CRITERIA_05, " &
                            "PRODUCT_SEARCH_CRITERIA_06, " &
                            "PRODUCT_SEARCH_CRITERIA_07, " &
                            "PRODUCT_SEARCH_CRITERIA_08, " &
                            "PRODUCT_SEARCH_CRITERIA_09, " &
                            "PRODUCT_SEARCH_CRITERIA_10, " &
                            "PRODUCT_SEARCH_CRITERIA_11, " &
                            "PRODUCT_SEARCH_CRITERIA_12, " &
                            "PRODUCT_SEARCH_CRITERIA_13, " &
                            "PRODUCT_SEARCH_CRITERIA_14, " &
                            "PRODUCT_SEARCH_CRITERIA_15, " &
                            "PRODUCT_SEARCH_CRITERIA_16, " &
                            "PRODUCT_SEARCH_CRITERIA_17, " &
                            "PRODUCT_SEARCH_CRITERIA_18, " &
                            "PRODUCT_SEARCH_CRITERIA_19, " &
                            "PRODUCT_SEARCH_CRITERIA_20, " &
                            "PRODUCT_SEARCH_SWITCH_01, " &
                            "PRODUCT_SEARCH_SWITCH_02, " &
                            "PRODUCT_SEARCH_SWITCH_03, " &
                            "PRODUCT_SEARCH_SWITCH_04, " &
                            "PRODUCT_SEARCH_SWITCH_05, " &
                            "PRODUCT_SEARCH_SWITCH_06, " &
                            "PRODUCT_SEARCH_SWITCH_07, " &
                            "PRODUCT_SEARCH_SWITCH_08, " &
                            "PRODUCT_SEARCH_SWITCH_09, " &
                            "PRODUCT_SEARCH_SWITCH_10, " &
                            "PRODUCT_SEARCH_DATE_01, " &
                            "PRODUCT_SEARCH_DATE_02, " &
                            "PRODUCT_SEARCH_DATE_03, " &
                            "PRODUCT_SEARCH_DATE_04, " &
                            "PRODUCT_SEARCH_DATE_05, " &
                            "PRODUCT_TARIFF_CODE, " &
                            "PRODUCT_OPTION_MASTER, " &
                            "ALTERNATE_SKU, " &
                            "AVAILABLE_ONLINE, " &
                            "PRODUCT_GLCODE_1, " &
                            "PRODUCT_GLCODE_2, " &
                            "PRODUCT_GLCODE_3, " &
                            "PRODUCT_GLCODE_4, " &
                            "PRODUCT_GLCODE_5, " &
                            "PRODUCT_HTML_4, " &
                            "PRODUCT_HTML_5, " &
                            "PRODUCT_HTML_6, " &
                            "PRODUCT_HTML_7, " &
                            "PRODUCT_HTML_8, " &
                            "PRODUCT_HTML_9 ) " &
                            "SELECT " &
                            "PRODUCT_CODE, " &
                            "PRODUCT_DESCRIPTION_1, " &
                            "PRODUCT_DESCRIPTION_2, " &
                            "PRODUCT_DESCRIPTION_3, " &
                            "PRODUCT_DESCRIPTION_4, " &
                            "PRODUCT_DESCRIPTION_5, " &
                            "PRODUCT_LENGTH, " &
                            "PRODUCT_LENGTH_UOM, " &
                            "PRODUCT_WIDTH, " &
                            "PRODUCT_WIDTH_UOM, " &
                            "PRODUCT_DEPTH, " &
                            "PRODUCT_DEPTH_UOM, " &
                            "PRODUCT_HEIGHT, " &
                            "PRODUCT_HEIGHT_UOM, " &
                            "PRODUCT_SIZE, " &
                            "PRODUCT_SIZE_UOM, " &
                            "PRODUCT_WEIGHT, " &
                            "PRODUCT_WEIGHT_UOM, " &
                            "PRODUCT_VOLUME, " &
                            "PRODUCT_VOLUME_UOM, " &
                            "PRODUCT_COLOUR, " &
                            "PRODUCT_PACK_SIZE, " &
                            "PRODUCT_PACK_SIZE_UOM, " &
                            "PRODUCT_SUPPLIER_PART_NO, " &
                            "PRODUCT_CUSTOMER_PART_NO, " &
                            "PRODUCT_TASTING_NOTES_1, " &
                            "PRODUCT_TASTING_NOTES_2, " &
                            "PRODUCT_ABV, " &
                            "PRODUCT_VINTAGE, " &
                            "PRODUCT_SUPPLIER, " &
                            "PRODUCT_COUNTRY, " &
                            "PRODUCT_REGION, " &
                            "PRODUCT_AREA, " &
                            "PRODUCT_GRAPE, " &
                            "PRODUCT_CLOSURE, " &
                            "PRODUCT_CATALOG_CODE, " &
                            "PRODUCT_VEGETARIAN, " &
                            "PRODUCT_VEGAN, " &
                            "PRODUCT_ORGANIC, " &
                            "PRODUCT_BIODYNAMIC, " &
                            "PRODUCT_LUTTE, " &
                            "PRODUCT_MINIMUM_AGE, " &
                            "PRODUCT_HTML_1, " &
                            "PRODUCT_HTML_2, " &
                            "PRODUCT_HTML_3, " &
                            "PRODUCT_SEARCH_KEYWORDS, " &
                            "PRODUCT_PAGE_TITLE, " &
                            "PRODUCT_META_DESCRIPTION, " &
                            "PRODUCT_META_KEYWORDS, " &
                            "PRODUCT_SEARCH_RANGE_01, " &
                            "PRODUCT_SEARCH_RANGE_02, " &
                            "PRODUCT_SEARCH_RANGE_03, " &
                            "PRODUCT_SEARCH_RANGE_04, " &
                            "PRODUCT_SEARCH_RANGE_05, " &
                            "PRODUCT_SEARCH_CRITERIA_01, " &
                            "PRODUCT_SEARCH_CRITERIA_02, " &
                            "PRODUCT_SEARCH_CRITERIA_03, " &
                            "PRODUCT_SEARCH_CRITERIA_04, " &
                            "PRODUCT_SEARCH_CRITERIA_05, " &
                            "PRODUCT_SEARCH_CRITERIA_06, " &
                            "PRODUCT_SEARCH_CRITERIA_07, " &
                            "PRODUCT_SEARCH_CRITERIA_08, " &
                            "PRODUCT_SEARCH_CRITERIA_09, " &
                            "PRODUCT_SEARCH_CRITERIA_10, " &
                            "PRODUCT_SEARCH_CRITERIA_11, " &
                            "PRODUCT_SEARCH_CRITERIA_12, " &
                            "PRODUCT_SEARCH_CRITERIA_13, " &
                            "PRODUCT_SEARCH_CRITERIA_14, " &
                            "PRODUCT_SEARCH_CRITERIA_15, " &
                            "PRODUCT_SEARCH_CRITERIA_16, " &
                            "PRODUCT_SEARCH_CRITERIA_17, " &
                            "PRODUCT_SEARCH_CRITERIA_18, " &
                            "PRODUCT_SEARCH_CRITERIA_19, " &
                            "PRODUCT_SEARCH_CRITERIA_20, " &
                            "PRODUCT_SEARCH_SWITCH_01, " &
                            "PRODUCT_SEARCH_SWITCH_02, " &
                            "PRODUCT_SEARCH_SWITCH_03, " &
                            "PRODUCT_SEARCH_SWITCH_04, " &
                            "PRODUCT_SEARCH_SWITCH_05, " &
                            "PRODUCT_SEARCH_SWITCH_06, " &
                            "PRODUCT_SEARCH_SWITCH_07, " &
                            "PRODUCT_SEARCH_SWITCH_08, " &
                            "PRODUCT_SEARCH_SWITCH_09, " &
                            "PRODUCT_SEARCH_SWITCH_10, " &
                            "PRODUCT_SEARCH_DATE_01, " &
                            "PRODUCT_SEARCH_DATE_02, " &
                            "PRODUCT_SEARCH_DATE_03, " &
                            "PRODUCT_SEARCH_DATE_04, " &
                            "PRODUCT_SEARCH_DATE_05, " &
                            "PRODUCT_TARIFF_CODE, " &
                            "PRODUCT_OPTION_MASTER, " &
                            "ALTERNATE_SKU, " &
                            "AVAILABLE_ONLINE, " &
                            "PRODUCT_GLCODE_1, " &
                            "PRODUCT_GLCODE_2, " &
                            "PRODUCT_GLCODE_3, " &
                            "PRODUCT_GLCODE_4, " &
                            "PRODUCT_GLCODE_5, " &
                            "PRODUCT_HTML_4, " &
                            "PRODUCT_HTML_5, " &
                            "PRODUCT_HTML_6, " &
                            "PRODUCT_HTML_7, " &
                            "PRODUCT_HTML_8, " &
                            "PRODUCT_HTML_9 " &
                            "FROM tbl_product_work " &
                            "WHERE NOT EXISTS " &
                            "(SELECT * FROM tbl_product " &
                            "WHERE tbl_product.PRODUCT_CODE = tbl_product_work.PRODUCT_CODE)"

                        strDelete = "DELETE FROM tbl_product " &
                            "WHERE NOT EXISTS (" &
                            "SELECT * FROM tbl_product_work WHERE " &
                            "tbl_product_work.PRODUCT_CODE = tbl_product.PRODUCT_CODE)"

                        Try
                            cmd = New SqlCommand(strUpdate, conSql2005)
                            cmd.ExecuteNonQuery()
                        Catch ex As Exception
                            ResultDataSet = Nothing
                            Const strError As String = "Error during database access"
                            With err
                                .ErrorMessage = ex.Message
                                .ErrorStatus = strError
                                .ErrorNumber = "TACDBPD-SQL2005-G"
                                .HasError = True
                            End With
                        End Try

                        Try
                            cmd = New SqlCommand(strInsert, conSql2005)
                            cmd.ExecuteNonQuery()
                        Catch ex As Exception
                            ResultDataSet = Nothing
                            Const strError As String = "Error during database access"
                            With err
                                .ErrorMessage = ex.Message
                                .ErrorStatus = strError
                                .ErrorNumber = "TACDBPD-SQL2005-F"
                                .HasError = True
                            End With
                        End Try

                        Try
                            cmd = New SqlCommand(strDelete, conSql2005)
                            cmd.ExecuteNonQuery()
                        Catch ex As Exception
                            ResultDataSet = Nothing
                            Const strError As String = "Error during database access"
                            With err
                                .ErrorMessage = ex.Message
                                .ErrorStatus = strError
                                .ErrorNumber = "TACDBPD-SQL2005-E"
                                .HasError = True
                            End With
                        End Try
                End Select

            End If

        Catch ex As Exception
            ResultDataSet = Nothing
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBPD-SQL2005-A"
                .HasError = True
            End With
        End Try

        Return err
    End Function
#End Region

    Private Sub AddParameter(ByRef cmd As SqlCommand, ByVal att As String, ByVal type As String, ByVal col As Collection, ByVal pd As DEProductDescriptions, ByVal pa As DEProductEcommerceAttribute, ByVal ad As attributeDetails)
        With cmd.Parameters
            Try
                pa = Nothing
                ad = Nothing
                If col.Contains(att) Then
                    pa = col(att)
                    ad = GetAttributeDetails(pa.Attribute, pd.Language)
                End If
            Catch ex As Exception
            End Try
            If Not pa Is Nothing And Not ad Is Nothing Then
                Select Case type
                    Case Is = "VALUE"
                        .Add(New SqlParameter("@" & att, SqlDbType.Decimal)).Value = ad.attributeValue
                    Case Is = "DESCRIPTION"
                        .Add(New SqlParameter("@" & att, SqlDbType.NVarChar)).Value = ad.attributeDescription
                    Case Is = "Date"
                        .Add(New SqlParameter("@" & att, SqlDbType.DateTime)).Value = ad.attributeDate
                    Case Is = "Boolean"
                        .Add(New SqlParameter("@" & att, SqlDbType.Bit)).Value = ad.attributeBoolean
                End Select
            Else
                Select Case type
                    Case Is = "VALUE"
                        .Add(New SqlParameter("@" & att, SqlDbType.Decimal)).Value = 0
                    Case Is = "DESCRIPTION"
                        .Add(New SqlParameter("@" & att, SqlDbType.NVarChar)).Value = ""
                    Case Is = "DATE"
                        .Add(New SqlParameter("@" & att, SqlDbType.DateTime)).Value = "01/01/1900"
                    Case Is = "BOOLEAN"
                        .Add(New SqlParameter("@" & att, SqlDbType.Bit)).Value = False
                End Select
            End If
        End With
    End Sub

    Private Function GetAttributeDetails(ByVal attributeCode As String, ByVal language As String)
        Dim ad As attributeDetails = Nothing
        Dim cmdAttribute As SqlCommand
        Dim dtrAttribute As SqlDataReader

        cmdAttribute = New SqlCommand("SELECT * FROM tbl_attribute_lang WHERE ATTRIBUTE_CODE = @ATTRIBUTE_CODE AND LANGUAGE = @LANGUAGE", conSql2005)
        With cmdAttribute.Parameters
            .Clear()
            .Add(New SqlParameter("@ATTRIBUTE_CODE", SqlDbType.NVarChar)).Value = attributeCode
            .Add(New SqlParameter("@LANGUAGE", SqlDbType.NVarChar)).Value = language
        End With

        dtrAttribute = cmdAttribute.ExecuteReader
        If dtrAttribute.HasRows Then
            ad = New attributeDetails
            While dtrAttribute.Read()
                ad.attributeDescription = dtrAttribute("DESCRIPTION")
                ad.attributeValue = dtrAttribute("VALUE")
                ad.attributeDate = dtrAttribute("Date")
                ad.attributeBoolean = dtrAttribute("Boolean")
            End While
        End If
        dtrAttribute.Close()

        Return ad
    End Function
    Private Function AccessDatabaseSQL2005_ProductRelationsLoad() As ErrorObj
        Dim err As New ErrorObj
        Const strDelete = "DELETE FROM TBL_PRODUCT_RELATIONS_WORK "
        Dim cmd As SqlCommand
        Dim dtrReader As SqlDataReader
        Const strSelect = "SELECT * FROM TBL_PRODUCT_RELATIONS WHERE " &
                            " BUSINESS_UNIT = @BUSINESS_UNIT AND PARTNER = @PARTNER AND " &
                            " GROUP_L01_GROUP = @GROUP_L01_GROUP AND GROUP_L02_GROUP = @GROUP_L02_GROUP AND " &
                            " GROUP_L03_GROUP = @GROUP_L03_GROUP AND GROUP_L04_GROUP = @GROUP_L04_GROUP AND " &
                            " GROUP_L05_GROUP = @GROUP_L05_GROUP AND GROUP_L06_GROUP = @GROUP_L06_GROUP AND " &
                            " GROUP_L07_GROUP = @GROUP_L07_GROUP AND GROUP_L08_GROUP = @GROUP_L08_GROUP AND " &
                            " GROUP_L09_GROUP = @GROUP_L09_GROUP AND GROUP_L10_GROUP = @GROUP_L10_GROUP AND " &
                            " PRODUCT = @PRODUCT AND RELATED_GROUP_L01_GROUP = @RELATED_GROUP_L01_GROUP AND " &
                            " RELATED_GROUP_L02_GROUP = @RELATED_GROUP_L02_GROUP AND RELATED_GROUP_L03_GROUP = @RELATED_GROUP_L03_GROUP AND " &
                            " RELATED_GROUP_L04_GROUP = @RELATED_GROUP_L04_GROUP AND RELATED_GROUP_L05_GROUP = @RELATED_GROUP_L05_GROUP AND " &
                            " RELATED_GROUP_L06_GROUP = @RELATED_GROUP_L06_GROUP AND RELATED_GROUP_L07_GROUP = @RELATED_GROUP_L07_GROUP AND " &
                            " RELATED_GROUP_L08_GROUP = @RELATED_GROUP_L08_GROUP AND RELATED_GROUP_L09_GROUP = @RELATED_GROUP_L09_GROUP AND " &
                            " RELATED_GROUP_L10_GROUP = @RELATED_GROUP_L10_GROUP AND RELATED_PRODUCT = @RELATED_PRODUCT "

        Const strDelete2 = "DELETE FROM TBL_PRODUCT_RELATIONS WHERE " &
                           " BUSINESS_UNIT = @BUSINESS_UNIT AND PARTNER = @PARTNER AND " &
                           " GROUP_L01_GROUP = @GROUP_L01_GROUP AND GROUP_L02_GROUP = @GROUP_L02_GROUP AND " &
                           " GROUP_L03_GROUP = @GROUP_L03_GROUP AND GROUP_L04_GROUP = @GROUP_L04_GROUP AND " &
                           " GROUP_L05_GROUP = @GROUP_L05_GROUP AND GROUP_L06_GROUP = @GROUP_L06_GROUP AND " &
                           " GROUP_L07_GROUP = @GROUP_L07_GROUP AND GROUP_L08_GROUP = @GROUP_L08_GROUP AND " &
                           " GROUP_L09_GROUP = @GROUP_L09_GROUP AND GROUP_L10_GROUP = @GROUP_L10_GROUP AND " &
                           " PRODUCT = @PRODUCT AND RELATED_GROUP_L01_GROUP = @RELATED_GROUP_L01_GROUP AND " &
                           " RELATED_GROUP_L02_GROUP = @RELATED_GROUP_L02_GROUP AND RELATED_GROUP_L03_GROUP = @RELATED_GROUP_L03_GROUP AND " &
                           " RELATED_GROUP_L04_GROUP = @RELATED_GROUP_L04_GROUP AND RELATED_GROUP_L05_GROUP = @RELATED_GROUP_L05_GROUP AND " &
                           " RELATED_GROUP_L06_GROUP = @RELATED_GROUP_L06_GROUP AND RELATED_GROUP_L07_GROUP = @RELATED_GROUP_L07_GROUP AND " &
                           " RELATED_GROUP_L08_GROUP = @RELATED_GROUP_L08_GROUP AND RELATED_GROUP_L09_GROUP = @RELATED_GROUP_L09_GROUP AND " &
                           " RELATED_GROUP_L10_GROUP = @RELATED_GROUP_L10_GROUP AND RELATED_PRODUCT = @RELATED_PRODUCT "


        Const strInsertProductRelation As String = "INSERT INTO tbl_product_relations " &
                                                    "(QUALIFIER, BUSINESS_UNIT, PARTNER, GROUP_L01_GROUP " &
                      ", GROUP_L02_GROUP, GROUP_L03_GROUP, GROUP_L04_GROUP , GROUP_L05_GROUP , GROUP_L06_GROUP " &
                      ", GROUP_L07_GROUP , GROUP_L08_GROUP , GROUP_L09_GROUP , GROUP_L10_GROUP , PRODUCT " &
                      ", RELATED_GROUP_L01_GROUP , RELATED_GROUP_L02_GROUP , RELATED_GROUP_L03_GROUP , RELATED_GROUP_L04_GROUP " &
                      ", RELATED_GROUP_L05_GROUP , RELATED_GROUP_L06_GROUP , RELATED_GROUP_L07_GROUP , RELATED_GROUP_L08_GROUP, RELATED_GROUP_L09_GROUP " &
                      ", RELATED_GROUP_L10_GROUP , RELATED_PRODUCT , SEQUENCE) " &
                      " VALUES (@QUALIFIER, @BUSINESS_UNIT, @PARTNER, @GROUP_L01_GROUP, @GROUP_L02_GROUP, @GROUP_L03_GROUP, " &
                      "  @GROUP_L04_GROUP , @GROUP_L05_GROUP , @GROUP_L06_GROUP " &
                      ", @GROUP_L07_GROUP , @GROUP_L08_GROUP , @GROUP_L09_GROUP , @GROUP_L10_GROUP , @PRODUCT " &
                      ", @RELATED_GROUP_L01_GROUP , @RELATED_GROUP_L02_GROUP , @RELATED_GROUP_L03_GROUP , @RELATED_GROUP_L04_GROUP " &
                      ", @RELATED_GROUP_L05_GROUP , @RELATED_GROUP_L06_GROUP , @RELATED_GROUP_L07_GROUP , @RELATED_GROUP_L08_GROUP, @RELATED_GROUP_L09_GROUP " &
                      ", @RELATED_GROUP_L10_GROUP , @RELATED_PRODUCT , @SEQUENCE)"
        Try

            For Each deProductRelationCollection As DEProductRelationCollection In DeProductRelations.ProductRelationCollection
                If deProductRelationCollection.Mode = "REPLACE" Then
                    '-------------------
                    ' Delete work tables
                    '-------------------
                    cmd = New SqlCommand(strDelete, conSql2005)
                    cmd.ExecuteNonQuery()
                End If
                '--------------------------------------
                ' Look through relations in current set
                '--------------------------------------
                For Each productRelation As DEProductRelation In deProductRelationCollection.ProductRelations
                    '-------------------------------------------
                    ' If replace then just write to the workfile
                    '-------------------------------------------
                    If deProductRelationCollection.Mode = "REPLACE" Then
                        cmd = New SqlCommand(Replace(strInsertProductRelation, "tbl_product_relations", "tbl_product_relations_work"), conSql2005)
                        AccessDatabaseSQL2005_ProductRelationsLoad_SetParms(cmd.Parameters, productRelation, deProductRelationCollection)

                        cmd.ExecuteNonQuery()
                    Else
                        Select Case productRelation.Mode
                            Case Is = "ADD"
                                '--------------------------
                                'Check if it already exists
                                '--------------------------
                                cmd = New SqlCommand(strSelect, conSql2005)
                                AccessDatabaseSQL2005_ProductRelationsLoad_SetParms(cmd.Parameters, productRelation, deProductRelationCollection)

                                dtrReader = cmd.ExecuteReader
                                If Not dtrReader.HasRows Then
                                    dtrReader.Close()

                                    cmd = New SqlCommand(strInsertProductRelation, conSql2005)
                                    AccessDatabaseSQL2005_ProductRelationsLoad_SetParms(cmd.Parameters, productRelation, deProductRelationCollection)

                                    cmd.ExecuteNonQuery()
                                Else
                                    dtrReader.Close()
                                End If
                            Case Is = "UPDATE"
                                '-------------------------
                                ' What is there to update?
                                '-------------------------
                            Case Is = "DELETE"
                                cmd = New SqlCommand(strDelete2, conSql2005)
                                AccessDatabaseSQL2005_ProductRelationsLoad_SetParms(cmd.Parameters, productRelation, deProductRelationCollection)

                                cmd.ExecuteNonQuery()

                        End Select
                    End If

                Next
                '----------------------------------------------
                ' If REPLACE then now need to call datatransfer
                ' to write from work files to main files
                '----------------------------------------------
                If deProductRelationCollection.Mode = "REPLACE" Then
                    DataTransfer.SQLConnectionString = Settings.FrontEndConnectionString
                    DataTransfer.DoUpdateTable_EBPRRL(deProductRelationCollection.BusinessUnit, deProductRelationCollection.Partner)
                End If
            Next
        Catch ex As Exception
            ResultDataSet = Nothing
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBPD-SQL2005-3"
                .HasError = True
            End With
        End Try

        Return err
    End Function
    Private Function AccessDatabaseSQL2005_ProductRelationsLoad_SetParms(ByRef cmdParameters As SqlParameterCollection, ByVal productRelation As DEProductRelation, ByVal productRelationCollection As DEProductRelationCollection) As ErrorObj
        Dim err As ErrorObj = Nothing

        Dim lastProductAfterLevel As Integer = CInt(productRelationCollection.LastProductAfterLevel)
        '--------------------------
        ' Fill in any *EMPTY groups
        '--------------------------
        If lastProductAfterLevel > 1 AndAlso productRelation.ProductInfo.L02Group = String.Empty Then
            productRelation.ProductInfo.L02Group = "*EMPTY"
        End If
        If lastProductAfterLevel > 2 AndAlso productRelation.ProductInfo.L03Group = String.Empty Then
            productRelation.ProductInfo.L03Group = "*EMPTY"
        End If
        If lastProductAfterLevel > 3 AndAlso productRelation.ProductInfo.L04Group = String.Empty Then
            productRelation.ProductInfo.L04Group = "*EMPTY"
        End If
        If lastProductAfterLevel > 4 AndAlso productRelation.ProductInfo.L05Group = String.Empty Then
            productRelation.ProductInfo.L05Group = "*EMPTY"
        End If
        If lastProductAfterLevel > 5 AndAlso productRelation.ProductInfo.L06Group = String.Empty Then
            productRelation.ProductInfo.L06Group = "*EMPTY"
        End If
        If lastProductAfterLevel > 6 AndAlso productRelation.ProductInfo.L07Group = String.Empty Then
            productRelation.ProductInfo.L07Group = "*EMPTY"
        End If
        If lastProductAfterLevel > 8 AndAlso productRelation.ProductInfo.L09Group = String.Empty Then
            productRelation.ProductInfo.L09Group = "*EMPTY"
        End If
        If lastProductAfterLevel > 9 AndAlso productRelation.ProductInfo.L10Group = String.Empty Then
            productRelation.ProductInfo.L10Group = "*EMPTY"
        End If
        If lastProductAfterLevel > 1 AndAlso productRelation.RelatedProductInfo.L02Group = String.Empty Then
            productRelation.RelatedProductInfo.L02Group = "*EMPTY"
        End If
        If lastProductAfterLevel > 2 AndAlso productRelation.RelatedProductInfo.L03Group = String.Empty Then
            productRelation.RelatedProductInfo.L03Group = "*EMPTY"
        End If
        If lastProductAfterLevel > 3 AndAlso productRelation.RelatedProductInfo.L04Group = String.Empty Then
            productRelation.RelatedProductInfo.L04Group = "*EMPTY"
        End If
        If lastProductAfterLevel > 4 AndAlso productRelation.RelatedProductInfo.L05Group = String.Empty Then
            productRelation.RelatedProductInfo.L05Group = "*EMPTY"
        End If
        If lastProductAfterLevel > 5 AndAlso productRelation.RelatedProductInfo.L06Group = String.Empty Then
            productRelation.RelatedProductInfo.L06Group = "*EMPTY"
        End If
        If lastProductAfterLevel > 6 AndAlso productRelation.RelatedProductInfo.L07Group = String.Empty Then
            productRelation.RelatedProductInfo.L07Group = "*EMPTY"
        End If
        If lastProductAfterLevel > 8 AndAlso productRelation.RelatedProductInfo.L09Group = String.Empty Then
            productRelation.RelatedProductInfo.L09Group = "*EMPTY"
        End If
        If lastProductAfterLevel > 9 AndAlso productRelation.RelatedProductInfo.L10Group = String.Empty Then
            productRelation.RelatedProductInfo.L10Group = "*EMPTY"
        End If
        With cmdParameters
            .Add("@QUALIFIER", SqlDbType.NVarChar).Value = productRelation.Qualifier
            .Add("@BUSINESS_UNIT", SqlDbType.NVarChar).Value = productRelationCollection.BusinessUnit
            .Add("@PARTNER", SqlDbType.NVarChar).Value = productRelationCollection.Partner
            .Add("@GROUP_L01_GROUP", SqlDbType.NVarChar).Value = productRelation.ProductInfo.L01Group
            .Add("@GROUP_L02_GROUP", SqlDbType.NVarChar).Value = productRelation.ProductInfo.L02Group
            .Add("@GROUP_L03_GROUP", SqlDbType.NVarChar).Value = productRelation.ProductInfo.L03Group
            .Add("@GROUP_L04_GROUP", SqlDbType.NVarChar).Value = productRelation.ProductInfo.L04Group
            .Add("@GROUP_L05_GROUP", SqlDbType.NVarChar).Value = productRelation.ProductInfo.L05Group
            .Add("@GROUP_L06_GROUP", SqlDbType.NVarChar).Value = productRelation.ProductInfo.L06Group
            .Add("@GROUP_L07_GROUP", SqlDbType.NVarChar).Value = productRelation.ProductInfo.L07Group
            .Add("@GROUP_L08_GROUP", SqlDbType.NVarChar).Value = productRelation.ProductInfo.L08Group
            .Add("@GROUP_L09_GROUP", SqlDbType.NVarChar).Value = productRelation.ProductInfo.L09Group
            .Add("@GROUP_L10_GROUP", SqlDbType.NVarChar).Value = productRelation.ProductInfo.L10Group
            .Add("@PRODUCT", SqlDbType.NVarChar).Value = CType(productRelation.ProductInfo.Products.Item(1), DEProductEcommerceDetails).Sku
            .Add("@RELATED_GROUP_L01_GROUP", SqlDbType.NVarChar).Value = productRelation.RelatedProductInfo.L01Group
            .Add("@RELATED_GROUP_L02_GROUP", SqlDbType.NVarChar).Value = productRelation.RelatedProductInfo.L02Group
            .Add("@RELATED_GROUP_L03_GROUP", SqlDbType.NVarChar).Value = productRelation.RelatedProductInfo.L03Group
            .Add("@RELATED_GROUP_L04_GROUP", SqlDbType.NVarChar).Value = productRelation.RelatedProductInfo.L04Group
            .Add("@RELATED_GROUP_L05_GROUP", SqlDbType.NVarChar).Value = productRelation.RelatedProductInfo.L05Group
            .Add("@RELATED_GROUP_L06_GROUP", SqlDbType.NVarChar).Value = productRelation.RelatedProductInfo.L06Group
            .Add("@RELATED_GROUP_L07_GROUP", SqlDbType.NVarChar).Value = productRelation.RelatedProductInfo.L07Group
            .Add("@RELATED_GROUP_L08_GROUP", SqlDbType.NVarChar).Value = productRelation.RelatedProductInfo.L08Group
            .Add("@RELATED_GROUP_L09_GROUP", SqlDbType.NVarChar).Value = productRelation.RelatedProductInfo.L09Group
            .Add("@RELATED_GROUP_L10_GROUP", SqlDbType.NVarChar).Value = productRelation.RelatedProductInfo.L10Group
            .Add("@RELATED_PRODUCT", SqlDbType.NVarChar).Value = CType(productRelation.RelatedProductInfo.Products.Item(1), DEProductEcommerceDetails).Sku
            .Add("@SEQUENCE", SqlDbType.NVarChar).Value = ""

        End With
        Return err
    End Function

    Private Function AccessDatabaseSQL2005_ProductPriceLoad() As ErrorObj
        Dim err As New ErrorObj
        Dim cmd As New SqlCommand
        Dim dtrReader As SqlDataReader
        '------------------
        ' PROCESS TAX CODES
        '------------------
        '------------------------------------------
        ' If replace then delete existing Tax codes
        '------------------------------------------
        Const strDeleteTaxCodes As String = "DELETE FROM tbl_tax_code "
        Const strSelectTaxCode As String = "Select * FROM tbl_tax_code where " &
                                                " TAX_CODE = @TAX_CODE  "
        Const strUpdateTaxCode As String = "UPDATE tbl_tax_code Set TAX_CODE_DESCRIPTION=@TAX_CODE_DESCRIPTION where " &
                                                " TAX_CODE = @TAX_CODE"
        Const strDeleteTaxCode2 As String = "DELETE FROM  tbl_tax_code where " &
                                            " TAX_CODE= @TAX_CODE "
        Const strAddTaxCode As String = "INSERT INTO tbl_tax_code([TAX_CODE], [TAX_CODE_DESCRIPTION],[TAX_PERCENTAGE]) " &
                                            "   VALUES(@TAX_CODE, @TAX_CODE_DESCRIPTION, @TAX_PERCENTAGE)"
        Dim taxCodeExists As Boolean = False
        If DeProductPriceLoad.TaxCodeMode = "REPLACE" Then
            cmd = New SqlCommand(strDeleteTaxCodes, conSql2005)
            cmd.ExecuteNonQuery()
        End If
        For Each deTaxCode As DETaxCode In DeProductPriceLoad.ColTaxCodes
            ' Check it exists
            taxCodeExists = False
            cmd = New SqlCommand(strSelectTaxCode, conSql2005)
            cmd.Parameters.Add("@TAX_CODE", SqlDbType.NVarChar, 20).Value = deTaxCode.TaxCode
            dtrReader = cmd.ExecuteReader
            If dtrReader.HasRows Then
                taxCodeExists = True
            End If

            dtrReader.Close()
            If deTaxCode.Mode = String.Empty Then
                deTaxCode.Mode = "ADD"
            End If
            Select Case deTaxCode.Mode
                Case Is = "ADD"
                    If Not taxCodeExists Then
                        cmd = New SqlCommand(strAddTaxCode, conSql2005)
                        cmd.Parameters.Add("@TAX_CODE", SqlDbType.NVarChar, 20).Value = deTaxCode.TaxCode
                        cmd.Parameters.Add("@TAX_CODE_DESCRIPTION", SqlDbType.NVarChar, 100).Value = deTaxCode.Description
                        cmd.Parameters.Add("@TAX_PERCENTAGE", SqlDbType.Decimal).Value = 0

                        cmd.ExecuteNonQuery()
                    End If
                Case Is = "UPDATE"
                    If taxCodeExists Then
                        cmd = New SqlCommand(strUpdateTaxCode, conSql2005)
                        cmd.Parameters.Add("@TAX_CODE", SqlDbType.NVarChar, 20).Value = deTaxCode.TaxCode
                        cmd.Parameters.Add("@TAX_CODE_DESCRIPTION", SqlDbType.NVarChar, 100).Value = deTaxCode.Description
                        cmd.ExecuteNonQuery()
                    End If
                Case Is = "DELETE"
                    If taxCodeExists Then
                        cmd = New SqlCommand(strDeleteTaxCode2, conSql2005)
                        cmd.Parameters.Add("@TAX_CODE", SqlDbType.NVarChar, 20).Value = deTaxCode.TaxCode
                        cmd.ExecuteNonQuery()
                    End If
            End Select

        Next
        '-----------------------
        ' PROCESS CURRENCY CODES
        '-----------------------
        '------------------------------------------
        ' If replace then delete existing Tax codes
        '------------------------------------------
        Const strDeleteCurrencyCodes As String = "DELETE FROM tbl_currency "
        Const strSelectCurrencyCode As String = "Select * FROM tbl_currency where " &
                                                " CURRENCY_CODE = @CURRENCY_CODE"
        Const strUpdateCurrencyCode As String = "UPDATE tbl_currency Set CURRENCY_CODE_DESCRIPTION=@CURRENCY_CODE_DESCRIPTION, " &
                                                "CURRENCY_SYMBOL = @CURRENCY_SYMBOL where " &
                                                " CURRENCY_CODE = @CURRENCY_CODE"
        Const strDeleteCurrencyCode2 As String = "DELETE FROM  tbl_currency where " &
                                            " CURRENCY_CODE= @CURRENCY_CODE "
        Const strAddCurrencyCode As String = "INSERT INTO tbl_currency" &
           "([CURRENCY_CODE]  " &
           ",[CURRENCY_CODE_DESCRIPTION]" &
           ",[CURRENCY_SYMBOL]" &
           ",[CURRENCY_CULTURE]" &
           ",[CURRENCY_CODE_1]" &
           ",[CURRENCY_CODE_2]" &
           ",[CURRENCY_CODE_3]" &
           ",[CURRENCY_CODE_4]" &
           ",[CURRENCY_CODE_5])" &
            " VALUES" &
          " (@CURRENCY_CODE" &
          " ,@CURRENCY_CODE_DESCRIPTION" &
          " ,@CURRENCY_SYMBOL" &
          " ,@CURRENCY_CULTURE" &
          " ,@CURRENCY_CODE_1" &
          " ,@CURRENCY_CODE_2" &
          " ,@CURRENCY_CODE_3" &
          " ,@CURRENCY_CODE_4" &
          " ,@CURRENCY_CODE_5)  "
        Dim currencyCodeExists As Boolean = False
        If DeProductPriceLoad.CurrencyCodeMode = "REPLACE" Then
            cmd = New SqlCommand(strDeleteCurrencyCodes, conSql2005)
            cmd.ExecuteNonQuery()
        End If
        For Each deCurrencyCode As DECurrencyCode In DeProductPriceLoad.ColCurrencyCodes
            ' Check it exists
            currencyCodeExists = False
            cmd = New SqlCommand(strSelectCurrencyCode, conSql2005)
            cmd.Parameters.Add("@CURRENCY_CODE", SqlDbType.NVarChar, 20).Value = deCurrencyCode.CurrencyCode
            dtrReader = cmd.ExecuteReader
            If dtrReader.HasRows Then
                currencyCodeExists = True
            End If

            dtrReader.Close()
            If deCurrencyCode.Mode = String.Empty Then
                deCurrencyCode.Mode = "ADD"
            End If
            Select Case deCurrencyCode.Mode
                Case Is = "ADD"
                    If Not currencyCodeExists Then
                        cmd = New SqlCommand(strAddCurrencyCode, conSql2005)
                        cmd.Parameters.Add("@CURRENCY_CODE", SqlDbType.NVarChar, 20).Value = deCurrencyCode.CurrencyCode
                        cmd.Parameters.Add("@CURRENCY_CODE_DESCRIPTION", SqlDbType.NVarChar, 100).Value = deCurrencyCode.Description
                        cmd.Parameters.Add("@CURRENCY_SYMBOL", SqlDbType.NVarChar, 20).Value = deCurrencyCode.HtmlCurrencySymbol
                        cmd.Parameters.Add("@CURRENCY_CULTURE", SqlDbType.NVarChar, 20).Value = String.Empty
                        cmd.Parameters.Add("@CURRENCY_CODE_1", SqlDbType.NVarChar, 20).Value = String.Empty
                        cmd.Parameters.Add("@CURRENCY_CODE_2", SqlDbType.NVarChar, 20).Value = String.Empty
                        cmd.Parameters.Add("@CURRENCY_CODE_3", SqlDbType.NVarChar, 20).Value = String.Empty
                        cmd.Parameters.Add("@CURRENCY_CODE_4", SqlDbType.NVarChar, 20).Value = String.Empty
                        cmd.Parameters.Add("@CURRENCY_CODE_5", SqlDbType.NVarChar, 20).Value = String.Empty

                        cmd.ExecuteNonQuery()
                    End If
                Case Is = "UPDATE"
                    If currencyCodeExists Then
                        cmd = New SqlCommand(strUpdateCurrencyCode, conSql2005)
                        cmd.Parameters.Add("@CURRENCY_CODE", SqlDbType.NVarChar, 20).Value = deCurrencyCode.CurrencyCode
                        cmd.Parameters.Add("@CURRENCY_CODE_DESCRIPTION", SqlDbType.NVarChar, 100).Value = deCurrencyCode.Description
                        cmd.Parameters.Add("@CURRENCY_SYMBOL", SqlDbType.NVarChar, 20).Value = deCurrencyCode.HtmlCurrencySymbol
                        cmd.ExecuteNonQuery()
                    End If
                Case Is = "DELETE"
                    If currencyCodeExists Then
                        cmd = New SqlCommand(strDeleteCurrencyCode2, conSql2005)
                        cmd.Parameters.Add("@CURRENCY_CODE", SqlDbType.NVarChar, 20).Value = deCurrencyCode.CurrencyCode
                        cmd.ExecuteNonQuery()
                    End If
            End Select

        Next
        '--------------------
        ' PROCESS PRICE LISTS
        '--------------------
        Const strDeleteAllPriceListDetailWork = "DELETE FROM TBL_PRICE_LIST_DETAIL_WORK "
        Const strDeleteAllPriceListHeaderWork = "DELETE FROM TBL_PRICE_LIST_HEADER_WORK "

        'Const strSelectPriceDetail = "Select * FROM TBL_PRICE_LIST_DETAIL WHERE " & _
        '                    " PRICE_LIST = @PRICE_LIST AND PRODUCT = @PRODUCT "
        Const strSelectPriceHeader = "Select * FROM TBL_PRICE_LIST_HEADER WHERE " &
                            " PRICE_LIST = @PRICE_LIST  "
        'Const strDeletePriceListDetail = "DELETE FROM TBL_PRICE_LIST_DETAIL WHERE " & _
        '                   " PRICE_LIST = @PRICE_LIST AND PRODUCT = @PRODUCT"
        Const strInsertPriceListHeader As String = "INSERT INTO TBL_PRICE_LIST_HEADER " &
                   "( PRICE_LIST" &
                   " ,PRICE_LIST_DESCRIPTION" &
                   " ,CURRENCY_CODE" &
                   " ,FREE_DELIVERY_VALUE" &
                   " ,MIN_DELIVERY_VALUE" &
                   " ,START_DATE" &
                   " ,END_DATE)" &
                   "    VALUES " &
                   "( @PRICE_LIST" &
                   " ,@PRICE_LIST_DESCRIPTION" &
                   " ,@CURRENCY_CODE" &
                   " ,@FREE_DELIVERY_VALUE" &
                   " ,@MIN_DELIVERY_VALUE" &
                   " ,@START_DATE" &
                   " ,@END_DATE)"
        Const strUpdatePriceListHeader As String = "UPDATE tbl_price_list_header" &
            " Set [PRICE_LIST_DESCRIPTION] = @PRICE_LIST_DESCRIPTION" &
            " ,[CURRENCY_CODE] = @CURRENCY_CODE " &
            " ,[FREE_DELIVERY_VALUE] = @FREE_DELIVERY_VALUE " &
            " ,[MIN_DELIVERY_VALUE] = @MIN_DELIVERY_VALUE " &
            " ,[START_DATE] = @START_DATE " &
            " ,[END_DATE] = @END_DATE" &
            " WHERE PRICE_LIST = @PRICE_LIST "

        Const strInsertPriceListDetail As String = "INSERT INTO TBL_PRICE_LIST_DETAIL " &
          " (PRICE_LIST,PRODUCT,FROM_DATE,TO_DATE,NET_PRICE,GROSS_PRICE,TAX_AMOUNT,SALE_NET_PRICE" &
           ",SALE_GROSS_PRICE,SALE_TAX_AMOUNT,DELIVERY_NET_PRICE,DELIVERY_GROSS_PRICE,DELIVERY_TAX_AMOUNT" &
           ",PRICE_1,PRICE_2,PRICE_3,PRICE_4,PRICE_5,TAX_CODE,TARIFF_CODE,PRICE_BREAK_CODE,PRICE_BREAK_QUANTITY_1" &
           ",SALE_PRICE_BREAK_QUANTITY_1,NET_PRICE_2,GROSS_PRICE_2,TAX_AMOUNT_2,PRICE_BREAK_QUANTITY_2,SALE_NET_PRICE_2" &
           ",SALE_GROSS_PRICE_2,SALE_TAX_AMOUNT_2,SALE_PRICE_BREAK_QUANTITY_2,NET_PRICE_3,GROSS_PRICE_3,TAX_AMOUNT_3,PRICE_BREAK_QUANTITY_3" &
           ",SALE_NET_PRICE_3,SALE_GROSS_PRICE_3,SALE_TAX_AMOUNT_3,SALE_PRICE_BREAK_QUANTITY_3,NET_PRICE_4" &
           ",GROSS_PRICE_4,TAX_AMOUNT_4,PRICE_BREAK_QUANTITY_4,SALE_NET_PRICE_4,SALE_GROSS_PRICE_4,SALE_TAX_AMOUNT_4" &
           ",SALE_PRICE_BREAK_QUANTITY_4,NET_PRICE_5,GROSS_PRICE_5,TAX_AMOUNT_5,PRICE_BREAK_QUANTITY_5" &
           ",SALE_NET_PRICE_5,SALE_GROSS_PRICE_5,SALE_TAX_AMOUNT_5,SALE_PRICE_BREAK_QUANTITY_5,NET_PRICE_6,GROSS_PRICE_6" &
           ",TAX_AMOUNT_6,PRICE_BREAK_QUANTITY_6,SALE_NET_PRICE_6,SALE_GROSS_PRICE_6,SALE_TAX_AMOUNT_6" &
           ",SALE_PRICE_BREAK_QUANTITY_6, NET_PRICE_7, GROSS_PRICE_7, TAX_AMOUNT_7, PRICE_BREAK_QUANTITY_7, SALE_NET_PRICE_7, SALE_GROSS_PRICE_7, SALE_TAX_AMOUNT_7, SALE_PRICE_BREAK_QUANTITY_7" &
           ",NET_PRICE_8,GROSS_PRICE_8,TAX_AMOUNT_8,PRICE_BREAK_QUANTITY_8,SALE_NET_PRICE_8,SALE_GROSS_PRICE_8,SALE_TAX_AMOUNT_8" &
           ",SALE_PRICE_BREAK_QUANTITY_8,NET_PRICE_9,GROSS_PRICE_9,TAX_AMOUNT_9,PRICE_BREAK_QUANTITY_9,SALE_NET_PRICE_9" &
           ",SALE_GROSS_PRICE_9,SALE_TAX_AMOUNT_9,SALE_PRICE_BREAK_QUANTITY_9,NET_PRICE_10,GROSS_PRICE_10" &
           ",TAX_AMOUNT_10,PRICE_BREAK_QUANTITY_10,SALE_NET_PRICE_10,SALE_GROSS_PRICE_10,SALE_TAX_AMOUNT_10,SALE_PRICE_BREAK_QUANTITY_10,DELIVERY_NET_PRICE_2" &
           ",DELIVERY_GROSS_PRICE_2,DELIVERY_TAX_AMOUNT_2,DELIVERY_SALE_NET_PRICE_2,DELIVERY_SALE_GROSS_PRICE_2,DELIVERY_SALE_TAX_AMOUNT_2" &
           ",DELIVERY_NET_PRICE_3,DELIVERY_GROSS_PRICE_3,DELIVERY_TAX_AMOUNT_3,DELIVERY_SALE_NET_PRICE_3,DELIVERY_SALE_GROSS_PRICE_3,DELIVERY_SALE_TAX_AMOUNT_3" &
           ",DELIVERY_NET_PRICE_4,DELIVERY_GROSS_PRICE_4,DELIVERY_TAX_AMOUNT_4,DELIVERY_SALE_NET_PRICE_4,DELIVERY_SALE_GROSS_PRICE_4" &
           ",DELIVERY_SALE_TAX_AMOUNT_4,DELIVERY_NET_PRICE_5,DELIVERY_GROSS_PRICE_5,DELIVERY_TAX_AMOUNT_5,DELIVERY_SALE_NET_PRICE_5" &
           ",DELIVERY_SALE_GROSS_PRICE_5,DELIVERY_SALE_TAX_AMOUNT_5,DELIVERY_NET_PRICE_6,DELIVERY_GROSS_PRICE_6,DELIVERY_TAX_AMOUNT_6" &
           ",DELIVERY_SALE_NET_PRICE_6,DELIVERY_SALE_GROSS_PRICE_6,DELIVERY_SALE_TAX_AMOUNT_6,DELIVERY_NET_PRICE_7,DELIVERY_GROSS_PRICE_7,DELIVERY_TAX_AMOUNT_7,DELIVERY_SALE_NET_PRICE_7,DELIVERY_SALE_GROSS_PRICE_7,DELIVERY_SALE_TAX_AMOUNT_7,DELIVERY_NET_PRICE_8" &
           ",DELIVERY_GROSS_PRICE_8,DELIVERY_TAX_AMOUNT_8,DELIVERY_SALE_NET_PRICE_8,DELIVERY_SALE_GROSS_PRICE_8,DELIVERY_SALE_TAX_AMOUNT_8" &
           ",DELIVERY_NET_PRICE_9,DELIVERY_GROSS_PRICE_9,DELIVERY_TAX_AMOUNT_9,DELIVERY_SALE_NET_PRICE_9,DELIVERY_SALE_GROSS_PRICE_9" &
           ",DELIVERY_SALE_TAX_AMOUNT_9,DELIVERY_NET_PRICE_10,DELIVERY_GROSS_PRICE_10" &
           ",DELIVERY_TAX_AMOUNT_10,DELIVERY_SALE_NET_PRICE_10,DELIVERY_SALE_GROSS_PRICE_10,DELIVERY_SALE_TAX_AMOUNT_10) " &
        " VALUES " &
           "(@PRICE_LIST,@PRODUCT,@FROM_DATE,@TO_DATE,@NET_PRICE,@GROSS_PRICE,@TAX_AMOUNT,@SALE_NET_PRICE,@SALE_GROSS_PRICE,@SALE_TAX_AMOUNT,@DELIVERY_NET_PRICE,@DELIVERY_GROSS_PRICE,@DELIVERY_TAX_AMOUNT,@PRICE_1,@PRICE_2,@PRICE_3,@PRICE_4,@PRICE_5,@TAX_CODE,@TARIFF_CODE" &
           ",@PRICE_BREAK_CODE,@PRICE_BREAK_QUANTITY_1,@SALE_PRICE_BREAK_QUANTITY_1,@NET_PRICE_2,@GROSS_PRICE_2,@TAX_AMOUNT_2,@PRICE_BREAK_QUANTITY_2,@SALE_NET_PRICE_2,@SALE_GROSS_PRICE_2,@SALE_TAX_AMOUNT_2,@SALE_PRICE_BREAK_QUANTITY_2,@NET_PRICE_3,@GROSS_PRICE_3,@TAX_AMOUNT_3" &
           ",@PRICE_BREAK_QUANTITY_3,@SALE_NET_PRICE_3,@SALE_GROSS_PRICE_3,@SALE_TAX_AMOUNT_3,@SALE_PRICE_BREAK_QUANTITY_3,@NET_PRICE_4,@GROSS_PRICE_4,@TAX_AMOUNT_4,@PRICE_BREAK_QUANTITY_4,@SALE_NET_PRICE_4,@SALE_GROSS_PRICE_4,@SALE_TAX_AMOUNT_4,@SALE_PRICE_BREAK_QUANTITY_4" &
          ",@NET_PRICE_5,@GROSS_PRICE_5,@TAX_AMOUNT_5,@PRICE_BREAK_QUANTITY_5,@SALE_NET_PRICE_5,@SALE_GROSS_PRICE_5,@SALE_TAX_AMOUNT_5,@SALE_PRICE_BREAK_QUANTITY_5,@NET_PRICE_6,@GROSS_PRICE_6,@TAX_AMOUNT_6,@PRICE_BREAK_QUANTITY_6,@SALE_NET_PRICE_6,@SALE_GROSS_PRICE_6" &
           ",@SALE_TAX_AMOUNT_6,@SALE_PRICE_BREAK_QUANTITY_6,@NET_PRICE_7,@GROSS_PRICE_7,@TAX_AMOUNT_7,@PRICE_BREAK_QUANTITY_7,@SALE_NET_PRICE_7,@SALE_GROSS_PRICE_7,@SALE_TAX_AMOUNT_7,@SALE_PRICE_BREAK_QUANTITY_7,@NET_PRICE_8,@GROSS_PRICE_8,@TAX_AMOUNT_8,@PRICE_BREAK_QUANTITY_8" &
           ",@SALE_NET_PRICE_8,@SALE_GROSS_PRICE_8,@SALE_TAX_AMOUNT_8,@SALE_PRICE_BREAK_QUANTITY_8,@NET_PRICE_9,@GROSS_PRICE_9,@TAX_AMOUNT_9,@PRICE_BREAK_QUANTITY_9,@SALE_NET_PRICE_9,@SALE_GROSS_PRICE_9,@SALE_TAX_AMOUNT_9,@SALE_PRICE_BREAK_QUANTITY_9,@NET_PRICE_10,@GROSS_PRICE_10" &
           ",@TAX_AMOUNT_10,@PRICE_BREAK_QUANTITY_10,@SALE_NET_PRICE_10,@SALE_GROSS_PRICE_10,@SALE_TAX_AMOUNT_10,@SALE_PRICE_BREAK_QUANTITY_10,@DELIVERY_NET_PRICE_2,@DELIVERY_GROSS_PRICE_2,@DELIVERY_TAX_AMOUNT_2,@DELIVERY_SALE_NET_PRICE_2,@DELIVERY_SALE_GROSS_PRICE_2" &
           ",@DELIVERY_SALE_TAX_AMOUNT_2,@DELIVERY_NET_PRICE_3,@DELIVERY_GROSS_PRICE_3,@DELIVERY_TAX_AMOUNT_3,@DELIVERY_SALE_NET_PRICE_3,@DELIVERY_SALE_GROSS_PRICE_3,@DELIVERY_SALE_TAX_AMOUNT_3,@DELIVERY_NET_PRICE_4,@DELIVERY_GROSS_PRICE_4,@DELIVERY_TAX_AMOUNT_4" &
           ",@DELIVERY_SALE_NET_PRICE_4,@DELIVERY_SALE_GROSS_PRICE_4,@DELIVERY_SALE_TAX_AMOUNT_4,@DELIVERY_NET_PRICE_5,@DELIVERY_GROSS_PRICE_5,@DELIVERY_TAX_AMOUNT_5,@DELIVERY_SALE_NET_PRICE_5,@DELIVERY_SALE_GROSS_PRICE_5,@DELIVERY_SALE_TAX_AMOUNT_5,@DELIVERY_NET_PRICE_6" &
           ",@DELIVERY_GROSS_PRICE_6,@DELIVERY_TAX_AMOUNT_6,@DELIVERY_SALE_NET_PRICE_6,@DELIVERY_SALE_GROSS_PRICE_6,@DELIVERY_SALE_TAX_AMOUNT_6,@DELIVERY_NET_PRICE_7,@DELIVERY_GROSS_PRICE_7,@DELIVERY_TAX_AMOUNT_7,@DELIVERY_SALE_NET_PRICE_7,@DELIVERY_SALE_GROSS_PRICE_7" &
           ",@DELIVERY_SALE_TAX_AMOUNT_7,@DELIVERY_NET_PRICE_8,@DELIVERY_GROSS_PRICE_8,@DELIVERY_TAX_AMOUNT_8,@DELIVERY_SALE_NET_PRICE_8,@DELIVERY_SALE_GROSS_PRICE_8,@DELIVERY_SALE_TAX_AMOUNT_8,@DELIVERY_NET_PRICE_9,@DELIVERY_GROSS_PRICE_9,@DELIVERY_TAX_AMOUNT_9" &
           ",@DELIVERY_SALE_NET_PRICE_9,@DELIVERY_SALE_GROSS_PRICE_9,@DELIVERY_SALE_TAX_AMOUNT_9,@DELIVERY_NET_PRICE_10,@DELIVERY_GROSS_PRICE_10,@DELIVERY_TAX_AMOUNT_10,@DELIVERY_SALE_NET_PRICE_10,@DELIVERY_SALE_GROSS_PRICE_10,@DELIVERY_SALE_TAX_AMOUNT_10)"
        Const strDeletePriceListheader As String = "DELETE FROM TBL_PRICE_LIST_HEADER WHERE PRICE_LIST = @PRICE_LIST"
        Const strDeletePriceListDetail_byPricelist As String = "DELETE FROM TBL_PRICE_LIST_DETAIL WHERE PRICE_LIST = @PRICE_LIST"
        Try
            If DeProductPriceLoad.PriceListMode = "REPLACE" Then
                '-------------------
                ' Delete work tables
                '-------------------
                cmd = New SqlCommand(strDeleteAllPriceListHeaderWork, conSql2005)
                cmd.ExecuteNonQuery()
                cmd = New SqlCommand(strDeleteAllPriceListDetailWork, conSql2005)
                cmd.ExecuteNonQuery()
            End If

            For Each dePriceList As DEPriceList In DeProductPriceLoad.ColPriceLists
                If DeProductPriceLoad.PriceListMode = "REPLACE" Then
                    '---------------------------
                    ' Write Header to Work table
                    '---------------------------
                    Dim strSqlHeaderInsert = strInsertPriceListHeader.Replace("TBL_PRICE_LIST_HEADER", "TBL_PRICE_LIST_HEADER_WORK")

                    err = AccessDatabaseSQL2005_ProductPriceLoad_WritePriceListHeader(cmd, strSqlHeaderInsert, dePriceList)
                    '----------------------------
                    ' Write details to Work table
                    '----------------------------
                    If Not err.HasError Then
                        Dim strSqlDetailInsert As String = strInsertPriceListDetail.Replace("TBL_PRICE_LIST_DETAIL", "TBL_PRICE_LIST_DETAIL_WORK")
                        err = AccessDatabaseSQL2005_ProductPriceLoad_ProcessPriceListDetail(cmd, strSqlDetailInsert, dePriceList, "TBL_PRICE_LIST_DETAIL_WORK")

                    End If

                Else
                    If dePriceList.Mode = "REPLACE" Then
                        '--------------------------------------------------------------
                        ' We are replacing the current price list - Update header file 
                        ' then write to WORK detail file. Finally call DT in 
                        ' a different mode
                        '--------------------------------------------------------------
                        '-------------------
                        ' Update header file
                        '-------------------
                        err = AccessDatabaseSQL2005_ProductPriceLoad_WritePriceListHeader(cmd, strUpdatePriceListHeader, dePriceList)

                        '---------------------
                        ' Write to detail file
                        '---------------------
                        Dim strSqlDetailInsert As String = strInsertPriceListDetail.Replace("TBL_PRICE_LIST_DETAIL", "TBL_PRICE_LIST_DETAIL_WORK")
                        err = AccessDatabaseSQL2005_ProductPriceLoad_ProcessPriceListDetail(cmd, strSqlDetailInsert, dePriceList, "TBL_PRICE_LIST_DETAIL_WORK")

                        '-------------------------------------
                        ' Data trasfer for this pricelist only
                        '-------------------------------------
                        DataTransfer.SQLConnectionString = Settings.FrontEndConnectionString
                        DataTransfer.DoUpdateTable_EBPLDT(True, dePriceList.Code)
                    Else
                        '--------------------------------------------
                        ' Updating individual records direct to db...
                        '--------------------------------------------
                        Select Case dePriceList.Mode.ToUpper
                            Case Is = "ADD"
                                '-----------------------
                                ' Check it doesn't exist
                                '-----------------------
                                Dim priceListExists As Boolean = False
                                cmd = New SqlCommand(strSelectPriceHeader, conSql2005)
                                cmd.Parameters.Add("@PRICE_LIST", SqlDbType.NVarChar, 20).Value = dePriceList.Code
                                dtrReader = cmd.ExecuteReader
                                If dtrReader.HasRows Then
                                    priceListExists = True
                                End If

                                dtrReader.Close()

                                If Not priceListExists Then
                                    '---------------------------
                                    ' Write Header to Main table
                                    '---------------------------
                                    Dim strSqlHeaderInsert = strInsertPriceListHeader
                                    err = AccessDatabaseSQL2005_ProductPriceLoad_WritePriceListHeader(cmd, strSqlHeaderInsert, dePriceList)
                                End If
                                '----------------
                                ' Process Details
                                '----------------
                                If Not err.HasError Then
                                    Dim strSqlDetailInsert As String = strInsertPriceListDetail
                                    AccessDatabaseSQL2005_ProductPriceLoad_ProcessPriceListDetail(cmd, strSqlDetailInsert, dePriceList, "TBL_PRICE_LIST_DETAIL")
                                End If

                                '   End If
                            Case Is = "UPDATE"
                                '----------------
                                ' Check it exists
                                '----------------
                                Dim priceListExists As Boolean = False
                                cmd = New SqlCommand(strSelectPriceHeader, conSql2005)
                                cmd.Parameters.Add("@PRICE_LIST", SqlDbType.NVarChar, 20).Value = dePriceList.Code
                                dtrReader = cmd.ExecuteReader
                                If dtrReader.HasRows Then
                                    priceListExists = True
                                End If

                                dtrReader.Close()

                                If priceListExists Then
                                    '--------------
                                    ' Update Header
                                    '--------------
                                    err = AccessDatabaseSQL2005_ProductPriceLoad_WritePriceListHeader(cmd, strUpdatePriceListHeader, dePriceList)
                                    '----------------
                                    ' Process Details
                                    '----------------
                                    Dim strSqlDetailInsert As String = strInsertPriceListDetail
                                    AccessDatabaseSQL2005_ProductPriceLoad_ProcessPriceListDetail(cmd, strSqlDetailInsert, dePriceList, "TBL_PRICE_LIST_DETAIL")

                                End If
                            Case Is = "DELETE"
                                '----------------
                                ' Check it exists
                                '----------------
                                Dim priceListExists As Boolean = False
                                cmd = New SqlCommand(strSelectPriceHeader, conSql2005)
                                cmd.Parameters.Add("@PRICE_LIST", SqlDbType.NVarChar, 20).Value = dePriceList.Code
                                dtrReader = cmd.ExecuteReader
                                If dtrReader.HasRows Then
                                    priceListExists = True
                                End If

                                dtrReader.Close()

                                If priceListExists Then
                                    '---------------------
                                    ' Delete header record
                                    '---------------------
                                    cmd = New SqlCommand(strDeletePriceListheader, conSql2005)
                                    cmd.Parameters.Add("@PRICE_LIST", SqlDbType.NVarChar, 20).Value = dePriceList.Code
                                    cmd.ExecuteNonQuery()
                                    '----------------------
                                    ' Delete detail records
                                    '----------------------
                                    cmd = New SqlCommand(strDeletePriceListDetail_byPricelist, conSql2005)
                                    cmd.Parameters.Add("@PRICE_LIST", SqlDbType.NVarChar, 20).Value = dePriceList.Code
                                    cmd.ExecuteNonQuery()
                                End If

                            Case Else
                                '-----------------------------------
                                ' Blank mode - just process products
                                '-----------------------------------
                                Dim strSqlDetailInsert As String = strInsertPriceListDetail
                                AccessDatabaseSQL2005_ProductPriceLoad_ProcessPriceListDetail(cmd, strSqlDetailInsert, dePriceList, "TBL_PRICE_LIST_DETAIL")

                        End Select

                    End If

                End If
            Next
            '--------------------------------------------------------
            ' If it's replace then update main files via datatransfer
            '--------------------------------------------------------
            If Not err.HasError AndAlso DeProductPriceLoad.PriceListMode = "REPLACE" Then
                DataTransfer.SQLConnectionString = Settings.FrontEndConnectionString
                DataTransfer.DoUpdateTable_EBPLHD()
                DataTransfer.DoUpdateTable_EBPLDT(True)
            End If

            '-----------------
            ' PROCESS DEFAULTS
            '-----------------
            '------------------------------------------------------
            ' If replace then delete existing PARTNER specific ones
            ' - i've deceided to do this direct instead of through 
            '   datatransfer as it's not really suitable
            '------------------------------------------------------
            Const strDeleteDefaults As String = "DELETE FROM tbl_ecommerce_module_defaults_bu where " &
                                                    "DEFAULT_NAME In ('SHOW_PRICES_EXCLUDING_VAT', 'PRICE_LIST') AND " &
                                                    " PARTNER <> '*ALL'"
            Const strSelectDefaults As String = "SELECT * FROM tbl_ecommerce_module_defaults_bu where " &
                                                    " BUSINESS_UNIT = @BUSINESS_UNIT AND " &
                                                    " PARTNER = @PARTNER AND " &
                                                    " DEFAULT_NAME = @DEFAULT_NAME "
            Const strUpdateDefaults As String = "UPDATE tbl_ecommerce_module_defaults_bu set VALUE=@VALUE where " &
                                                    " BUSINESS_UNIT = @BUSINESS_UNIT AND " &
                                                    " PARTNER = @PARTNER AND " &
                                                    " DEFAULT_NAME = @DEFAULT_NAME "
            Const strDeleteDefaults2 As String = "DELETE FROM  tbl_ecommerce_module_defaults_bu where " &
                                                " BUSINESS_UNIT = @BUSINESS_UNIT AND " &
                                                " PARTNER = @PARTNER AND " &
                                                " DEFAULT_NAME = @DEFAULT_NAME"
            Const strAddDefaults As String = "INSERT INTO tbl_ecommerce_module_defaults_bu" &
           " ([BUSINESS_UNIT],[PARTNER] ,[APPLICATION] ,[MODULE] ,[DEFAULT_NAME]  ,[VALUE]) " &
           " VALUES  (@BUSINESS_UNIT ,@PARTNER,@APPLICATION ,@MODULE,@DEFAULT_NAME ,@VALUE)"
            Dim defaultExists As Boolean = False
            If DeProductPriceLoad.DefaultsMode = "REPLACE" Then
                cmd = New SqlCommand(strDeleteDefaults, conSql2005)
                cmd.ExecuteNonQuery()
            End If
            For Each dePriceDefault As DEPriceDefault In DeProductPriceLoad.ColDefaults
                ' Check it exists
                defaultExists = False
                cmd = New SqlCommand(strSelectDefaults, conSql2005)
                cmd.Parameters.Add("@BUSINESS_UNIT", SqlDbType.NVarChar, 50).Value = dePriceDefault.BusinessUnit
                cmd.Parameters.Add("@PARTNER", SqlDbType.NVarChar, 50).Value = dePriceDefault.Partner
                cmd.Parameters.Add("@DEFAULT_NAME", SqlDbType.NVarChar, 50).Value = dePriceDefault.DefaultName
                dtrReader = cmd.ExecuteReader
                If dtrReader.HasRows Then
                    defaultExists = True
                End If

                dtrReader.Close()
                If dePriceDefault.Mode = String.Empty Then
                    dePriceDefault.Mode = "ADD"
                End If
                Select Case dePriceDefault.Mode
                    Case Is = "ADD"
                        If Not defaultExists Then
                            cmd = New SqlCommand(strAddDefaults, conSql2005)
                            cmd.Parameters.Add("@BUSINESS_UNIT", SqlDbType.NVarChar, 50).Value = dePriceDefault.BusinessUnit
                            cmd.Parameters.Add("@PARTNER", SqlDbType.NVarChar, 50).Value = dePriceDefault.Partner
                            cmd.Parameters.Add("@APPLICATION", SqlDbType.NVarChar, 50).Value = String.Empty
                            cmd.Parameters.Add("@MODULE", SqlDbType.NVarChar, 50).Value = String.Empty
                            cmd.Parameters.Add("@DEFAULT_NAME", SqlDbType.NVarChar, 50).Value = dePriceDefault.DefaultName
                            cmd.Parameters.Add("@VALUE", SqlDbType.NVarChar).Value = dePriceDefault.Value

                            cmd.ExecuteNonQuery()
                        End If
                    Case Is = "UPDATE"
                        If defaultExists Then
                            cmd = New SqlCommand(strUpdateDefaults, conSql2005)
                            cmd.Parameters.Add("@VALUE", SqlDbType.NVarChar).Value = dePriceDefault.Value
                            cmd.Parameters.Add("@BUSINESS_UNIT", SqlDbType.NVarChar, 50).Value = dePriceDefault.BusinessUnit
                            cmd.Parameters.Add("@PARTNER", SqlDbType.NVarChar, 50).Value = dePriceDefault.Partner
                            cmd.Parameters.Add("@DEFAULT_NAME", SqlDbType.NVarChar, 50).Value = dePriceDefault.DefaultName
                            cmd.ExecuteNonQuery()
                        End If
                    Case Is = "DELETE"
                        If defaultExists Then
                            cmd = New SqlCommand(strDeleteDefaults2, conSql2005)
                            cmd.Parameters.Add("@VALUE", SqlDbType.NVarChar).Value = dePriceDefault.Value
                            cmd.Parameters.Add("@BUSINESS_UNIT", SqlDbType.NVarChar, 50).Value = dePriceDefault.BusinessUnit
                            cmd.Parameters.Add("@PARTNER", SqlDbType.NVarChar, 50).Value = dePriceDefault.Partner
                            cmd.Parameters.Add("@DEFAULT_NAME", SqlDbType.NVarChar, 50).Value = dePriceDefault.DefaultName
                            cmd.ExecuteNonQuery()
                        End If
                End Select

            Next

        Catch ex As Exception
            ResultDataSet = Nothing
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBPD-SQL2005-3"
                .HasError = True
            End With
        End Try

        Return err
    End Function
    Private Function AccessDatabaseSQL2005_ProductPriceLoad_WritePriceListHeader(ByVal cmd As SqlCommand,
                                                                                 ByVal strSQL As String,
                                                                                 ByVal priceList As DEPriceList) As ErrorObj
        Dim err As New ErrorObj
        Try
            cmd = New SqlCommand(strSQL, conSql2005)
            cmd.Parameters.Add("@PRICE_LIST", SqlDbType.NVarChar, 20).Value = priceList.Code
            cmd.Parameters.Add("@PRICE_LIST_DESCRIPTION", SqlDbType.NVarChar, 100).Value = priceList.Description
            cmd.Parameters.Add("@CURRENCY_CODE", SqlDbType.NVarChar, 20).Value = priceList.CurrencyCode
            cmd.Parameters.Add("@FREE_DELIVERY_VALUE", SqlDbType.Decimal).Value = priceList.FreeDeliveryValue
            cmd.Parameters.Add("@MIN_DELIVERY_VALUE", SqlDbType.Decimal).Value = priceList.MinimumDeliveryValue
            cmd.Parameters.Add("@START_DATE", SqlDbType.DateTime).Value = priceList.StartDate
            cmd.Parameters.Add("@END_DATE", SqlDbType.DateTime).Value = priceList.EndDate

            cmd.ExecuteNonQuery()
        Catch ex As Exception
            ResultDataSet = Nothing
            Const strError As String = "Error during database access (WritePriceListHeader)"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBPD-SQL2005-3"
                .HasError = True
            End With
        End Try

        Return err
    End Function


    Private Function AccessDatabaseSQL2005_ProductPriceLoad_ProcessPriceListDetail(ByVal cmd As SqlCommand,
                                                                             ByVal strSql As String,
                                                                             ByVal priceList As DEPriceList,
                                                                             ByVal file As String) As ErrorObj
        Dim strDeleteSQL As String = "DELETE FROM tbl_price_list_detail WHERE " &
                                    " PRICE_LIST = @PRICE_LIST AND PRODUCT = @PRODUCT"
        Dim strDeleteSQL2 As String = "DELETE FROM tbl_price_list_detail WHERE " &
                                    " PRICE_LIST = @PRICE_LIST "
        Dim strUpdateSQL As String = "UPDATE tbl_price_list_detail  " &
  "SET   " &
   "   [FROM_DATE] = @FROM_DATE " &
   "   ,[TO_DATE] = @TO_DATE " &
   "   ,[NET_PRICE] = @NET_PRICE " &
   "   ,[GROSS_PRICE] = @GROSS_PRICE" &
   "  ,[TAX_AMOUNT] = @TAX_AMOUNT" &
   "  ,[SALE_NET_PRICE] = @SALE_NET_PRICE" &
   "   ,[SALE_GROSS_PRICE] = @SALE_GROSS_PRICE" &
   "   ,[SALE_TAX_AMOUNT] = @SALE_TAX_AMOUNT" &
   "   ,[DELIVERY_NET_PRICE] = @DELIVERY_NET_PRICE" &
   "   ,[DELIVERY_GROSS_PRICE] = @DELIVERY_GROSS_PRICE" &
   "   ,[DELIVERY_TAX_AMOUNT] = @DELIVERY_TAX_AMOUNT" &
   "   ,[TAX_CODE] = @TAX_CODE" &
   "   ,[TARIFF_CODE] = @TARIFF_CODE" &
   "   ,[PRICE_BREAK_CODE] = @PRICE_BREAK_CODE" &
   "   ,[PRICE_BREAK_QUANTITY_1] = @PRICE_BREAK_QUANTITY_1" &
   "   ,[SALE_PRICE_BREAK_QUANTITY_1] = @SALE_PRICE_BREAK_QUANTITY_1" &
   "   ,[NET_PRICE_2] = @NET_PRICE_2" &
   "   ,[GROSS_PRICE_2] = @GROSS_PRICE_2" &
   "   ,[TAX_AMOUNT_2] = @TAX_AMOUNT_2" &
   "   ,[PRICE_BREAK_QUANTITY_2] = @PRICE_BREAK_QUANTITY_2" &
   "   ,[SALE_NET_PRICE_2] = @SALE_NET_PRICE_2" &
   "   ,[SALE_GROSS_PRICE_2] = @SALE_GROSS_PRICE_2" &
   "   ,[SALE_TAX_AMOUNT_2] = @SALE_TAX_AMOUNT_2" &
   "   ,[SALE_PRICE_BREAK_QUANTITY_2] = @SALE_PRICE_BREAK_QUANTITY_2" &
   "   ,[NET_PRICE_3] = @NET_PRICE_3" &
   "   ,[GROSS_PRICE_3] = @GROSS_PRICE_3" &
   "   ,[TAX_AMOUNT_3] = @TAX_AMOUNT_3" &
   "   ,[PRICE_BREAK_QUANTITY_3] = @PRICE_BREAK_QUANTITY_3" &
   "   ,[SALE_NET_PRICE_3] = @SALE_NET_PRICE_3" &
   "   ,[SALE_GROSS_PRICE_3] = @SALE_GROSS_PRICE_3" &
   "   ,[SALE_TAX_AMOUNT_3] = @SALE_TAX_AMOUNT_3" &
   "   ,[SALE_PRICE_BREAK_QUANTITY_3] = @SALE_PRICE_BREAK_QUANTITY_3" &
   "   ,[NET_PRICE_4] = @NET_PRICE_4" &
   "   ,[GROSS_PRICE_4] = @GROSS_PRICE_4" &
   "   ,[TAX_AMOUNT_4] = @TAX_AMOUNT_4" &
   "   ,[PRICE_BREAK_QUANTITY_4] = @PRICE_BREAK_QUANTITY_4" &
   "   ,[SALE_NET_PRICE_4] = @SALE_NET_PRICE_4" &
   "   ,[SALE_GROSS_PRICE_4] = @SALE_GROSS_PRICE_4" &
   "   ,[SALE_TAX_AMOUNT_4] = @SALE_TAX_AMOUNT_4" &
   "   ,[SALE_PRICE_BREAK_QUANTITY_4] = @SALE_PRICE_BREAK_QUANTITY_4" &
   "   ,[NET_PRICE_5] = @NET_PRICE_5" &
   "   ,[GROSS_PRICE_5] = @GROSS_PRICE_5" &
   "   ,[TAX_AMOUNT_5] = @TAX_AMOUNT_5" &
   "   ,[PRICE_BREAK_QUANTITY_5] = @PRICE_BREAK_QUANTITY_5" &
   "   ,[SALE_NET_PRICE_5] = @SALE_NET_PRICE_5" &
   "   ,[SALE_GROSS_PRICE_5] = @SALE_GROSS_PRICE_5" &
   "   ,[SALE_TAX_AMOUNT_5] = @SALE_TAX_AMOUNT_5" &
   "   ,[SALE_PRICE_BREAK_QUANTITY_5] = @SALE_PRICE_BREAK_QUANTITY_5" &
   "   ,[NET_PRICE_6] = @NET_PRICE_6" &
   "   ,[GROSS_PRICE_6] = @GROSS_PRICE_6" &
   "   ,[TAX_AMOUNT_6] = @TAX_AMOUNT_6" &
   "   ,[PRICE_BREAK_QUANTITY_6] = @PRICE_BREAK_QUANTITY_6" &
   "   ,[SALE_NET_PRICE_6] = @SALE_NET_PRICE_6" &
   "   ,[SALE_GROSS_PRICE_6] = @SALE_GROSS_PRICE_6" &
   "  ,[SALE_TAX_AMOUNT_6] = @SALE_TAX_AMOUNT_6" &
   "   ,[SALE_PRICE_BREAK_QUANTITY_6] = @SALE_PRICE_BREAK_QUANTITY_6" &
   "   ,[NET_PRICE_7] = @NET_PRICE_7" &
   "   ,[GROSS_PRICE_7] = @GROSS_PRICE_7" &
   "   ,[TAX_AMOUNT_7] = @TAX_AMOUNT_7" &
   "   ,[PRICE_BREAK_QUANTITY_7] = @PRICE_BREAK_QUANTITY_7" &
   "   ,[SALE_NET_PRICE_7] = @SALE_NET_PRICE_7" &
   "   ,[SALE_GROSS_PRICE_7] = @SALE_GROSS_PRICE_7" &
   "   ,[SALE_TAX_AMOUNT_7] = @SALE_TAX_AMOUNT_7" &
   "   ,[SALE_PRICE_BREAK_QUANTITY_7] = @SALE_PRICE_BREAK_QUANTITY_7" &
   "   ,[NET_PRICE_8] = @NET_PRICE_8" &
   "   ,[GROSS_PRICE_8] = @GROSS_PRICE_8" &
   "   ,[TAX_AMOUNT_8] = @TAX_AMOUNT_8" &
   "   ,[PRICE_BREAK_QUANTITY_8] = @PRICE_BREAK_QUANTITY_8" &
   "   ,[SALE_NET_PRICE_8] = @SALE_NET_PRICE_8" &
   "   ,[SALE_GROSS_PRICE_8] = @SALE_GROSS_PRICE_8" &
   "   ,[SALE_TAX_AMOUNT_8] = @SALE_TAX_AMOUNT_8" &
   "   ,[SALE_PRICE_BREAK_QUANTITY_8] = @SALE_PRICE_BREAK_QUANTITY_8" &
   "   ,[NET_PRICE_9] = @NET_PRICE_9" &
   "   ,[GROSS_PRICE_9] = @GROSS_PRICE_9" &
   "   ,[TAX_AMOUNT_9] = @TAX_AMOUNT_9" &
   "   ,[PRICE_BREAK_QUANTITY_9] = @PRICE_BREAK_QUANTITY_9" &
   "   ,[SALE_NET_PRICE_9] = @SALE_NET_PRICE_9" &
   "   ,[SALE_GROSS_PRICE_9] = @SALE_GROSS_PRICE_9" &
   "   ,[SALE_TAX_AMOUNT_9] = @SALE_TAX_AMOUNT_9" &
   "   ,[SALE_PRICE_BREAK_QUANTITY_9] = @SALE_PRICE_BREAK_QUANTITY_9" &
   "   ,[NET_PRICE_10] = @NET_PRICE_10" &
   "   ,[GROSS_PRICE_10] = @GROSS_PRICE_10" &
   "   ,[TAX_AMOUNT_10] = @TAX_AMOUNT_10" &
   "   ,[PRICE_BREAK_QUANTITY_10] = @PRICE_BREAK_QUANTITY_10" &
   "   ,[SALE_NET_PRICE_10] = @SALE_NET_PRICE_10" &
   "   ,[SALE_GROSS_PRICE_10] = @SALE_GROSS_PRICE_10" &
   "   ,[SALE_TAX_AMOUNT_10] = @SALE_TAX_AMOUNT_10" &
   "   ,[SALE_PRICE_BREAK_QUANTITY_10] = @SALE_PRICE_BREAK_QUANTITY_10" &
   "   ,[DELIVERY_NET_PRICE_2] = @DELIVERY_NET_PRICE_2" &
   "   ,[DELIVERY_GROSS_PRICE_2] = @DELIVERY_GROSS_PRICE_2" &
   "   ,[DELIVERY_TAX_AMOUNT_2] = @DELIVERY_TAX_AMOUNT_2" &
   "   ,[DELIVERY_SALE_NET_PRICE_2] = @DELIVERY_SALE_NET_PRICE_2" &
   "   ,[DELIVERY_SALE_GROSS_PRICE_2] = @DELIVERY_SALE_GROSS_PRICE_2" &
   "   ,[DELIVERY_SALE_TAX_AMOUNT_2] = @DELIVERY_SALE_TAX_AMOUNT_2" &
   "   ,[DELIVERY_NET_PRICE_3] = @DELIVERY_NET_PRICE_3" &
   "   ,[DELIVERY_GROSS_PRICE_3] = @DELIVERY_GROSS_PRICE_3" &
   "   ,[DELIVERY_TAX_AMOUNT_3] = @DELIVERY_TAX_AMOUNT_3" &
   "   ,[DELIVERY_SALE_NET_PRICE_3] = @DELIVERY_SALE_NET_PRICE_3" &
   "   ,[DELIVERY_SALE_GROSS_PRICE_3] = @DELIVERY_SALE_GROSS_PRICE_3" &
   "   ,[DELIVERY_SALE_TAX_AMOUNT_3] = @DELIVERY_SALE_TAX_AMOUNT_3" &
   "   ,[DELIVERY_NET_PRICE_4] = @DELIVERY_NET_PRICE_4" &
   "   ,[DELIVERY_GROSS_PRICE_4] = @DELIVERY_GROSS_PRICE_4" &
   "   ,[DELIVERY_TAX_AMOUNT_4] = @DELIVERY_TAX_AMOUNT_4" &
   "   ,[DELIVERY_SALE_NET_PRICE_4] = @DELIVERY_SALE_NET_PRICE_4" &
   "   ,[DELIVERY_SALE_GROSS_PRICE_4] = @DELIVERY_SALE_GROSS_PRICE_4" &
   "   ,[DELIVERY_SALE_TAX_AMOUNT_4] = @DELIVERY_SALE_TAX_AMOUNT_4" &
   "   ,[DELIVERY_NET_PRICE_5] = @DELIVERY_NET_PRICE_5" &
   "   ,[DELIVERY_GROSS_PRICE_5] = @DELIVERY_GROSS_PRICE_5" &
   "   ,[DELIVERY_TAX_AMOUNT_5] = @DELIVERY_TAX_AMOUNT_5" &
   "   ,[DELIVERY_SALE_NET_PRICE_5] = @DELIVERY_SALE_NET_PRICE_5" &
   "   ,[DELIVERY_SALE_GROSS_PRICE_5] = @DELIVERY_SALE_GROSS_PRICE_5" &
   "   ,[DELIVERY_SALE_TAX_AMOUNT_5] = @DELIVERY_SALE_TAX_AMOUNT_5" &
   "   ,[DELIVERY_NET_PRICE_6] = @DELIVERY_NET_PRICE_6" &
   "   ,[DELIVERY_GROSS_PRICE_6] = @DELIVERY_GROSS_PRICE_6" &
   "   ,[DELIVERY_TAX_AMOUNT_6] = @DELIVERY_TAX_AMOUNT_6" &
   "   ,[DELIVERY_SALE_NET_PRICE_6] = @DELIVERY_SALE_NET_PRICE_6" &
   "   ,[DELIVERY_SALE_GROSS_PRICE_6] = @DELIVERY_SALE_GROSS_PRICE_6" &
   "   ,[DELIVERY_SALE_TAX_AMOUNT_6] = @DELIVERY_SALE_TAX_AMOUNT_6" &
   "   ,[DELIVERY_NET_PRICE_7] = @DELIVERY_NET_PRICE_7" &
   "   ,[DELIVERY_GROSS_PRICE_7] = @DELIVERY_GROSS_PRICE_7" &
   "   ,[DELIVERY_TAX_AMOUNT_7] = @DELIVERY_TAX_AMOUNT_7" &
   "   ,[DELIVERY_SALE_NET_PRICE_7] = @DELIVERY_SALE_NET_PRICE_7" &
   "   ,[DELIVERY_SALE_GROSS_PRICE_7] = @DELIVERY_SALE_GROSS_PRICE_7" &
   "   ,[DELIVERY_SALE_TAX_AMOUNT_7] = @DELIVERY_SALE_TAX_AMOUNT_7" &
   "   ,[DELIVERY_NET_PRICE_8] = @DELIVERY_NET_PRICE_8" &
   "   ,[DELIVERY_GROSS_PRICE_8] = @DELIVERY_GROSS_PRICE_8" &
   "   ,[DELIVERY_TAX_AMOUNT_8] = @DELIVERY_TAX_AMOUNT_8" &
   "   ,[DELIVERY_SALE_NET_PRICE_8] = @DELIVERY_SALE_NET_PRICE_8" &
   "   ,[DELIVERY_SALE_GROSS_PRICE_8] = @DELIVERY_SALE_GROSS_PRICE_8" &
   "   ,[DELIVERY_SALE_TAX_AMOUNT_8] = @DELIVERY_SALE_TAX_AMOUNT_8" &
   "   ,[DELIVERY_NET_PRICE_9] = @DELIVERY_NET_PRICE_9" &
   "   ,[DELIVERY_GROSS_PRICE_9] = @DELIVERY_GROSS_PRICE_9" &
   "   ,[DELIVERY_TAX_AMOUNT_9] = @DELIVERY_TAX_AMOUNT_9" &
   "   ,[DELIVERY_SALE_NET_PRICE_9] = @DELIVERY_SALE_NET_PRICE_9" &
   "   ,[DELIVERY_SALE_GROSS_PRICE_9] = @DELIVERY_SALE_GROSS_PRICE_9" &
   "   ,[DELIVERY_SALE_TAX_AMOUNT_9] = @DELIVERY_SALE_TAX_AMOUNT_9" &
   "   ,[DELIVERY_NET_PRICE_10] = @DELIVERY_NET_PRICE_10" &
   "   ,[DELIVERY_GROSS_PRICE_10] = @DELIVERY_GROSS_PRICE_10" &
   "   ,[DELIVERY_TAX_AMOUNT_10] = @DELIVERY_TAX_AMOUNT_10" &
   "   ,[DELIVERY_SALE_NET_PRICE_10] = @DELIVERY_SALE_NET_PRICE_10" &
   "   ,[DELIVERY_SALE_GROSS_PRICE_10] = @DELIVERY_SALE_GROSS_PRICE_10" &
   "  ,[DELIVERY_SALE_TAX_AMOUNT_10] = @DELIVERY_SALE_TAX_AMOUNT_10" &
" WHERE PRICE_LIST = @PRICE_LIST AND PRODUCT = @PRODUCT "
        Dim err As New ErrorObj
        Dim strSelectBuild As String = String.Empty
        Dim dtrReader As SqlDataReader
        Dim priceListEntryExists As Boolean = False

        ' if replacing products then delete first for that price list
        'If priceList.ProductPricesMode = "REPLACE" Then
        '    cmd = New SqlCommand(strDeleteSQL2, conSql2005)
        '    cmd.Parameters.Add("@PRICE_LIST", SqlDbType.NVarChar, 20).Value = priceList.Code
        '    cmd.ExecuteNonQuery()
        '  End If
        Try
            For Each productPrice As DEProductPrice In priceList.ColProductPrice
                priceListEntryExists = False
                '---------------------------
                ' Check if it already exists
                '---------------------------
                strSelectBuild = "SELECT * FROM " &
                                    file &
                                    " WHERE " &
                                      " PRICE_LIST = @PRICE_LIST AND " &
                                                     " PRODUCT = @PRODUCT  "
                cmd = New SqlCommand(strSelectBuild, conSql2005)
                cmd.Parameters.Add("@PRICE_LIST", SqlDbType.NVarChar, 20).Value = priceList.Code
                cmd.Parameters.Add("@PRODUCT", SqlDbType.NVarChar, 20).Value = productPrice.SKU

                dtrReader = cmd.ExecuteReader

                If dtrReader.HasRows Then
                    priceListEntryExists = True
                End If

                dtrReader.Close()
                '------------------------------------------------
                ' If doing a replace and writing to the work file 
                ' then it's always a replace
                '------------------------------------------------
                If file = "TBL_PRICE_LIST_DETAIL_WORK" Then
                    productPrice.Mode = "ADD"
                End If
                Select Case productPrice.Mode
                    Case Is = "ADD", String.Empty
                        If Not priceListEntryExists Then
                            cmd = New SqlCommand(strSql, conSql2005)
                            AccessDatabaseSQL2005_ProductPriceLoad_ProcessPriceListDetail_setParms(cmd, priceList, productPrice, "UPDATE")
                            cmd.ExecuteNonQuery()
                        Else
                            ' if add and exists, perform an update
                            cmd = New SqlCommand(strUpdateSQL, conSql2005)
                            AccessDatabaseSQL2005_ProductPriceLoad_ProcessPriceListDetail_setParms(cmd, priceList, productPrice, "UPDATE")
                            cmd.ExecuteNonQuery()
                        End If

                    Case Is = "UPDATE"
                        If priceListEntryExists Then
                            cmd = New SqlCommand(strUpdateSQL, conSql2005)
                            AccessDatabaseSQL2005_ProductPriceLoad_ProcessPriceListDetail_setParms(cmd, priceList, productPrice, "UPDATE")
                            cmd.ExecuteNonQuery()
                        End If
                    Case Is = "DELETE"
                        If priceListEntryExists Then
                            cmd = New SqlCommand(strDeleteSQL, conSql2005)
                            cmd.Parameters.Add("@PRICE_LIST", SqlDbType.NVarChar, 20).Value = priceList.Code
                            cmd.Parameters.Add("@PRODUCT", SqlDbType.NVarChar, 20).Value = productPrice.SKU
                            cmd.ExecuteNonQuery()
                        End If

                End Select
            Next
        Catch ex As Exception
            ResultDataSet = Nothing
            Const strError As String = "Error during database access (ProcessPriceListDetail)"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBPD-SQL2005-3"
                .HasError = True
            End With
        End Try

        Return err

    End Function
    Private Sub AccessDatabaseSQL2005_ProductPriceLoad_ProcessPriceListDetail_setParms(ByRef cmd As SqlCommand, ByVal pricelist As DEPriceList, ByVal productPrice As DEProductPrice, ByVal type As String)
        cmd.Parameters.Add("@PRICE_LIST", SqlDbType.NVarChar, 20).Value = pricelist.Code
        cmd.Parameters.Add("@PRODUCT", SqlDbType.NVarChar, 20).Value = productPrice.SKU
        cmd.Parameters.Add("@FROM_DATE", SqlDbType.DateTime).Value = productPrice.FromDate
        cmd.Parameters.Add("@TO_DATE", SqlDbType.DateTime).Value = productPrice.ToDate
        cmd.Parameters.Add("@NET_PRICE", SqlDbType.Decimal).Value = productPrice.Price.NetPrice
        cmd.Parameters.Add("@GROSS_PRICE", SqlDbType.Decimal).Value = productPrice.Price.GrossPrice
        cmd.Parameters.Add("@TAX_AMOUNT", SqlDbType.Decimal).Value = productPrice.Price.Tax
        cmd.Parameters.Add("@SALE_NET_PRICE", SqlDbType.Decimal).Value = productPrice.Price.SalePriceNet
        cmd.Parameters.Add("@SALE_GROSS_PRICE", SqlDbType.Decimal).Value = productPrice.Price.SalePriceGross
        cmd.Parameters.Add("@SALE_TAX_AMOUNT", SqlDbType.Decimal).Value = productPrice.Price.SalePriceTax
        cmd.Parameters.Add("@DELIVERY_NET_PRICE", SqlDbType.Decimal).Value = productPrice.Price.DeliveryGross
        cmd.Parameters.Add("@DELIVERY_GROSS_PRICE", SqlDbType.Decimal).Value = productPrice.Price.DeliveryNet
        cmd.Parameters.Add("@DELIVERY_TAX_AMOUNT", SqlDbType.Decimal).Value = productPrice.Price.DeliveryTax
        cmd.Parameters.Add("@PRICE_1", SqlDbType.Decimal).Value = 0
        cmd.Parameters.Add("@PRICE_2", SqlDbType.Decimal).Value = 0
        cmd.Parameters.Add("@PRICE_3", SqlDbType.Decimal).Value = 0
        cmd.Parameters.Add("@PRICE_4", SqlDbType.Decimal).Value = 0
        cmd.Parameters.Add("@PRICE_5", SqlDbType.Decimal).Value = 0
        cmd.Parameters.Add("@TAX_CODE", SqlDbType.NVarChar, 20).Value = productPrice.TaxCode
        cmd.Parameters.Add("@TARIFF_CODE", SqlDbType.NVarChar, 50).Value = productPrice.TariffCode
        cmd.Parameters.Add("@PRICE_BREAK_CODE", SqlDbType.NVarChar, 50).Value = productPrice.Price.PriceBreakCode
        cmd.Parameters.Add("@PRICE_BREAK_QUANTITY_1", SqlDbType.Decimal).Value = productPrice.PriceBreak1.PriceBreakQty
        cmd.Parameters.Add("@SALE_PRICE_BREAK_QUANTITY_1", SqlDbType.Decimal).Value = productPrice.PriceBreak1.SalePriceBreakQuantity

        cmd.Parameters.Add("@NET_PRICE_2", SqlDbType.Decimal).Value = productPrice.PriceBreak2.NetPrice
        cmd.Parameters.Add("@GROSS_PRICE_2", SqlDbType.Decimal).Value = productPrice.PriceBreak2.GrossPrice
        cmd.Parameters.Add("@TAX_AMOUNT_2", SqlDbType.Decimal).Value = productPrice.PriceBreak2.Tax
        cmd.Parameters.Add("@PRICE_BREAK_QUANTITY_2", SqlDbType.Decimal).Value = productPrice.PriceBreak2.PriceBreakQty
        cmd.Parameters.Add("@SALE_NET_PRICE_2", SqlDbType.Decimal).Value = productPrice.PriceBreak2.SalePriceNet
        cmd.Parameters.Add("@SALE_GROSS_PRICE_2", SqlDbType.Decimal).Value = productPrice.PriceBreak2.SalePriceGross
        cmd.Parameters.Add("@SALE_TAX_AMOUNT_2", SqlDbType.Decimal).Value = productPrice.PriceBreak2.SalePriceTax
        cmd.Parameters.Add("@SALE_PRICE_BREAK_QUANTITY_2", SqlDbType.Decimal).Value = productPrice.PriceBreak2.SalePriceBreakQuantity

        cmd.Parameters.Add("@NET_PRICE_3", SqlDbType.Decimal).Value = productPrice.PriceBreak3.NetPrice
        cmd.Parameters.Add("@GROSS_PRICE_3", SqlDbType.Decimal).Value = productPrice.PriceBreak3.GrossPrice
        cmd.Parameters.Add("@TAX_AMOUNT_3", SqlDbType.Decimal).Value = productPrice.PriceBreak3.Tax
        cmd.Parameters.Add("@PRICE_BREAK_QUANTITY_3", SqlDbType.Decimal).Value = productPrice.PriceBreak3.PriceBreakQty
        cmd.Parameters.Add("@SALE_NET_PRICE_3", SqlDbType.Decimal).Value = productPrice.PriceBreak3.SalePriceNet
        cmd.Parameters.Add("@SALE_GROSS_PRICE_3", SqlDbType.Decimal).Value = productPrice.PriceBreak3.SalePriceGross
        cmd.Parameters.Add("@SALE_TAX_AMOUNT_3", SqlDbType.Decimal).Value = productPrice.PriceBreak3.SalePriceTax
        cmd.Parameters.Add("@SALE_PRICE_BREAK_QUANTITY_3", SqlDbType.Decimal).Value = productPrice.PriceBreak3.SalePriceBreakQuantity

        cmd.Parameters.Add("@NET_PRICE_4", SqlDbType.Decimal).Value = productPrice.PriceBreak4.NetPrice
        cmd.Parameters.Add("@GROSS_PRICE_4", SqlDbType.Decimal).Value = productPrice.PriceBreak4.GrossPrice
        cmd.Parameters.Add("@TAX_AMOUNT_4", SqlDbType.Decimal).Value = productPrice.PriceBreak4.Tax
        cmd.Parameters.Add("@PRICE_BREAK_QUANTITY_4", SqlDbType.Decimal).Value = productPrice.PriceBreak4.PriceBreakQty
        cmd.Parameters.Add("@SALE_NET_PRICE_4", SqlDbType.Decimal).Value = productPrice.PriceBreak4.SalePriceNet
        cmd.Parameters.Add("@SALE_GROSS_PRICE_4", SqlDbType.Decimal).Value = productPrice.PriceBreak4.SalePriceGross
        cmd.Parameters.Add("@SALE_TAX_AMOUNT_4", SqlDbType.Decimal).Value = productPrice.PriceBreak4.SalePriceTax
        cmd.Parameters.Add("@SALE_PRICE_BREAK_QUANTITY_4", SqlDbType.Decimal).Value = productPrice.PriceBreak4.SalePriceBreakQuantity

        cmd.Parameters.Add("@NET_PRICE_5", SqlDbType.Decimal).Value = productPrice.PriceBreak5.NetPrice
        cmd.Parameters.Add("@GROSS_PRICE_5", SqlDbType.Decimal).Value = productPrice.PriceBreak5.GrossPrice
        cmd.Parameters.Add("@TAX_AMOUNT_5", SqlDbType.Decimal).Value = productPrice.PriceBreak5.Tax
        cmd.Parameters.Add("@PRICE_BREAK_QUANTITY_5", SqlDbType.Decimal).Value = productPrice.PriceBreak5.PriceBreakQty
        cmd.Parameters.Add("@SALE_NET_PRICE_5", SqlDbType.Decimal).Value = productPrice.PriceBreak5.SalePriceNet
        cmd.Parameters.Add("@SALE_GROSS_PRICE_5", SqlDbType.Decimal).Value = productPrice.PriceBreak5.SalePriceGross
        cmd.Parameters.Add("@SALE_TAX_AMOUNT_5", SqlDbType.Decimal).Value = productPrice.PriceBreak5.SalePriceTax
        cmd.Parameters.Add("@SALE_PRICE_BREAK_QUANTITY_5", SqlDbType.Decimal).Value = productPrice.PriceBreak5.SalePriceBreakQuantity

        cmd.Parameters.Add("@NET_PRICE_6", SqlDbType.Decimal).Value = productPrice.PriceBreak6.NetPrice
        cmd.Parameters.Add("@GROSS_PRICE_6", SqlDbType.Decimal).Value = productPrice.PriceBreak6.GrossPrice
        cmd.Parameters.Add("@TAX_AMOUNT_6", SqlDbType.Decimal).Value = productPrice.PriceBreak6.Tax
        cmd.Parameters.Add("@PRICE_BREAK_QUANTITY_6", SqlDbType.Decimal).Value = productPrice.PriceBreak6.PriceBreakQty
        cmd.Parameters.Add("@SALE_NET_PRICE_6", SqlDbType.Decimal).Value = productPrice.PriceBreak6.SalePriceNet
        cmd.Parameters.Add("@SALE_GROSS_PRICE_6", SqlDbType.Decimal).Value = productPrice.PriceBreak6.SalePriceGross
        cmd.Parameters.Add("@SALE_TAX_AMOUNT_6", SqlDbType.Decimal).Value = productPrice.PriceBreak6.SalePriceTax
        cmd.Parameters.Add("@SALE_PRICE_BREAK_QUANTITY_6", SqlDbType.Decimal).Value = productPrice.PriceBreak6.SalePriceBreakQuantity

        cmd.Parameters.Add("@NET_PRICE_7", SqlDbType.Decimal).Value = productPrice.PriceBreak7.NetPrice
        cmd.Parameters.Add("@GROSS_PRICE_7", SqlDbType.Decimal).Value = productPrice.PriceBreak7.GrossPrice
        cmd.Parameters.Add("@TAX_AMOUNT_7", SqlDbType.Decimal).Value = productPrice.PriceBreak7.Tax
        cmd.Parameters.Add("@PRICE_BREAK_QUANTITY_7", SqlDbType.Decimal).Value = productPrice.PriceBreak7.PriceBreakQty
        cmd.Parameters.Add("@SALE_NET_PRICE_7", SqlDbType.Decimal).Value = productPrice.PriceBreak7.SalePriceNet
        cmd.Parameters.Add("@SALE_GROSS_PRICE_7", SqlDbType.Decimal).Value = productPrice.PriceBreak7.SalePriceGross
        cmd.Parameters.Add("@SALE_TAX_AMOUNT_7", SqlDbType.Decimal).Value = productPrice.PriceBreak7.SalePriceTax
        cmd.Parameters.Add("@SALE_PRICE_BREAK_QUANTITY_7", SqlDbType.Decimal).Value = productPrice.PriceBreak7.SalePriceBreakQuantity

        cmd.Parameters.Add("@NET_PRICE_8", SqlDbType.Decimal).Value = productPrice.PriceBreak8.NetPrice
        cmd.Parameters.Add("@GROSS_PRICE_8", SqlDbType.Decimal).Value = productPrice.PriceBreak8.GrossPrice
        cmd.Parameters.Add("@TAX_AMOUNT_8", SqlDbType.Decimal).Value = productPrice.PriceBreak8.Tax
        cmd.Parameters.Add("@PRICE_BREAK_QUANTITY_8", SqlDbType.Decimal).Value = productPrice.PriceBreak8.PriceBreakQty
        cmd.Parameters.Add("@SALE_NET_PRICE_8", SqlDbType.Decimal).Value = productPrice.PriceBreak8.SalePriceNet
        cmd.Parameters.Add("@SALE_GROSS_PRICE_8", SqlDbType.Decimal).Value = productPrice.PriceBreak8.SalePriceGross
        cmd.Parameters.Add("@SALE_TAX_AMOUNT_8", SqlDbType.Decimal).Value = productPrice.PriceBreak8.SalePriceTax
        cmd.Parameters.Add("@SALE_PRICE_BREAK_QUANTITY_8", SqlDbType.Decimal).Value = productPrice.PriceBreak8.SalePriceBreakQuantity

        cmd.Parameters.Add("@NET_PRICE_9", SqlDbType.Decimal).Value = productPrice.PriceBreak9.NetPrice
        cmd.Parameters.Add("@GROSS_PRICE_9", SqlDbType.Decimal).Value = productPrice.PriceBreak9.GrossPrice
        cmd.Parameters.Add("@TAX_AMOUNT_9", SqlDbType.Decimal).Value = productPrice.PriceBreak9.Tax
        cmd.Parameters.Add("@PRICE_BREAK_QUANTITY_9", SqlDbType.Decimal).Value = productPrice.PriceBreak9.PriceBreakQty
        cmd.Parameters.Add("@SALE_NET_PRICE_9", SqlDbType.Decimal).Value = productPrice.PriceBreak9.SalePriceNet
        cmd.Parameters.Add("@SALE_GROSS_PRICE_9", SqlDbType.Decimal).Value = productPrice.PriceBreak9.SalePriceGross
        cmd.Parameters.Add("@SALE_TAX_AMOUNT_9", SqlDbType.Decimal).Value = productPrice.PriceBreak9.SalePriceTax
        cmd.Parameters.Add("@SALE_PRICE_BREAK_QUANTITY_9", SqlDbType.Decimal).Value = productPrice.PriceBreak9.SalePriceBreakQuantity

        cmd.Parameters.Add("@NET_PRICE_10", SqlDbType.Decimal).Value = productPrice.PriceBreak10.NetPrice
        cmd.Parameters.Add("@GROSS_PRICE_10", SqlDbType.Decimal).Value = productPrice.PriceBreak10.GrossPrice
        cmd.Parameters.Add("@TAX_AMOUNT_10", SqlDbType.Decimal).Value = productPrice.PriceBreak10.Tax
        cmd.Parameters.Add("@PRICE_BREAK_QUANTITY_10", SqlDbType.Decimal).Value = productPrice.PriceBreak10.PriceBreakQty
        cmd.Parameters.Add("@SALE_NET_PRICE_10", SqlDbType.Decimal).Value = productPrice.PriceBreak10.SalePriceNet
        cmd.Parameters.Add("@SALE_GROSS_PRICE_10", SqlDbType.Decimal).Value = productPrice.PriceBreak10.SalePriceGross
        cmd.Parameters.Add("@SALE_TAX_AMOUNT_10", SqlDbType.Decimal).Value = productPrice.PriceBreak10.SalePriceTax
        cmd.Parameters.Add("@SALE_PRICE_BREAK_QUANTITY_10", SqlDbType.Decimal).Value = productPrice.PriceBreak10.SalePriceBreakQuantity

        cmd.Parameters.Add("@DELIVERY_NET_PRICE_2", SqlDbType.Decimal).Value = productPrice.PriceBreak2.DeliveryNet
        cmd.Parameters.Add("@DELIVERY_GROSS_PRICE_2", SqlDbType.Decimal).Value = productPrice.PriceBreak2.DeliveryGross
        cmd.Parameters.Add("@DELIVERY_TAX_AMOUNT_2", SqlDbType.Decimal).Value = productPrice.PriceBreak2.DeliveryTax
        cmd.Parameters.Add("@DELIVERY_SALE_NET_PRICE_2", SqlDbType.Decimal).Value = productPrice.PriceBreak2.SaleDeliveryNet
        cmd.Parameters.Add("@DELIVERY_SALE_GROSS_PRICE_2", SqlDbType.Decimal).Value = productPrice.PriceBreak2.SaleDeliveryGross
        cmd.Parameters.Add("@DELIVERY_SALE_TAX_AMOUNT_2", SqlDbType.Decimal).Value = productPrice.PriceBreak2.SaleDeliveryTax

        cmd.Parameters.Add("@DELIVERY_NET_PRICE_3", SqlDbType.Decimal).Value = productPrice.PriceBreak3.DeliveryNet
        cmd.Parameters.Add("@DELIVERY_GROSS_PRICE_3", SqlDbType.Decimal).Value = productPrice.PriceBreak3.DeliveryGross
        cmd.Parameters.Add("@DELIVERY_TAX_AMOUNT_3", SqlDbType.Decimal).Value = productPrice.PriceBreak3.DeliveryTax
        cmd.Parameters.Add("@DELIVERY_SALE_NET_PRICE_3", SqlDbType.Decimal).Value = productPrice.PriceBreak3.SaleDeliveryNet
        cmd.Parameters.Add("@DELIVERY_SALE_GROSS_PRICE_3", SqlDbType.Decimal).Value = productPrice.PriceBreak3.SaleDeliveryGross
        cmd.Parameters.Add("@DELIVERY_SALE_TAX_AMOUNT_3", SqlDbType.Decimal).Value = productPrice.PriceBreak3.SaleDeliveryTax

        cmd.Parameters.Add("@DELIVERY_NET_PRICE_4", SqlDbType.Decimal).Value = productPrice.PriceBreak4.DeliveryNet
        cmd.Parameters.Add("@DELIVERY_GROSS_PRICE_4", SqlDbType.Decimal).Value = productPrice.PriceBreak4.DeliveryGross
        cmd.Parameters.Add("@DELIVERY_TAX_AMOUNT_4", SqlDbType.Decimal).Value = productPrice.PriceBreak4.DeliveryTax
        cmd.Parameters.Add("@DELIVERY_SALE_NET_PRICE_4", SqlDbType.Decimal).Value = productPrice.PriceBreak4.SaleDeliveryNet
        cmd.Parameters.Add("@DELIVERY_SALE_GROSS_PRICE_4", SqlDbType.Decimal).Value = productPrice.PriceBreak4.SaleDeliveryGross
        cmd.Parameters.Add("@DELIVERY_SALE_TAX_AMOUNT_4", SqlDbType.Decimal).Value = productPrice.PriceBreak4.SaleDeliveryTax

        cmd.Parameters.Add("@DELIVERY_NET_PRICE_5", SqlDbType.Decimal).Value = productPrice.PriceBreak5.DeliveryNet
        cmd.Parameters.Add("@DELIVERY_GROSS_PRICE_5", SqlDbType.Decimal).Value = productPrice.PriceBreak5.DeliveryGross
        cmd.Parameters.Add("@DELIVERY_TAX_AMOUNT_5", SqlDbType.Decimal).Value = productPrice.PriceBreak5.DeliveryTax
        cmd.Parameters.Add("@DELIVERY_SALE_NET_PRICE_5", SqlDbType.Decimal).Value = productPrice.PriceBreak5.SaleDeliveryNet
        cmd.Parameters.Add("@DELIVERY_SALE_GROSS_PRICE_5", SqlDbType.Decimal).Value = productPrice.PriceBreak5.SaleDeliveryGross
        cmd.Parameters.Add("@DELIVERY_SALE_TAX_AMOUNT_5", SqlDbType.Decimal).Value = productPrice.PriceBreak5.SaleDeliveryTax

        cmd.Parameters.Add("@DELIVERY_NET_PRICE_6", SqlDbType.Decimal).Value = productPrice.PriceBreak6.DeliveryNet
        cmd.Parameters.Add("@DELIVERY_GROSS_PRICE_6", SqlDbType.Decimal).Value = productPrice.PriceBreak6.DeliveryGross
        cmd.Parameters.Add("@DELIVERY_TAX_AMOUNT_6", SqlDbType.Decimal).Value = productPrice.PriceBreak6.DeliveryTax
        cmd.Parameters.Add("@DELIVERY_SALE_NET_PRICE_6", SqlDbType.Decimal).Value = productPrice.PriceBreak6.SaleDeliveryNet
        cmd.Parameters.Add("@DELIVERY_SALE_GROSS_PRICE_6", SqlDbType.Decimal).Value = productPrice.PriceBreak6.SaleDeliveryGross
        cmd.Parameters.Add("@DELIVERY_SALE_TAX_AMOUNT_6", SqlDbType.Decimal).Value = productPrice.PriceBreak6.SaleDeliveryTax

        cmd.Parameters.Add("@DELIVERY_NET_PRICE_7", SqlDbType.Decimal).Value = productPrice.PriceBreak7.DeliveryNet
        cmd.Parameters.Add("@DELIVERY_GROSS_PRICE_7", SqlDbType.Decimal).Value = productPrice.PriceBreak7.DeliveryGross
        cmd.Parameters.Add("@DELIVERY_TAX_AMOUNT_7", SqlDbType.Decimal).Value = productPrice.PriceBreak7.DeliveryTax
        cmd.Parameters.Add("@DELIVERY_SALE_NET_PRICE_7", SqlDbType.Decimal).Value = productPrice.PriceBreak7.SaleDeliveryNet
        cmd.Parameters.Add("@DELIVERY_SALE_GROSS_PRICE_7", SqlDbType.Decimal).Value = productPrice.PriceBreak7.SaleDeliveryGross
        cmd.Parameters.Add("@DELIVERY_SALE_TAX_AMOUNT_7", SqlDbType.Decimal).Value = productPrice.PriceBreak7.SaleDeliveryTax

        cmd.Parameters.Add("@DELIVERY_NET_PRICE_8", SqlDbType.Decimal).Value = productPrice.PriceBreak8.DeliveryNet
        cmd.Parameters.Add("@DELIVERY_GROSS_PRICE_8", SqlDbType.Decimal).Value = productPrice.PriceBreak8.DeliveryGross
        cmd.Parameters.Add("@DELIVERY_TAX_AMOUNT_8", SqlDbType.Decimal).Value = productPrice.PriceBreak8.DeliveryTax
        cmd.Parameters.Add("@DELIVERY_SALE_NET_PRICE_8", SqlDbType.Decimal).Value = productPrice.PriceBreak8.SaleDeliveryNet
        cmd.Parameters.Add("@DELIVERY_SALE_GROSS_PRICE_8", SqlDbType.Decimal).Value = productPrice.PriceBreak8.SaleDeliveryGross
        cmd.Parameters.Add("@DELIVERY_SALE_TAX_AMOUNT_8", SqlDbType.Decimal).Value = productPrice.PriceBreak8.SaleDeliveryTax

        cmd.Parameters.Add("@DELIVERY_NET_PRICE_9", SqlDbType.Decimal).Value = productPrice.PriceBreak9.DeliveryNet
        cmd.Parameters.Add("@DELIVERY_GROSS_PRICE_9", SqlDbType.Decimal).Value = productPrice.PriceBreak9.DeliveryGross
        cmd.Parameters.Add("@DELIVERY_TAX_AMOUNT_9", SqlDbType.Decimal).Value = productPrice.PriceBreak9.DeliveryTax
        cmd.Parameters.Add("@DELIVERY_SALE_NET_PRICE_9", SqlDbType.Decimal).Value = productPrice.PriceBreak9.SaleDeliveryNet
        cmd.Parameters.Add("@DELIVERY_SALE_GROSS_PRICE_9", SqlDbType.Decimal).Value = productPrice.PriceBreak9.SaleDeliveryGross
        cmd.Parameters.Add("@DELIVERY_SALE_TAX_AMOUNT_9", SqlDbType.Decimal).Value = productPrice.PriceBreak9.SaleDeliveryNet

        cmd.Parameters.Add("@DELIVERY_NET_PRICE_10", SqlDbType.Decimal).Value = productPrice.PriceBreak10.DeliveryNet
        cmd.Parameters.Add("@DELIVERY_GROSS_PRICE_10", SqlDbType.Decimal).Value = productPrice.PriceBreak10.DeliveryGross
        cmd.Parameters.Add("@DELIVERY_TAX_AMOUNT_10", SqlDbType.Decimal).Value = productPrice.PriceBreak10.DeliveryTax
        cmd.Parameters.Add("@DELIVERY_SALE_NET_PRICE_10", SqlDbType.Decimal).Value = productPrice.PriceBreak10.SaleDeliveryNet
        cmd.Parameters.Add("@DELIVERY_SALE_GROSS_PRICE_10", SqlDbType.Decimal).Value = productPrice.PriceBreak10.SaleDeliveryGross
        cmd.Parameters.Add("@DELIVERY_SALE_TAX_AMOUNT_10", SqlDbType.Decimal).Value = productPrice.PriceBreak10.SaleDeliveryTax

    End Sub
    '-----------------------------------------
    ' This function is redundant - BF 13/10/08
    '-----------------------------------------
    'Protected Overrides Function AccessDataBaseSQL2005() As ErrorObj

    '    Dim err As New ErrorObj
    '    ResultDataSet = New DataSet
    '    Dim cmdSelect As SqlCommand = Nothing
    '    Dim dtr As SqlDataReader
    '    Dim dr As DataRow

    '    'Create the Stand Descriptions data table
    '    Dim DtStandDescription As New DataTable
    '    ResultDataSet.Tables.Add(DtStandDescription)
    '    With DtStandDescription.Columns
    '        .Add("StandCode", GetType(String))
    '        .Add("StandDescription", GetType(String))
    '        .Add("AreaCode", GetType(String))
    '        .Add("AreaDescription", GetType(String))
    '    End With

    '    Try

    '        'Retrieve the stand and area descriptions from the database
    '        Dim SQLSelect As String = "SELECT * FROM tbl_ticketing_stadium WITH (NOLOCK)   " & _
    '                                    " WHERE  PATNER	    = @Param1   " & _
    '                                    " AND    STADIUM	= @Param2   "

    '        'Execute the command
    '        cmdSelect = New SqlCommand(SQLSelect, conSql2005)
    '        With cmdSelect
    '            .Parameters.Add(New SqlParameter(Param1, SqlDbType.Char)).Value = Settings.Company
    '            .Parameters.Add(New SqlParameter(Param2, SqlDbType.Char)).Value = De.StadiumCode
    '            dtr = .ExecuteReader()
    '        End With

    '        'Read through the data reader
    '        While dtr.Read

    '            'Populate the data row with the description
    '            dr = Nothing
    '            dr = DtStandDescription.NewRow
    '            dr("StandCode") = dtr.Item("STAND").ToString.Trim
    '            dr("StandDescription") = dtr.Item("STAND_DESCRIPTION").ToString.Trim
    '            dr("AreaCode") = dtr.Item("AREA").ToString.Trim
    '            dr("AreaDescription") = dtr.Item("AREA_DESCRIPTION").ToString.Trim
    '            DtStandDescription.Rows.Add(dr)
    '        End While
    '        dtr.Close()


    '    Catch ex As Exception
    '        ResultDataSet = Nothing
    '        Const strError As String = "Error during database access"
    '        With err
    '            .ErrorMessage = ex.Message
    '            .ErrorStatus = strError
    '            .ErrorNumber = "TACDBPD-SQL2005-1"
    '            .HasError = True
    '        End With
    '    End Try


    '    Return err

    'End Function
#End Region

#Region "CHORUS"
    Protected Overrides Function AccessDataBaseCHORUS() As ErrorObj
        Dim err As New ErrorObj

        Select Case _settings.ModuleName
            Case Is = "RetrieveAlternativeProducts"
                err = AccessDataBaseCHORUS_RetrieveAlternativeProducts()
            Case Else

        End Select

        Return err
    End Function

    Protected Function AccessDataBaseCHORUS_RetrieveAlternativeProducts() As ErrorObj
        Dim err As New ErrorObj
        ResultDataSet = New DataSet
        Dim dRow As DataRow = Nothing
        Dim PARAMOUT1 As String = String.Empty
        Dim PARAMOUT2 As String = String.Empty
        Dim moreRecords As Boolean = True
        Dim moreProducts As Boolean = True
        Dim count1 As Integer = 0
        Dim count2 As Integer = 0
        ' Collection index - collections are indexed from 1
        Dim productCount As Integer = 1
        Dim productResultsReceived As Boolean = False
        Dim lastProduct As String = String.Empty
        Dim lastAltProduct As String = String.Empty

        'Create the Status data table
        Dim DtStatusResults As New DataTable("STATUSRESULTS")
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        'Create the product results data table
        Dim DtProductResults As New DataTable("PRODUCTRESULTS")
        ResultDataSet.Tables.Add(DtProductResults)
        With DtProductResults.Columns
            .Add("ProductCode", GetType(String))
            .Add("OnStop", GetType(String))
        End With

        'Create the alt product results data table
        Dim dtAltProducts As New DataTable("ALTPRODUCTRESULTS")
        ResultDataSet.Tables.Add(dtAltProducts)
        With dtAltProducts.Columns
            .Add("ProductCode", GetType(String))
            .Add("AltProductCode", GetType(String))
        End With

        Try
            'Loop until no more products available
            Do While moreProducts
                productResultsReceived = False
                lastProduct = String.Empty
                lastAltProduct = String.Empty
                moreRecords = True

                ' Loop until no more records available for this set of products
                Do While moreRecords

                    'Call RTVALTPRD
                    Dim cmdSELECT As iDB2Command = Nothing
                    Dim strProgram As String = "RTVALTPRD"
                    Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim &
                                                "/" & strProgram & "(@PARAM1,@PARAM2)"
                    Dim parmIO, parmIO2 As iDB2Parameter

                    'Set the connection string
                    cmdSELECT = New iDB2Command(strHEADER, conChorus)

                    'Populate the parameter
                    parmIO = cmdSELECT.Parameters.Add("@PARAM1", iDB2DbType.iDB2Char, 2048)
                    parmIO.Value = AccessDataBaseCHORUS_RetrieveAlternativeProducts_PARM(productCount, moreProducts)
                    parmIO.Direction = ParameterDirection.InputOutput

                    parmIO2 = cmdSELECT.Parameters.Add("@PARAM2", iDB2DbType.iDB2Char, 4096)
                    parmIO2.Value = Utilities.FixStringLength(" ", 4000) &
                                    Utilities.FixStringLength(lastProduct, 30) &
                                    Utilities.FixStringLength(lastAltProduct, 30)

                    parmIO2.Direction = ParameterDirection.InputOutput

                    TalentCommonLog("CallRTVALTPRD", "", "Backend Request: strHEADER=" & strHEADER & ", parmIO.Value=" & parmIO.Value)

                    cmdSELECT.ExecuteNonQuery()
                    PARAMOUT1 = cmdSELECT.Parameters("@PARAM1").Value.ToString
                    PARAMOUT2 = cmdSELECT.Parameters("@PARAM2").Value.ToString

                    TalentCommonLog("CallRTVALTPRD", "", "Backend Response: PARAMOUT=" & PARAMOUT2)

                    'Set the response data
                    dRow = Nothing
                    dRow = DtStatusResults.NewRow

                    If PARAMOUT2.Substring(4095, 1) = "E" Or PARAMOUT2.Substring(4093, 2).Trim <> "" Then
                        dRow("ErrorOccurred") = "E"
                        dRow("ReturnCode") = PARAMOUT2.Substring(4093, 2)
                    Else
                        dRow("ErrorOccurred") = ""
                        dRow("ReturnCode") = ""
                    End If

                    DtStatusResults.Rows.Add(dRow)

                    'No errors 
                    If PARAMOUT2.Substring(4095, 1) <> "E" And PARAMOUT2.Substring(4093, 2).Trim = "" Then
                        If Not productResultsReceived Then
                            productResultsReceived = True
                            ' Extract the product info (e.g. stop flag)
                            count1 = 30
                            Do While count1 < 2000 AndAlso PARAMOUT1.Substring(count1, 30).Trim <> String.Empty
                                dRow = Nothing
                                dRow = DtProductResults.NewRow
                                dRow("ProductCode") = PARAMOUT1.Substring(count1, 30)
                                dRow("OnStop") = PARAMOUT1.Substring(count1 + 30, 1)
                                DtProductResults.Rows.Add(dRow)
                                count1 += 50
                            Loop
                        End If

                        'Extract the alt product info
                        count2 = 0

                        Do While count2 < 4000 AndAlso PARAMOUT2.Substring(count2, 30).Trim <> String.Empty
                            dRow = Nothing
                            dRow = dtAltProducts.NewRow
                            dRow("ProductCode") = PARAMOUT2.Substring(count2, 30).Trim
                            dRow("AltProductCode") = PARAMOUT2.Substring(count2 + 30, 30).Trim
                            dtAltProducts.Rows.Add(dRow)
                            count2 += 100
                        Loop
                        If count2 >= 4000 Then
                            ' Set last items for next call
                            lastProduct = dRow("ProductCode").ToString
                            lastAltProduct = dRow("AltProductCode").ToString
                            moreRecords = True
                        Else
                            moreRecords = False
                        End If
                    End If
                Loop

            Loop

        Catch ex As Exception
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBPD-RTVALTPRD"
                .HasError = True
            End With
        End Try
        Return err
    End Function

    Private Function AccessDataBaseCHORUS_RetrieveAlternativeProducts_PARM(ByRef productCount As Integer,
                                                                          ByRef moreProducts As Boolean) As String

        Dim myString As String = Utilities.FixStringLength("", 30)
        Dim count1 As Integer = 0

        Do While productCount <= _productCollection.Count AndAlso count1 < 38
            myString &= Utilities.FixStringLength(_productCollection.Item(productCount).ToString, 30) &
                        Utilities.FixStringLength(" ", 20)
            productCount += 1
            count1 += 1
        Loop

        If count1 = 38 AndAlso productCount <= ProductCollection.Count Then
            moreProducts = True
        Else
            moreProducts = False
        End If

        Return Utilities.FixStringLength(myString, 2048)

    End Function

#End Region

#Region "SYSTEM21"
    Protected Overrides Function AccessDataBaseSYSTEM21() As ErrorObj
        Dim err As New ErrorObj

        Select Case _settings.ModuleName
            Case Is = "RetrieveAlternativeProducts"
                err = AccessDataBaseSYSTEM21_RetrieveAlternativeProducts()
            Case Else

        End Select

        Return err
    End Function

    Protected Function AccessDataBaseSYSTEM21_RetrieveAlternativeProducts() As ErrorObj
        Dim err As New ErrorObj
        ResultDataSet = New DataSet
        Dim dRow As DataRow = Nothing
        Dim PARAMOUT1 As String = String.Empty
        Dim PARAMOUT2 As String = String.Empty
        Dim moreRecords As Boolean = True
        Dim moreProducts As Boolean = True
        Dim count1 As Integer = 0
        Dim count2 As Integer = 0
        ' Collection index - collections are indexed from 1
        Dim productCount As Integer = 1
        Dim productResultsReceived As Boolean = False
        Dim lastProduct As String = String.Empty
        Dim lastAltProduct As String = String.Empty

        'Create the Status data table
        Dim DtStatusResults As New DataTable("STATUSRESULTS")
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        'Create the product results data table
        Dim DtProductResults As New DataTable("PRODUCTRESULTS")
        ResultDataSet.Tables.Add(DtProductResults)
        With DtProductResults.Columns
            .Add("ProductCode", GetType(String))
            .Add("OnStop", GetType(String))
        End With

        'Create the alt product results data table
        Dim dtAltProducts As New DataTable("ALTPRODUCTRESULTS")
        ResultDataSet.Tables.Add(dtAltProducts)
        With dtAltProducts.Columns
            .Add("ProductCode", GetType(String))
            .Add("AltProductCode", GetType(String))
        End With

        Try
            '--------------------------
            ' Read Alt Items file INP38
            '--------------------------
            Const sqlSelect As String = "SELECT * FROM INAITMRF WHERE CONO38 = @PARAM1 AND ALTI38 = @PARAM2"
            Dim cmdSelect As iDB2Command = Nothing
            Dim dtrReader As iDB2DataReader = Nothing
            Dim param1 As String = "@PARAM1"
            Dim param2 As String = "@PARAM2"
            Try
                cmdSelect = New iDB2Command(sqlSelect, conSystem21)
                productCount = 1
                '-----------------------------------------------------------------------------
                Select Case Settings.DatabaseType1
                    Case Is = T65535
                        cmdSelect.Parameters.Add(param1, iDB2DbType.iDB2CharBitData, 2).Value = Settings.AccountNo3
                        cmdSelect.Parameters.Add(param2, iDB2DbType.iDB2CharBitData, 36).Value = _productCollection.Item(productCount)
                    Case Is = T285
                        cmdSelect.Parameters.Add(param1, iDB2DbType.iDB2Char, 2).Value = Settings.AccountNo3
                        cmdSelect.Parameters.Add(param2, iDB2DbType.iDB2Char, 36).Value = _productCollection.Item(productCount)
                    Case Else
                        cmdSelect.Parameters.Add(param1, iDB2DbType.iDB2Char, 2).Value = Settings.AccountNo3
                        cmdSelect.Parameters.Add(param2, iDB2DbType.iDB2Char, 36).Value = _productCollection.Item(productCount)
                End Select
                '-----------------------------------------------------------------------------
                dtrReader = cmdSelect.ExecuteReader()
                Dim found As Boolean = False
                Dim sku As String = String.Empty
                With dtrReader
                    While dtrReader.Read AndAlso Not found
                        If .GetString(.GetOrdinal("EXTA38")).Trim = Settings.AccountNo1.Trim Then
                            found = True
                            sku = .GetString(.GetOrdinal("PNUM38")).Trim
                        End If
                    End While
                    .Close()
                End With
                If sku <> String.Empty Then
                    Dim dr As Data.DataRow = dtAltProducts.NewRow
                    dr("ProductCode") = sku
                    dr("AltProductCode") = _productCollection.Item(productCount)
                    dtAltProducts.Rows.Add(dr)
                End If
            Catch ex As Exception
                Const strError As String = "Failed to Read Alt Parts:" ' & ex.Message
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TACDBOR75"
                    .HasError = True
                End With
            End Try

        Catch ex As Exception
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBPD-RTVALTPRDSYS21"
                .HasError = True
            End With
        End Try
        Return err
    End Function

#End Region

#Region "DEV"
    Protected Overrides Function AccessDataBaseDev() As ErrorObj
        Dim err As New ErrorObj
        Select Case _settings.ModuleName
            Case Is = ProductDetails

                Dim DtStatusResults As New DataTable

                'Create the Status data table
                ResultDataSet = New DataSet
                ResultDataSet.Tables.Add(DtStatusResults)
                With DtStatusResults.Columns
                    .Add("ErrorOccurred", GetType(String))
                    .Add("ReturnCode", GetType(String))
                End With
                DtStatusResults.Rows.Add("", "")

                'Create the product Price Band Details data table
                Dim DtPriceBandResults As New DataTable
                ResultDataSet.Tables.Add(DtPriceBandResults)
                With DtPriceBandResults.Columns
                    .Add("PriceBand", GetType(String))
                    .Add("PriceBandDescription", GetType(String))
                    .Add("PriceBandPriority", GetType(String))
                End With
                DtPriceBandResults.Rows.Add("", "", "")

                'Create the product  Details data table
                Dim DtProdDetailsResults As New DataTable
                ResultDataSet.Tables.Add(DtProdDetailsResults)
                With DtProdDetailsResults.Columns
                    .Add("ProductCode", GetType(String))
                    .Add("ProductText1", GetType(String))
                    .Add("ProductText2", GetType(String))
                    .Add("ProductText3", GetType(String))
                    .Add("ProductText4", GetType(String))
                    .Add("ProductText5", GetType(String))
                    .Add("ProductDetail1", GetType(String))
                    .Add("ProductDetail2", GetType(String))
                    .Add("ProductDetail3", GetType(String))
                    .Add("ProductDetail4", GetType(String))
                    .Add("ProductDetail5", GetType(String))
                    .Add("ProductDescription", GetType(String))
                    .Add("ProductDate", GetType(String))
                    .Add("ProductTime", GetType(String))
                    .Add("ProductType", GetType(String))
                    .Add("LoyalityOnOff", GetType(String))
                    .Add("ProductReqdLoyalityPoints", GetType(String))
                    .Add("MembLoyalityPointsTotal", GetType(String))
                    .Add("MembFavStand", GetType(String))
                    .Add("MembFavArea", GetType(String))
                    .Add("UseVisualSeatLevelSelection", GetType(Boolean))
                    .Add("CoReqProductGroup", GetType(String))
                    .Add("LoyaltyRequirementMet", GetType(String))
                    .Add("LimitRequirementMet", GetType(String))
                End With
                Dim r As DataRow = Nothing
                r = DtProdDetailsResults.NewRow
                r("ProductCode") = "HOME91"
                r("ProductText1") = "HOME91 Also on web"
                r("ProductText2") = ""
                r("ProductText3") = ""
                r("ProductText4") = ""
                r("ProductText5") = ""
                r("ProductDetail1") = "Home 91 also on web"
                r("ProductDetail2") = ""
                r("ProductDetail3") = ""
                r("ProductDetail4") = ""
                r("ProductDetail5") = ""
                r("ProductDescription") = "Home 91 also on web"
                r("ProductDate") = "Thu 1091015"
                r("ProductTime") = "8pm"
                r("ProductType") = "H"
                r("LoyalityOnOff") = "Y"
                r("ProductReqdLoyalityPoints") = "00000"
                r("MembLoyalityPointsTotal") = "00000"
                r("MembFavStand") = ""
                r("MembFavArea") = ""
                r("UseVisualSeatLevelSelection") = "N"
                r("CoReqProductGroup") = ""
                r("LoyaltyRequirementMet") = ""
                r("LimitRequirementMet") = ""
                DtProdDetailsResults.Rows.Add(r)


            Case Is = ProductStadiumAvailability

                ResultDataSet = New DataSet

                'Create the Status data table
                Dim DtStatusResults As New DataTable
                ResultDataSet.Tables.Add(DtStatusResults)
                With DtStatusResults.Columns
                    .Add("ErrorOccurred", GetType(String))
                    .Add("ReturnCode", GetType(String))
                End With
                DtStatusResults.Rows.Add("", "")

                'Create the product list data table
                Dim DtProductStadiumAvailability As New DataTable
                ResultDataSet.Tables.Add(DtProductStadiumAvailability)
                With DtProductStadiumAvailability.Columns
                    .Add("ProductCode", GetType(String))
                    .Add("StandCode", GetType(String))
                    .Add("AreaCode", GetType(String))
                    .Add("Availability", GetType(String))
                    .Add("AdditionalText", GetType(String))
                    .Add("Capacity", GetType(String))
                    .Add("Reserved", GetType(String))
                    .Add("SeatSelection", GetType(String))
                End With

                Dim r As DataRow = Nothing
                r = DtProductStadiumAvailability.NewRow
                r("ProductCode") = "HOME91"
                r("StandCode") = "E"
                r("AreaCode") = "EA"
                r("Availability") = "50"
                r("AdditionalText") = ""
                r("Capacity") = "0000303"
                r("Reserved") = "0000152"
                r("SeatSelection") = "Y"
                DtProductStadiumAvailability.Rows.Add(r)

            Case "StandDescriptions"

                ResultDataSet = New DataSet

                'Create the Status data table
                Dim DtStatusResults As New DataTable
                ResultDataSet.Tables.Add(DtStatusResults)
                With DtStatusResults.Columns
                    .Add("ErrorOccurred", GetType(String))
                    .Add("ReturnCode", GetType(String))
                    .Add("StadiumCode", GetType(String))
                End With
                DtStatusResults.Rows.Add("", "", "")

                'Create the stand details data table
                Dim DtStadiumDetailsResults As New DataTable
                ResultDataSet.Tables.Add(DtStadiumDetailsResults)
                With DtStadiumDetailsResults.Columns
                    .Add("StandCode", GetType(String))
                    .Add("StandDescription", GetType(String))
                    .Add("AreaCode", GetType(String))
                    .Add("AreaDescription", GetType(String))
                End With

                Dim r As DataRow = Nothing
                r = DtStadiumDetailsResults.NewRow
                r("StandCode") = "E"
                r("StandDescription") = "East Stand"
                r("AreaCode") = "EA"
                r("AreaDescription") = "East Area A"
                DtStadiumDetailsResults.Rows.Add(r)

            Case Else
                ' Throw an error?  We have not yet set up dunmmy data for the requested module.
                Dim errorString As String = "There has not yet been any dummy data set up for the requested module: " & _settings.ModuleName
                Const errorStatus As String = "No Dummy data set up for module"
                With err
                    .ErrorMessage = errorString
                    .ErrorStatus = errorStatus
                    .ErrorNumber = "TACDABAS-DEV-1"
                    .HasError = True
                End With
        End Select
        Return err
    End Function

#End Region
End Class