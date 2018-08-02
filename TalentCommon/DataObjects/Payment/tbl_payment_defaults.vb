Imports System.Data.SqlClient
Namespace DataObjects.TableObjects
    ''' <summary>
    ''' Provides the functionality to manage data from the table tbl_payment_defaults
    ''' </summary>
    <Serializable()> _
    Public Class tbl_payment_defaults
        Inherits DBObjectBase

#Region "Public Properties"

        Public Property SuccessCodes As String
        Public Property PaymentDetails1 As String
        Public Property PaymentDetails2 As String
        Public Property PaymentDetails3 As String
        Public Property PaymentDetails4 As String
        Public Property PaymentDetails5 As String
        Public Property PaymentDetails6 As String
        Public Property PaymentDetails7 As String
        Public Property PaymentDetails8 As String
        Public Property PaymentDetails9 As String
        Public Property PaymentDetails10 As String
        Public Property PaymentUrl1 As String
        Public Property PaymentUrl2 As String
        Public Property OurAccountID As String
        Public Property OurSystemID As String
        Public Property OurPasscode As String
        Public Property OurSystemGUID As String
        Public Property IFrameWidth As String

#End Region

#Region "Class Level Fields"

        ''' <summary>
        ''' Instance of DESettings
        ''' </summary>
        Private _settings As New DESettings

        ''' <summary>
        ''' Class Name which is used in cache key construction
        ''' </summary>
        Const CACHEKEY_CLASSNAME As String = "tbl_payment_defaults"
#End Region

#Region "Constructors"
        Sub New()
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="tbl_payment_defaults" /> class.
        ''' </summary>
        ''' <param name="settings">The DESettings instance</param>
        Sub New(ByVal settings As DESettings)
            _settings = settings
        End Sub
#End Region

#Region "Private Methods"

        ''' <summary>
        ''' Populate the public properties based on the data table provided
        ''' </summary>
        ''' <param name="outputDataTable">results from tbl_payment_defaults query</param>
        ''' <remarks></remarks>
        Private Sub populatePublicProperties(ByVal outputDataTable As DataTable)
            For Each row As DataRow In outputDataTable.Rows
                Select Case row("DEFAULT_NAME").ToString
                    Case Is = "PAYMENT_3DSECURE_SUCCESS_CODES"
                        SuccessCodes = Utilities.CheckForDBNull_String(row("VALUE"))
                    Case Is = "PAYMENT_3DSECURE_DETAILS_1"
                        PaymentDetails1 = Utilities.CheckForDBNull_String(row("VALUE"))
                    Case Is = "PAYMENT_3DSECURE_DETAILS_2"
                        PaymentDetails2 = Utilities.CheckForDBNull_String(row("VALUE"))
                    Case Is = "PAYMENT_3DSECURE_DETAILS_3"
                        PaymentDetails3 = Utilities.CheckForDBNull_String(row("VALUE"))
                    Case Is = "PAYMENT_3DSECURE_DETAILS_4"
                        PaymentDetails4 = Utilities.CheckForDBNull_String(row("VALUE"))
                    Case Is = "PAYMENT_3DSECURE_DETAILS_5"
                        PaymentDetails5 = Utilities.CheckForDBNull_String(row("VALUE"))
                    Case Is = "PAYMENT_3DSECURE_DETAILS_6"
                        PaymentDetails6 = Utilities.CheckForDBNull_String(row("VALUE"))
                    Case Is = "PAYMENT_3DSECURE_DETAILS_7"
                        PaymentDetails7 = Utilities.CheckForDBNull_String(row("VALUE"))
                    Case Is = "PAYMENT_3DSECURE_DETAILS_8"
                        PaymentDetails8 = Utilities.CheckForDBNull_String(row("VALUE"))
                    Case Is = "PAYMENT_3DSECURE_DETAILS_9"
                        PaymentDetails9 = Utilities.CheckForDBNull_String(row("VALUE"))
                    Case Is = "PAYMENT_3DSECURE_DETAILS_10"
                        PaymentDetails10 = Utilities.CheckForDBNull_String(row("VALUE"))
                    Case Is = "PAYMENT_3DSECURE_URL_1"
                        PaymentUrl1 = Utilities.CheckForDBNull_String(row("VALUE"))
                    Case Is = "PAYMENT_3DSECURE_URL_2"
                        PaymentUrl2 = Utilities.CheckForDBNull_String(row("VALUE"))
                    Case Is = "PAYMENT_3DSECURE_OUR_ACCOUNT_ID"
                        OurAccountID = Utilities.CheckForDBNull_String(row("VALUE"))
                    Case Is = "PAYMENT_3DSECURE_OUR_SYSTEM_ID"
                        OurSystemID = Utilities.CheckForDBNull_String(row("VALUE"))
                    Case Is = "PAYMENT_3DSECURE_OUR_PASSCODE"
                        OurPasscode = Utilities.CheckForDBNull_String(row("VALUE"))
                    Case Is = "PAYMENT_3DSECURE_OUR_SYSTEM_GUID"
                        OurSystemGUID = Utilities.CheckForDBNull_String(row("VALUE"))
                    Case Is = "PAYMENT_3DSECURE_IFRAMEWIDTH"
                        IFrameWidth = Utilities.CheckForDBNull_String(row("VALUE"))
                End Select
            Next
        End Sub

