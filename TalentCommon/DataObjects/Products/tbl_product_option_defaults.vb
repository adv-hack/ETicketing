Imports System.Data.SqlClient
Namespace DataObjects.TableObjects
    ''' <summary>
    ''' Provides the functionality to manage data from the table tbl_product_option_defaults
    ''' </summary>
    <Serializable()> _
    Public Class tbl_product_option_defaults
        Inherits DBObjectBase

#Region "Class Level Fields"

        ''' <summary>
        ''' Instance of DESettings
        ''' </summary>
        Private _settings As New DESettings

        ''' <summary>
        ''' Class Name which is used in cache key construction
        ''' </summary>
        Const CACHEKEY_CLASSNAME As String = "tbl_product_option_defaults"
        ''' <summary>
        ''' Gets or sets the product option default entity.
        ''' </summary>
        ''' <value>
        ''' The product option default entity.
        ''' </value>
        Public Property ProductOptionDefaultEntity() As DEProductOptionDefault

#End Region

#Region "Constructors"
        Sub New()
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="tbl_product_option_defaults" /> class.
        ''' </summary>
        ''' <param name="settings">The DESettings instance</param>
        Sub New(ByVal settings As DESettings)
            _settings = settings
        End Sub
#End Region

#Region "Public Methods"

        Public Function Insert(ByVal businessUnit As String, ByVal partner As String, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer

            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess

            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheStringExtension = ""
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            Dim sqlStatement As String = "INSERT tbl_product_option_defaults " & _
                                    "(BUSINESS_UNIT,PARTNER, MASTER_PRODUCT, OPTION_TYPE, MATCH_ACTION, " & _
                                    "IS_DEFAULT, APPEND_SEQUENCE, DISPLAY_SEQUENCE, DISPLAY_TYPE) VALUES " & _
                                    "(@BusinessUnit, @Partner, @MasterProduct, @OptionType, @MatchAction,  " & _
                                    "@IsDefault, @AppendSequence, @DisplaySequence, @DisplayType) "

            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", businessUnit))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Partner", partner))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@MasterProduct", ProductOptionDefaultEntity.MasterProduct))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@OptionType", ProductOptionDefaultEntity.OptionType))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@MatchAction", ProductOptionDefaultEntity.MatchAction))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@IsDefault", ProductOptionDefaultEntity.IsDefault))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@AppendSequence", ProductOptionDefaultEntity.AppendSequence))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DisplaySequence", ProductOptionDefaultEntity.DisplaySequence))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DisplayType", ProductOptionDefaultEntity.DisplayType))

            'Execute
            Dim err As New ErrorObj
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
        ''' Retrieves options defaults by master product
        ''' </summary>
        ''' <param name="businessUnit"></param>
        ''' <param name="masterProduct"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetProductOptionDefaults(ByVal businessUnit As String, ByVal masterProduct As String) As DataTable
            Dim outputDataTable As New DataTable
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandText = "SELECT * FROM tbl_product_option_defaults WHERE PRODUCT_OPTION_MASTER = @ProductOptionMaster AND BUSINESS_UNIT=@BusinessUnit"
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ProductOptionMaster", masterProduct))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", businessUnit))

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
        ''' Removes options defaults by master product
        ''' </summary>
        ''' <param name="masterProductCode"></param>
        ''' <param name="givenTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function RemoveByMasterProduct(ByVal masterProductCode As String, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer
            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = False

                'Execute
                Dim err As New ErrorObj
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
                talentSqlAccessDetail.CommandElements.CommandText = "DELETE FROM tbl_product_option_defaults Where MASTER_PRODUCT=@MasterProductCode"

                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@MasterProductCode", masterProductCode))
                If (givenTransaction Is Nothing) Then
                    err = talentSqlAccessDetail.SQLAccess()
                Else
                    err = talentSqlAccessDetail.SQLAccess(givenTransaction)
                End If
                If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                    affectedRows = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
                Else
                    affectedRows = 0
                End If
            Catch ex As Exception
                Throw
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            'Return the results 
            Return affectedRows
        End Function
#End Region

    End Class
End Namespace


