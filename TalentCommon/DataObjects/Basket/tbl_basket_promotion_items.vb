Imports System.Data.SqlClient
Imports System.Text

Namespace DataObjects.TableObjects
    ''' <summary>
    ''' Provides the functionality to manage data for the table tbl_basket_promotion_items based on business functionality
    ''' </summary>
    <Serializable()> _
    Public Class tbl_basket_promotion_items
        Inherits DBObjectBase
#Region "Class Level Fields"

        ''' <summary>
        ''' Instance of DESettings
        ''' </summary>
        Private _settings As New DESettings

        ''' <summary>
        ''' Class Name which is used in cache key construction
        ''' </summary>
        Const CACHEKEY_CLASSNAME As String = "tbl_basket_promotion_items"

#End Region

#Region "Constructors"
        Sub New()
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="tbl_basket_promotion_items" /> class.
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
            Dim sqlStatement As String = "DELETE [tbl_basket_promotion_items] WHERE BASKET_HEADER_ID=@BasketHeaderID AND MODULE=@Module"
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
        Public Function GetByBasketDetailIDAndModule(ByVal basketHeaderId As String, ByVal module_ As String) As DataTable
            Dim outputDataTable As New DataTable
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = False
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandText = "SELECT * FROM [tbl_basket_promotion_items] WHERE BASKET_HEADER_ID = @BasketHeaderID AND MODULE = @Module"
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
        ''' Inserts the specified records by basket header id and module.
        ''' </summary>
        ''' <param name="basketHeaderID">The basket header id</param>
        ''' <param name="basketDetailId">The basket detail id</param>
        ''' <param name="promotionId">The promotion id</param>
        ''' <param name="givenTransaction">The given transaction.</param>
        ''' <returns>No of affected rows</returns>
        Public Function InsertTicketingPromotion(ByVal basketHeaderId As String, ByVal basketDetailId As String, ByVal promotionId As Integer, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer
            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj
            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheStringExtension = String.Empty
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery

            Dim sqlStatement As New StringBuilder
            sqlStatement.Append("INSERT INTO [tbl_basket_promotion_items] ")
            sqlStatement.Append("([BASKET_HEADER_ID], [BASKET_DETAIL_ID], [PROMOTION_CODE], [PRODUCT_CODE], [REQUIRES_SELECT_OPTION], [MODULE], [PROMOTION_ID]) ")
            sqlStatement.Append("VALUES ")
            sqlStatement.Append("(@BasketHeaderID, @BasketDetailID, '', '', 0, '").Append(GlobalConstants.BASKETMODULETICKETING).Append("', @PromotionId)")
            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement.ToString()
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BasketHeaderID", basketHeaderId))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BasketDetailID", basketDetailId))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PromotionId", promotionId))

            'Execute
            If (givenTransaction Is Nothing) Then
                err = talentSqlAccessDetail.SQLAccess()
            Else
                err = talentSqlAccessDetail.SQLAccess(givenTransaction)
            End If
            If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                affectedRows = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
            Else
                affectedRows = -1
            End If
            talentSqlAccessDetail = Nothing

            'Return results
            Return affectedRows
        End Function

        ''' <summary>
        ''' Check to see if the current basket has promotion items records
        ''' </summary>
        ''' <param name="basketHeaderId">The basket header id</param>
        ''' <param name="module_">The basket module</param>
        ''' <returns><c>True</c> if the basket has promotions, otherwise <c>False</c></returns>
        ''' <remarks></remarks>
        Public Function DoesBasketHeaderIdHavePromotions(ByVal basketHeaderId As String, ByVal module_ As String) As Boolean
            Dim basketHasPromotions As Boolean = False
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetByBasketDetailIDAndModule")
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = False
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandText = "SELECT COUNT(*) FROM [tbl_basket_promotion_items] WHERE BASKET_HEADER_ID = @BasketHeaderID AND MODULE = @Module"
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BasketHeaderID", basketHeaderId))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Module", module_))

                'Execute
                Dim err As New ErrorObj
                err = talentSqlAccessDetail.SQLAccess()
                If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                    If Utilities.CheckForDBNull_Int(talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)) > 0 Then
                        basketHasPromotions = True
                    End If
                End If
            Catch ex As Exception
                Throw
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            'Return the results 
            Return basketHasPromotions
        End Function
#End Region

    End Class
End Namespace
