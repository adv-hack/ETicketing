Imports System.Data.SqlClient
Imports System.Text

Namespace DataObjects.TableObjects
    ''' <summary>
    ''' Provides the functionality to manage data from the table tbl_creditcard_type_control based on business functionality
    ''' </summary>
    <Serializable()> _
    Public Class tbl_creditcard_type_control
        Inherits DBObjectBase

#Region "Class Level Fields"
        ''' <summary>
        ''' Instance of DESettings
        ''' </summary>
        Private _settings As New DESettings

        ''' <summary>
        ''' Class Name which is used in cache key construction
        ''' </summary>
        Const CACHEKEY_CLASSNAME As String = "tbl_creditcard_type_control"
#End Region

#Region "Constructors"
        Sub New()
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="tbl_creditcard_type_control" /> class.
        ''' </summary>
        ''' <param name="settings">The DESettings instance</param>
        Sub New(ByVal settings As DESettings)
            _settings = settings
        End Sub
#End Region

#Region "Public Methods"

        ''' <summary>
        ''' Returns a list of installments, if only 1 installment is given this is the maximum number of installments and the user may enter anything upto that max
        ''' </summary>
        ''' <param name="cardNumber">The first 6 digits of the card number</param>
        ''' <param name="cacheing">An optional boolean value to represent caching, default true</param>
        ''' <param name="cacheTimeMinutes">An optional cache time value, default 30 minss</param>
        ''' <returns>List of installments</returns>
        ''' <remarks></remarks>
        Public Function GetInstallmentsByCard(ByVal cardNumber As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As List(Of Integer)
            Dim installments As New List(Of Integer)
            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetInstallmentsByCard")
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj

            Try
                If cardNumber.Length > 6 Then cardNumber = cardNumber.Substring(0, 6)
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@CardNumber", cardNumber))

                Dim sqlStatement As New StringBuilder
                sqlStatement.Append("SELECT CARD_CODE, MAX_INSTALLMENTS, INSTALLMENTS_LIST FROM [tbl_creditcard_type_control] ")
                sqlStatement.Append("WHERE @CardNumber >= CARD_FROM_RANGE ")
                sqlStatement.Append("AND @CardNumber <= CARD_TO_RANGE ")
                sqlStatement.Append("ORDER BY CARD_FROM_RANGE DESC")

                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix & cardNumber
                talentSqlAccessDetail.CommandElements.CommandText = sqlStatement.ToString()
                err = talentSqlAccessDetail.SQLAccess()
                If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                    If talentSqlAccessDetail.ResultDataSet.Tables(0).Rows.Count > 0 Then
                        outputDataTable = talentSqlAccessDetail.ResultDataSet.Tables(0)
                        installments = getInstallmentsValue(outputDataTable)
                    End If
                End If
            Catch ex As Exception
                Throw
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            Return installments
        End Function

#End Region

#Region "Private Functions"

        ''' <summary>
        ''' Get the installments value list by the datatable of results
        ''' </summary>
        ''' <param name="outputDataTable">The data table of results</param>
        ''' <returns>Installments list array</returns>
        ''' <remarks></remarks>
        Private Function getInstallmentsValue(ByRef outputDataTable As DataTable) As List(Of Integer)
            Dim installments As New List(Of Integer)
            Dim maxInstallments As String = Utilities.CheckForDBNull_String(outputDataTable.Rows(0)("MAX_INSTALLMENTS"))
            Dim installmentsList As String = Utilities.CheckForDBNull_String(outputDataTable.Rows(0)("INSTALLMENTS_LIST"))
            If String.IsNullOrEmpty(installmentsList) Then
                If Not String.IsNullOrEmpty(maxInstallments) Then
                    Dim maxInstallmentsInt As Integer = 0
                    If Integer.TryParse(maxInstallments, maxInstallmentsInt) Then installments.Add(maxInstallmentsInt)
                End If
            Else
                Dim tempArray() As String = installmentsList.Split(",")
                For Each item As String In tempArray
                    Dim intItem As Integer = 0
                    If Integer.TryParse(item, intItem) Then installments.Add(intItem)
                Next
            End If
            Return installments
        End Function

#End Region

    End Class
End Namespace