Imports System.Data.SqlClient
Imports System.Text
Imports System.Transactions
Imports Talent.Common.DataObjects.TableObjects
Namespace DataObjects

    '   Error Code  -   TACTKDES- (TAC -Talent Common, TKDES - class name TicketDesigner)
    '   Next Error Code Starting is TACTKDES-08

    ''' <summary>
    ''' Class provides the functionality to access and manage Ticket Designer related tables
    ''' </summary>
    <Serializable()> _
        Public Class TicketDesigner
        Inherits DBObjectBase

#Region "Class Level Fields"
        ''' <summary>
        ''' DESettings Instance
        ''' </summary>
        Private _settings As New DESettings
        Private _ticketDesignerDataEntity As DETicketDesigner
        Private _tblTDLabelProperties As tbl_td_labelproperties
        Private _tblTDFieldProperties As tbl_td_fieldproperties
        Private _tblTDFontSimulate As tbl_td_fontsimulate
        Private _tblTDFontsLaser As tbl_td_fontslaser
        Private _tblTDPDFFontProperties As tbl_td_pdffontproperties
        Private _tblTDFormattingRules As tbl_td_formattingrules

        ''' <summary>
        ''' Class Name which is used in cache key construction
        ''' </summary>
        Const CACHEKEY_CLASSNAME As String = "TicketDesigner"

        'Used for logging
        Private Const SOURCEAPPLICATION As String = "SUPPLYNET"
        Private Const SOURCECLASS As String = "TICKETDESIGNER"

#End Region

#Region "Constructors"
        Sub New()
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="TicketDesigner" /> class.
        ''' </summary>
        ''' <param name="settings">DESettings Instance</param>
        Sub New(ByVal settings As DESettings)
            _settings = settings
        End Sub
#End Region

#Region "Properties"

        ''' <summary>
        ''' Gets or sets the ticket desinger data entity.
        ''' </summary>
        ''' <value>The ticket desinger data entity.</value>
        Public Property TicketDesignerDataEntity() As DETicketDesigner
            Get
                Return _ticketDesignerDataEntity
            End Get
            Set(ByVal value As DETicketDesigner)
                _ticketDesignerDataEntity = value
            End Set
        End Property

        ''' <summary>
        ''' Create and Gets the tbl_td_labelproperties instance with DESettings
        ''' </summary>
        ''' <value>tbl_td_labelproperties instance</value>
        Public ReadOnly Property TblTDLabelProperties() As tbl_td_labelproperties
            Get
                If (_tblTDLabelProperties Is Nothing) Then
                    _tblTDLabelProperties = New tbl_td_labelproperties(_settings)
                End If
                Return _tblTDLabelProperties
            End Get
        End Property

        ''' <summary>
        ''' Create and Gets the tbl_td_fieldproperties instance with DESettings
        ''' </summary>
        ''' <value>tbl_td_fieldproperties instance</value>
        Public ReadOnly Property TblTDFieldProperties() As tbl_td_fieldproperties
            Get
                If (_tblTDFieldProperties Is Nothing) Then
                    _tblTDFieldProperties = New tbl_td_fieldproperties(_settings)
                End If
                Return _tblTDFieldProperties
            End Get
        End Property

        ''' <summary>
        ''' Create and Gets the tbl_td_fontsimulate instance with DESettings
        ''' </summary>
        ''' <value>tbl_td_fontsimulate instance</value>
        Public ReadOnly Property TblTDFontSimulate() As tbl_td_fontsimulate
            Get
                If (_tblTDFontSimulate Is Nothing) Then
                    _tblTDFontSimulate = New tbl_td_fontsimulate(_settings)
                End If
                Return _tblTDFontSimulate
            End Get
        End Property

        ''' <summary>
        ''' Create and Gets the tbl_td_fontslaser instance with DESettings
        ''' </summary>
        ''' <value>tbl_td_fontslaser instance</value>
        Public ReadOnly Property TblTDFontsLaser() As tbl_td_fontslaser
            Get
                If (_tblTDFontsLaser Is Nothing) Then
                    _tblTDFontsLaser = New tbl_td_fontslaser(_settings)
                End If
                Return _tblTDFontsLaser
            End Get
        End Property

        ''' <summary>
        ''' Create and Gets the tbl_td_pdffontproperties instance with DESettings
        ''' </summary>
        ''' <value>tbl_td_pdffontproperties instance</value>
        Public ReadOnly Property TblTDPDFFontProperties() As tbl_td_pdffontproperties
            Get
                If (_tblTDPDFFontProperties Is Nothing) Then
                    _tblTDPDFFontProperties = New tbl_td_pdffontproperties(_settings)
                End If
                Return _tblTDPDFFontProperties
            End Get
        End Property

        ''' <summary>
        ''' Create and Gets the tbl_td_formattingrules instance with DESettings
        ''' </summary>
        ''' <value>tbl_td_formattingrules instance</value>
        Public ReadOnly Property TblTDFormattingRules() As tbl_td_formattingrules
            Get
                If (_tblTDFormattingRules Is Nothing) Then
                    _tblTDFormattingRules = New tbl_td_formattingrules(_settings)
                End If
                Return _tblTDFormattingRules
            End Get
        End Property

#End Region

#Region "Public Methods"

        ''' <summary>
        ''' Uploads the Ticket Designer Data to the given databases
        ''' </summary>
        ''' <param name="connectionStrings">The connection strings.</param>
        ''' <param name="connectionStringCount">The connection string count.</param>
        ''' <param name="cacheing">if set to <c>true</c> [cacheing].</param>
        ''' <param name="cacheTimeMinutes">The cache time minutes.</param>
        ''' <returns></returns>
        Public Function UploadData(ByVal connectionStrings() As String, ByVal connectionStringCount As Integer, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As ErrorObj

            'variable initialisations
            Dim currentConnectionString As String = String.Empty
            Dim loggingConnectionString As String = String.Empty
            Dim labelPropertiesOpenXmlString As String = String.Empty
            Dim fieldPropertiesOpenXmlString As String = String.Empty
            Dim fontSimulateOpenXmlString As String = String.Empty
            Dim hasTransactionError As Boolean = True
            Dim fontsLaserOpenXmlString As String = String.Empty
            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "UploadData")
            Dim errObj As New ErrorObj
            Dim talentSqlAccessDetail As TalentDataAccess = Nothing

            'Logging the transaction starts
            Dim logHeaderId As Integer = 0
            Dim SOURCEMETHOD As String = "UPLOADDATA"
            Dim additionalDetails As String = "Upload Data transaction started" & _
                    " ConnectionStringCount : " & connectionStringCount
            Dim loggingSettings As New Logging(_settings)
            loggingConnectionString = _settings.FrontEndConnectionString
            Dim affectedRows As Integer = loggingSettings.TblLogHeader.Create(SOURCEAPPLICATION, SOURCECLASS, SOURCEMETHOD, ActivityStatusEnum.INPROGRESS, additionalDetails, logHeaderId)
            'Logging ends

            If (logHeaderId > 0) Then

                'before start transaction make ready all the open xml sql statement
                Try
                    'Converting list of DELabelProperties to OpenXml SQL String
                    errObj = TblTDLabelProperties.GenericListToSQLOpenXmlString(TicketDesignerDataEntity.LabelProperties)
                    If (Not errObj.HasError) Then
                        labelPropertiesOpenXmlString = TblTDLabelProperties.GenericListToXml
                        'Converting list of DEFieldProperties to OpenXml SQL String
                        errObj = TblTDFieldProperties.GenericListToSQLOpenXmlString(TicketDesignerDataEntity.FieldProperties)
                        If (Not errObj.HasError) Then
                            fieldPropertiesOpenXmlString = TblTDFieldProperties.GenericListToXml
                            'Converting list of DEFontsLaser to OpenXml SQL String
                            errObj = TblTDFontsLaser.GenericListToSQLOpenXmlString(TicketDesignerDataEntity.FontsLaser)
                            If (Not errObj.HasError) Then
                                fontsLaserOpenXmlString = TblTDFontsLaser.GenericListToXml
                                'Converting list of DEFontSimulate to OpenXml SQL String
                                errObj = TblTDFontSimulate.GenericListToSQLOpenXmlString(TicketDesignerDataEntity.FontSimulate)
                                If (Not errObj.HasError) Then
                                    fontSimulateOpenXmlString = TblTDFontSimulate.GenericListToXml
                                End If
                            End If
                        End If
                    End If

                    If (Not errObj.HasError) Then
                        'initialising TalentDataAccess Object
                        'this below settings are common for all execution under transaction for TalentDataAccess
                        cacheing = False
                        talentSqlAccessDetail = New TalentDataAccess
                        talentSqlAccessDetail.Settings = _settings
                        talentSqlAccessDetail.Settings.Cacheing = cacheing
                        talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                        talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
                        talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
                    End If

                Catch ex As Exception
                    errObj.HasError = True
                    errObj.ErrorMessage = ex.Message
                    errObj.ErrorNumber = "TACTKDES-02"
                End Try

                'Have You Finished all kinds validation and initialise any common settings
                'make sure everything ready so that under transaction just open and execute
                'transaction starts
                If (Not errObj.HasError) Then
                    Using scopeObject As TransactionScope = New TransactionScope()
                        Try
                            For connStringIndex As Integer = 0 To connectionStringCount - 1
                                hasTransactionError = True
                                'Assign the current connection string
                                currentConnectionString = connectionStrings(connStringIndex)
                                talentSqlAccessDetail.Settings.FrontEndConnectionString = currentConnectionString

                                'Label Properties Delete and Insert
                                talentSqlAccessDetail.CommandElements.CommandText = labelPropertiesOpenXmlString
                                errObj = talentSqlAccessDetail.SQLAccess()

                                If (Not (errObj.HasError)) Then
                                    hasTransactionError = False
                                Else
                                    hasTransactionError = True
                                    errObj.ErrorNumber = "TACTKDES-03"
                                    Exit For
                                End If

                                'Field Properties Delete and Insert
                                talentSqlAccessDetail.CommandElements.CommandText = fieldPropertiesOpenXmlString
                                errObj = talentSqlAccessDetail.SQLAccess()
                                If (Not (errObj.HasError)) Then
                                    hasTransactionError = False
                                Else
                                    hasTransactionError = True
                                    errObj.ErrorNumber = "TACTKDES-04"
                                    Exit For
                                End If

                                'Fonts Laser Truncate and Insert
                                talentSqlAccessDetail.CommandElements.CommandText = fontsLaserOpenXmlString
                                errObj = talentSqlAccessDetail.SQLAccess()
                                If (Not (errObj.HasError)) Then
                                    hasTransactionError = False
                                Else
                                    hasTransactionError = True
                                    errObj.ErrorNumber = "TACTKDES-05"
                                    Exit For
                                End If

                                'FontSimulate Truncate and Insert
                                talentSqlAccessDetail.CommandElements.CommandText = fontSimulateOpenXmlString
                                errObj = talentSqlAccessDetail.SQLAccess()
                                If (Not (errObj.HasError)) Then
                                    hasTransactionError = False
                                Else
                                    hasTransactionError = True
                                    errObj.ErrorNumber = "TACTKDES-06"
                                    Exit For
                                End If

                            Next

                            'transaction to commit or abort
                            If (Not hasTransactionError) Then
                                scopeObject.Complete()
                            Else
                                'There is error in the transaction so no commit it automatically abort
                            End If
                        Catch ex As Exception
                            errObj.HasError = True
                            errObj.ErrorMessage = ex.Message
                            errObj.ErrorNumber = "TACTKDES-07"
                        Finally
                            'Todo: any resources to dispose
                        End Try
                    End Using
                End If
            Else
                errObj.HasError = True
                errObj.ErrorMessage = "Failed to create log header ID"
                errObj.ErrorNumber = "TACTKDES-01"
            End If
            talentSqlAccessDetail = Nothing

            'Logging the transaction ends or error
            _settings.FrontEndConnectionString = loggingConnectionString
            Dim lableNames As String = String.Empty
            For Each labelProperty As DELabelProperties In TicketDesignerDataEntity.LabelProperties
                lableNames += labelProperty.LabellName + ","
            Next
            lableNames = lableNames.Trim(",")
            If (Not errObj.HasError) Then
                affectedRows = loggingSettings.TblLogHeader.Update(logHeaderId, ActivityStatusEnum.SUCCESS, " Successfully Transfered Ticket Designer Data ")
                affectedRows = loggingSettings.TblLogs.WriteLog(logHeaderId, SOURCECLASS, SOURCEMETHOD, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, "Transfered Label Names : " & lableNames)
            Else
                affectedRows = loggingSettings.TblLogHeader.Update(logHeaderId, ActivityStatusEnum.FAILED, " Failed while transfering data ")
                Dim logContent As String = " Error Connection String : " & currentConnectionString & _
                                            " Error Message : " & errObj.ErrorMessage & _
                                            " Label Names :  " & lableNames
                affectedRows = loggingSettings.TblLogs.WriteLog(logHeaderId, SOURCECLASS, SOURCEMETHOD, errObj.ErrorNumber, Nothing, Nothing, Nothing, Nothing, Nothing, logContent)
            End If
            'Logging ends

            Return errObj
        End Function
#End Region
    End Class
End Namespace
