Imports System.Data.SqlClient
Imports System.Text

Namespace DataObjects.TableObjects
    ''' <summary>
    ''' Provides the functionality to manage data for the table tbl_basket_detail_exceptions based on business functionality
    ''' </summary>
    <Serializable()> _
    Public Class tbl_basket_detail_exceptions
        Inherits DBObjectBase

#Region "Class Level Fields"

        ''' <summary>
        ''' Instance of DESettings
        ''' </summary>
        Private _settings As New DESettings

        ''' <summary>
        ''' Class Name which is used in cache key construction
        ''' </summary>
        Const CACHEKEY_CLASSNAME As String = "tbl_basket_detail_exceptions"

#End Region

#Region "Constructors"
        Sub New()
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="tbl_basket_detail_exceptions" /> class.
        ''' </summary>
        ''' <param name="settings">The DESettings instance</param>
        Sub New(ByVal settings As DESettings)
            MyBase.New()
            _settings = settings
        End Sub
#End Region

#Region "Properties"

#End Region

#Region "Public Methods"

        ''' <summary>
        ''' Deletes the specified records by basket header id and module.
        ''' </summary>
        ''' <param name="basketHeaderID">The basket header id</param>
        ''' <param name="module_">The module ("module" is a keyword, hence the underscore)</param>
        ''' <param name="givenTransaction">The given transaction.</param>
        ''' <returns>No of affected rows</returns>
        Public Function DeleteByBasketHeaderIdAndModule(ByVal basketHeaderId As String, ByVal module_ As String, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer
            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj
            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheStringExtension = String.Empty
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            Dim sqlStatement As String = "DELETE [tbl_basket_detail_exceptions] WHERE BASKET_HEADER_ID=@BasketHeaderID AND MODULE=@Module"
            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BasketHeaderID", basketHeaderId))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Module", module_))

            'Execute
            If (givenTransaction Is Nothing) Then
                err = talentSqlAccessDetail.SQLAccess()
            Else
                err = talentSqlAccessDetail.SQLAccess(givenTransaction)
            End If
            If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                affectedRows = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
            End If
            talentSqlAccessDetail = Nothing

            'Return results
            Return affectedRows
        End Function

        ''' <summary>
        ''' Gets the table records by basket header id and module.
        ''' </summary>
        ''' <param name="basketHeaderID">The basket header id</param>
        ''' <param name="module_">The module ("module" is a keyword, hence the underscore)</param>
        ''' <returns>Data table of results</returns>
        Public Function GetByBasketDetailHeaderIDAndModule(ByVal basketHeaderId As String, ByVal module_ As String) As DataTable
            Dim outputDataTable As New DataTable
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = False
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandText = "SELECT * FROM [tbl_basket_detail_exceptions] WHERE BASKET_HEADER_ID = @BasketHeaderID AND MODULE = @Module"
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BasketHeaderID", basketHeaderId))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Module", module_))

                'Execute
                Dim err As New ErrorObj
                err = talentSqlAccessDetail.SQLAccess()
                If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                    outputDataTable = talentSqlAccessDetail.ResultDataSet.Tables(0)
                End If
            Catch ex As Exception
                Throw
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            'Return the results 
            Return outputDataTable
        End Function

        ''' <summary>
        ''' Gets the table records by basket header id, season ticket product and module.
        ''' </summary>
        ''' <param name="basketHeaderID">The basket header id</param>
        ''' <param name="productCode">The product code</param>
        ''' <param name="module_">The module ("module" is a keyword, hence the underscore)</param>
        ''' <returns>Data table of results</returns>
        Public Function GetByBasketDetailHeaderIDProductCodeAndModule(ByVal basketHeaderId As String, ByVal productCode As String, ByVal module_ As String) As List(Of DESeatDetails)
            Dim exceptionSeatsList As New List(Of DESeatDetails)
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = False
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandText = "SELECT * FROM [tbl_basket_detail_exceptions] WHERE BASKET_HEADER_ID = @BasketHeaderID AND PRODUCT_CODE = @ProductCode AND MODULE = @Module"
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BasketHeaderID", basketHeaderId))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ProductCode", productCode))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Module", module_))

                'Execute
                Dim err As New ErrorObj
                err = talentSqlAccessDetail.SQLAccess()
                If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                    For Each row As DataRow In talentSqlAccessDetail.ResultDataSet.Tables(0).Rows
                        Dim exceptionSeat As New DESeatDetails
                        exceptionSeat.Stand = row("SEAT").ToString().Substring(0, 3).Trim()
                        exceptionSeat.Area = row("SEAT").ToString().Substring(3, 4).Trim()
                        exceptionSeat.Row = row("SEAT").ToString().Substring(7, 4).Trim()
                        exceptionSeat.Seat = row("SEAT").ToString().Substring(11, 5)
                        exceptionSeatsList.Add(exceptionSeat)
                    Next
                End If
            Catch ex As Exception
                Throw
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            'Return the results 
            Return exceptionSeatsList
        End Function

        ''' <summary>
        ''' Get the season ticket product based on the given basket details
        ''' </summary>
        ''' <param name="basketHeaderId">The basket header id</param>
        ''' <param name="exceptionProductCode">The exception home product code</param>
        ''' <param name="module_">The module ("module" is a keyword, hence the underscore)</param>
        ''' <returns>The season ticket product code that the exception belongs to</returns>
        ''' <remarks></remarks>
        Public Function GetSTProductCode(ByVal basketHeaderId As String, ByVal exceptionProductCode As String, ByVal module_ As String) As String
            Dim seasonTicketProductCode As String = String.Empty
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = False
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandText = "SELECT [SEASON_TICKET_PRODUCT] FROM [tbl_basket_detail_exceptions] WHERE BASKET_HEADER_ID = @BasketHeaderID AND PRODUCT_CODE = @ProductCode AND MODULE = @Module"
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BasketHeaderID", basketHeaderId))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ProductCode", exceptionProductCode))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Module", module_))

                'Execute
                Dim err As New ErrorObj
                err = talentSqlAccessDetail.SQLAccess()
                If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                    If talentSqlAccessDetail.ResultDataSet.Tables(0).Rows.Count > 0 Then
                        seasonTicketProductCode = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
                    End If
                End If
            Catch ex As Exception
                Throw
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            'Return the results 
            Return seasonTicketProductCode
        End Function

        ''' <summary>
        ''' Insert the season ticket exceptions basket exception product records
        ''' </summary>
        ''' <param name="basketHeaderID">The basket header id</param>
        ''' <param name="dtBasketDetailExceptions">The basket exceptions table to use to write to tbl_basket_detail_exceptions</param>
        ''' <param name="givenTransaction">The given transaction</param>
        ''' <returns>The number of affected rows</returns>
        ''' <remarks></remarks>
        Public Function InsertBasketDetailExceptionRecords(ByVal basketHeaderID As String, ByVal dtBasketDetailExceptions As DataTable, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer
            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj
            Dim sqlStatement As New StringBuilder
            Dim quantityFromBackend As Integer = 0

            sqlStatement.Append("INSERT INTO [dbo].[tbl_basket_detail_exceptions] (")
            sqlStatement.Append("[BASKET_HEADER_ID],[PRODUCT_CODE],[QUANTITY],[PRICE],[ORIGINAL_PRICE],[MODULE],[SEAT],[SEASON_TICKET_SEAT],[SEASON_TICKET_PRODUCT],[HAS_AVAILABILITY],[SEAT_TYPE],")
            sqlStatement.Append("[PRICE_BAND],[LOGINID],[PRODUCT_DESCRIPTION1],[PRODUCT_DESCRIPTION2],[PRODUCT_OPPOSITION_CODE],[PRODUCT_DATE],[PRODUCT_TIME],[ERROR],")
            sqlStatement.Append("[PRICE_CODE],[PRODUCT_TYPE],[PRODUCT_SUB_TYPE],[PRODUCT_TYPE_ACTUAL],[RESTRICTION_CODE],[CANNOT_APPLY_FEES],[CAT_FLAG],[STADIUM_CODE],[PACKAGE_ID])")
            sqlStatement.Append("VALUES (")
            sqlStatement.Append("@BasketHeaderID,@ProductCode,@Quantity,@Price,@OriginalPrice,@Module,@Seat,@SeasonTicketSeat,@SeasonTicketProduct,@HasAvailability,@SeatType,")
            sqlStatement.Append("@PriceBand,@loginID,@ProductDescription1,@ProductDescription2,@ProductOppositionCode,@ProductDate,@ProductTime,@Error,")
            sqlStatement.Append("@PriceCode,@ProductType,@ProductSubType,@ProductTypeActual,@RestrictionCode,@CannotApplyFees,@CATFlag,@StadiumCode,@PackageId)")

            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheStringExtension = String.Empty
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement.ToString()

            'loop it here so clear the command parameters first and then add it
            For rowIndex As Integer = 0 To dtBasketDetailExceptions.Rows.Count - 1
                talentSqlAccessDetail.CommandElements.CommandParameter.Clear()
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BasketHeaderID", basketHeaderID))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ProductCode", Utilities.CheckForDBNull_String(dtBasketDetailExceptions.Rows(rowIndex)("ProductCode"))))
                quantityFromBackend = Utilities.CheckForDBNull_Decimal(dtBasketDetailExceptions.Rows(rowIndex)("Quantity"))
                If quantityFromBackend <= 0 Then
                    quantityFromBackend = 1
                End If
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Quantity", quantityFromBackend))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Price", Utilities.CheckForDBNull_Decimal(dtBasketDetailExceptions.Rows(rowIndex)("Price"))))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@OriginalPrice", 0))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Module", "Ticketing"))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Seat", Utilities.CheckForDBNull_String(dtBasketDetailExceptions.Rows(rowIndex)("Seat"))))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@SeasonTicketSeat", Utilities.CheckForDBNull_String(dtBasketDetailExceptions.Rows(rowIndex)("SeasonTicketSeat"))))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@SeasonTicketProduct", Utilities.CheckForDBNull_String(dtBasketDetailExceptions.Rows(rowIndex)("SeasonTktProduct"))))
                If String.IsNullOrEmpty(dtBasketDetailExceptions.Rows(rowIndex)("HasAvailability")) Then
                    talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@HasAvailability", True))
                Else
                    talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@HasAvailability", Utilities.convertToBool(dtBasketDetailExceptions.Rows(rowIndex)("HasAvailability"))))
                End If
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@SeatType", Utilities.CheckForDBNull_String(dtBasketDetailExceptions.Rows(rowIndex)("SeatType"))))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PriceBand", Utilities.CheckForDBNull_String(dtBasketDetailExceptions.Rows(rowIndex)("PriceBand"))))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@loginID", Utilities.CheckForDBNull_String(dtBasketDetailExceptions.Rows(rowIndex)("CustomerNo"))))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ProductDescription1", Utilities.CheckForDBNull_String(dtBasketDetailExceptions.Rows(rowIndex)("ProductDesc"))))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ProductDescription2", Utilities.CheckForDBNull_String(dtBasketDetailExceptions.Rows(rowIndex)("ProductCompText"))))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ProductOppositionCode", Utilities.CheckForDBNull_String(dtBasketDetailExceptions.Rows(rowIndex)("ProductOppoCode"))))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ProductDate", Utilities.CheckForDBNull_String(dtBasketDetailExceptions.Rows(rowIndex)("ProductDate"))))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ProductTime", Utilities.CheckForDBNull_String(dtBasketDetailExceptions.Rows(rowIndex)("ProductTime"))))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Error", False))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PriceCode", Utilities.CheckForDBNull_String(dtBasketDetailExceptions.Rows(rowIndex)("PriceCode"))))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ProductType", GlobalConstants.HOMEPRODUCTTYPE))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ProductSubType", Utilities.CheckForDBNull_String(dtBasketDetailExceptions.Rows(rowIndex)("ProductSubType"))))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ProductTypeActual", GlobalConstants.HOMEPRODUCTTYPE))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@RestrictionCode", Utilities.CheckForDBNull_String(dtBasketDetailExceptions.Rows(rowIndex)("RestrictionCode"))))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@CannotApplyFees", True))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@CATFlag", Utilities.CheckForDBNull_String(dtBasketDetailExceptions.Rows(rowIndex)("CATFlag"))))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@StadiumCode", Utilities.CheckForDBNull_String(dtBasketDetailExceptions.Rows(rowIndex)("StadiumCode"))))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PackageId", Utilities.CheckForDBNull_String(dtBasketDetailExceptions.Rows(rowIndex)("PackageId"))))

                'Execute
                If (givenTransaction Is Nothing) Then
                    err = talentSqlAccessDetail.SQLAccess()
                Else
                    err = talentSqlAccessDetail.SQLAccess(givenTransaction)
                End If
                If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                    affectedRows = rowIndex + 1
                Else
                    affectedRows = -1
                End If
            Next

            talentSqlAccessDetail = Nothing

            'Return results
            Return affectedRows
        End Function

        ''' <summary>
        ''' Gets the table records by basket header id, module, season ticket product code and package id.
        ''' </summary>
        ''' <param name="basketHeaderID">The basket header id</param>      
        ''' <param name="module_">The module ("module" is a keyword, hence the underscore)</param>
        ''' <param name="seasonTicketProduct">The season ticket product code</param>
        ''' <param name="packageId">The package id</param>
        ''' <returns>Data table of results</returns>
        Public Function GetByBasketDetailHeaderIDModuleCorporateSTProductAndPackage(ByVal basketHeaderId As String, ByVal module_ As String, ByVal seasonTicketProduct As String, ByVal packageId As Long) As DataTable
            Dim outputDataTable As New DataTable
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = False
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandText = "SELECT * FROM [tbl_basket_detail_exceptions] WHERE BASKET_HEADER_ID = @BasketHeaderID AND MODULE = @Module AND SEASON_TICKET_PRODUCT = @SeasonTicketProduct AND PACKAGE_ID = @PackageId "
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BasketHeaderID", basketHeaderId))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Module", module_))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@SeasonTicketProduct", seasonTicketProduct))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PackageId", packageId))

                'Execute
                Dim err As New ErrorObj
                err = talentSqlAccessDetail.SQLAccess()
                If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                    outputDataTable = talentSqlAccessDetail.ResultDataSet.Tables(0)
                End If
            Catch ex As Exception
                Throw
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            'Return the results 
            Return outputDataTable
        End Function
#End Region

    End Class
End Namespace
