Imports System.Data.SqlClient
Namespace DataObjects.TableObjects
    ''' <summary>
    ''' Provides the functionality to manage data from the table tbl_group_product
    ''' </summary>
    <Serializable()> _
    Public Class tbl_group_product
        Inherits DBObjectBase

#Region "Class Level Fields"

        ''' <summary>
        ''' Instance of DESettings
        ''' </summary>
        Private _settings As New DESettings

        ''' <summary>
        ''' Class Name which is used in cache key construction
        ''' </summary>
        Const CACHEKEY_CLASSNAME As String = "tbl_group_product"

#End Region

#Region "Constructors"
        Sub New()
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="tbl_group_product" /> class.
        ''' </summary>
        ''' <param name="settings">The DESettings instance</param>
        Sub New(ByVal settings As DESettings)
            _settings = settings
        End Sub
#End Region

#Region "Public Methods"

        ''' <summary>
        ''' Get the group product sequence value of a product in the group based on the given business unit, partner and group product ID
        ''' </summary>
        ''' <param name="businessUnit">The given business unit</param>
        ''' <param name="partner">The given partner code</param>
        ''' <param name="groupProductId">The given group product ID</param>
        ''' <returns>The sequence value</returns>
        ''' <remarks></remarks>
        Public Function GetSequenceValueByGroupProductId(ByVal businessUnit As String, ByVal partner As String, ByVal groupProductId As String) As String
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim sequenceValue As String = String.Empty

            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheStringExtension = String.Empty
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet

            talentSqlAccessDetail.CommandElements.CommandText = "SELECT [SEQUENCE] FROM [tbl_group_product] WHERE [GROUP_BUSINESS_UNIT] = @BusinessUnit AND [GROUP_PARTNER] = @Partner AND [GROUP_PRODUCT_ID] = @GroupProductId"
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", businessUnit))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Partner", partner))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@GroupProductId", groupProductId))

            'Execute
            Dim err As New ErrorObj
            err = talentSqlAccessDetail.SQLAccess()
            If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                sequenceValue = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
            End If
            talentSqlAccessDetail = Nothing

            'Return results
            Return sequenceValue
        End Function

        ''' <summary>
        ''' Update the product sequence in the group based on the group product ID
        ''' </summary>
        ''' <param name="businessUnit">The given business unit of the record being updated</param>
        ''' <param name="partner">The given partner of the record being updated</param>
        ''' <param name="groupProductId">The given group product ID of the record being updated</param>
        ''' <param name="sequenceValue">The sequence value to set</param>
        ''' <returns>The number of affected records</returns>
        ''' <remarks></remarks>
        Public Function UpdateSequenceByGroupProductId(ByVal businessUnit As String, ByVal partner As String, ByVal groupProductId As String, ByVal sequenceValue As String) As Integer
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim affectedRows As Integer = 0

            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheStringExtension = String.Empty
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery

            talentSqlAccessDetail.CommandElements.CommandText = "UPDATE [tbl_group_product] SET [SEQUENCE] = @Sequence WHERE [GROUP_BUSINESS_UNIT] = @BusinessUnit AND [GROUP_PARTNER] = @Partner AND [GROUP_PRODUCT_ID] = @GroupProductId"
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", businessUnit))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Partner", partner))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@GroupProductId", groupProductId))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Sequence", sequenceValue))

            'Execute
            Dim err As New ErrorObj
            err = talentSqlAccessDetail.SQLAccess()
            If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                affectedRows = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
            End If
            talentSqlAccessDetail = Nothing

            'Return results
            Return affectedRows
        End Function

#End Region

    End Class
End Namespace