#End Region

#Region "Public Methods"
        ''' <summary>
        ''' Gets the payment defaults based on the given account id
        ''' </summary>
        ''' <param name="accountId">The account id</param>
        ''' <param name="cacheing">if set to <c>true</c> [cacheing]. Default <c>true</c></param>
        ''' <param name="cacheTimeMinutes">The cache time minutes. Default <c>30</c></param>
        Sub GetPaymentDefaults(ByVal accountId As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30)
            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetPaymentDefaults")
            Dim talentSqlAccessDetail As New TalentDataAccess

            'Permutation and Combinations Select statement
            'BusinessUnit   Partner
            'Given          Given
            'Given          *ALL
            '*ALL           *ALL

            Dim whereClauseFetchHierarchy(2) As String
            Dim cacheKeyHierarchyBased(2) As String

            whereClauseFetchHierarchy(0) = "BUSINESS_UNIT=@BusinessUnit AND PARTNER=@Partner"
            cacheKeyHierarchyBased(0) = ToUpper(_settings.BusinessUnit) & ToUpper(_settings.Partner)

            whereClauseFetchHierarchy(1) = "BUSINESS_UNIT=@BusinessUnit AND PARTNER='" & ReplaceSingleQuote(Utilities.GetAllString) & "'"
            cacheKeyHierarchyBased(1) = ToUpper(_settings.BusinessUnit) & ToUpper(Utilities.GetAllString)

            whereClauseFetchHierarchy(2) = "BUSINESS_UNIT='" & ReplaceSingleQuote(Utilities.GetAllString) & "' AND PARTNER='" & ReplaceSingleQuote(Utilities.GetAllString) & "'"
            cacheKeyHierarchyBased(2) = ToUpper(Utilities.GetAllString) & ToUpper(Utilities.GetAllString)

            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", _settings.BusinessUnit))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Partner", _settings.Partner))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@AccountID", accountId))

                Dim sqlStatement As String = "SELECT * FROM [tbl_payment_defaults] WHERE [ACCOUNT_ID] = @AccountID AND "
                Dim err As New ErrorObj

                For whereClauseFetchHierarchyCounter As Integer = 0 To 2 Step 1
                    talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix & cacheKeyHierarchyBased(whereClauseFetchHierarchyCounter)
                    talentSqlAccessDetail.CommandElements.CommandText = sqlStatement & whereClauseFetchHierarchy(whereClauseFetchHierarchyCounter)
                    err = talentSqlAccessDetail.SQLAccess()
                    If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                        outputDataTable = talentSqlAccessDetail.ResultDataSet.Tables(0)
                        If outputDataTable.Rows.Count > 0 Then
                            populatePublicProperties(outputDataTable)
                            Exit For
                        End If
                    Else
                        Exit For
                    End If
                Next
            Catch ex As Exception
                Throw
            Finally
                talentSqlAccessDetail = Nothing
            End Try

        End Sub
#End Region

    End Class
End Namespace
