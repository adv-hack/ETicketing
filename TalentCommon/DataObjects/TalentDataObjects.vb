Imports System.Data.SqlClient
Imports Talent.Common.Dataobjects

''' <summary>
''' Creates and provides the instances to access the data objects
''' </summary>
<Serializable()> _
Public Class TalentDataObjects
    Inherits DBObjectBase

#Region "Class Level Fields"
    ''' <summary>
    ''' DESettings instance
    ''' </summary>
    Private _settings As New DESettings

    Private _activities As Activities
    Private _agent As Agent
    Private _alerts As Alerts
    Private _appVariables As ApplicationVariables
    Private _basket As Basket
    Private _clubs As Clubs
    Private _controls As Controls
    Private _delivery As Delivery
    Private _emailTemplates As EmailTemplates
    Private _feeds As Feeds
    Private _fees As Fees
    Private _flash As Flash
    Private _logging As Logging
    Private _maintenance As Maintenance
    Private _order As Order
    Private _pages As Pages
    Private _payment As Payment
    Private _products As Products
    Private _profile As Profile
    Private _promotions As Promotions
    Private _serialized As Serialized
    Private _sports As Sports
    Private _stadiums As Stadiums
    Private _talentAdmin As TalentAdmin
    Private _talentDefinitions As TalentDefinitions
    Private _talentDatabase As TalentDatabase
    Private _templates As Templates
    Private _ticketDesigner As TicketDesigner
    Private _tracking As Tracking
    Private _version As Version


    'Used for logging
    Private Const SOURCEAPPLICATION As String = "MAINTENANCE"
    Private Const SOURCECLASS As String = "TALENTDATAOBJECTS"

#End Region

#Region "Properties"
    ''' <summary>
    ''' Gets or sets the settings value from DESettings
    ''' </summary>
    ''' <value>The settings.</value>
    Public Property Settings() As DESettings
        Get
            Return _settings
        End Get
        Set(ByVal value As DESettings)
            _settings = value
        End Set
    End Property

    ''' <summary>
    ''' Create and Gets the Logging instance with DESettings
    ''' </summary>
    ''' <value>The applicationvariables instance.</value>
    Public ReadOnly Property LoggingSettings() As Logging
        Get
            If (_logging Is Nothing) Then
                _logging = New Logging(_settings)
            End If
            Return _logging
        End Get
    End Property

    ''' <summary>
    ''' Create and Gets the Activities instance with DESettings
    ''' </summary>
    ''' <value>The Activities instance.</value>
    Public ReadOnly Property ActivitiesSettings() As Activities
        Get
            If (_activities Is Nothing) Then
                _activities = New Activities(_settings)
            End If
            Return _activities
        End Get
    End Property

    ''' <summary>
    ''' Create and Gets the Basket instance with DESettings
    ''' </summary>
    ''' <value>The Basket instance.</value>
    Public ReadOnly Property BasketSettings() As Basket
        Get
            If (_basket Is Nothing) Then
                _basket = New Basket(_settings)
            End If
            Return _basket
        End Get
    End Property

    ''' <summary>
    ''' Create and Gets the Alerts instance with DESettings
    ''' </summary>
    ''' <value>The Alerts instance.</value>
    Public ReadOnly Property AlertSettings() As Alerts
        Get
            If (_alerts Is Nothing) Then
                _alerts = New Alerts(_settings)
            End If
            Return _alerts
        End Get
    End Property

    ''' <summary>
    ''' Create and Gets the Email instance with DESettings
    ''' </summary>
    ''' <value>The Alerts instance.</value>
    Public ReadOnly Property EmailTemplateSettings() As EmailTemplates
        Get
            If (_emailTemplates Is Nothing) Then
                _emailTemplates = New EmailTemplates(_settings)
            End If
            Return _emailTemplates
        End Get
    End Property

    ''' <summary>
    ''' Create and Gets the ApplicationVariables instance with DESettings
    ''' </summary>
    ''' <value>The applicationvariables instance.</value>
    Public ReadOnly Property AppVariableSettings() As ApplicationVariables
        Get
            If (_appVariables Is Nothing) Then
                _appVariables = New ApplicationVariables(_settings)
            End If
            Return _appVariables
        End Get
    End Property

    ''' <summary>
    ''' Create and Gets the Templates instance with DESettings
    ''' </summary>
    ''' <value>The templates instance.</value>
    Public ReadOnly Property TemplateSettings() As Templates
        Get
            If (_templates Is Nothing) Then
                _templates = New Templates(_settings)
            End If
            Return _templates
        End Get
    End Property

    ''' <summary>
    ''' Create and Gets the Flash instance with DESettings
    ''' </summary>
    ''' <value>The flash instance.</value>
    Public ReadOnly Property FlashSettings() As Flash
        Get
            If (_flash Is Nothing) Then
                _flash = New Flash(_settings)
            End If
            Return _flash
        End Get
    End Property

    ''' <summary>
    ''' Create and Gets the Version Deployed instance with DESettings
    ''' </summary>
    ''' <value>The flash instance.</value>
    Public ReadOnly Property VersionDeployed() As Version
        Get
            If (_version Is Nothing) Then
                _version = New Version(_settings)
            End If
            Return _version
        End Get
    End Property

    ''' <summary>
    ''' Create and Gets the Controls instance with DESettings
    ''' </summary>
    ''' <value>The flash instance.</value>
    Public ReadOnly Property ControlSettings() As Controls
        Get
            If (_controls Is Nothing) Then
                _controls = New Controls(_settings)
            End If
            Return _controls
        End Get
    End Property

    ''' <summary>
    ''' Create and Gets the Controls instance with DESettings
    ''' </summary>
    ''' <value>The flash instance.</value>
    Public ReadOnly Property PageSettings() As Pages
        Get
            If (_pages Is Nothing) Then
                _pages = New Pages(_settings)
            End If
            Return _pages
        End Get
    End Property

    ''' <summary>
    ''' Create and Gets the Clubs instance with DESettings
    ''' </summary>
    ''' <value>The clubs instance.</value>
    Public ReadOnly Property ClubSettings() As Clubs
        Get
            If (_clubs Is Nothing) Then
                _clubs = New Clubs(_settings)
            End If
            Return _clubs
        End Get
    End Property

    ''' <summary>
    ''' Create and Gets the Profile instance with DESettings
    ''' </summary>
    ''' <value>The users instance.</value>
    Public ReadOnly Property ProfileSettings() As Profile
        Get
            If (_profile Is Nothing) Then
                _profile = New Profile(_settings)
            End If
            Return _profile
        End Get
    End Property

    ''' <summary>
    ''' Create and Gets the Promotions instance with DESettings
    ''' </summary>
    ''' <value>The promotions instance.</value>
    Public ReadOnly Property PromotionsSettings() As Promotions
        Get
            If (_promotions Is Nothing) Then
                _promotions = New Promotions(_settings)
            End If
            Return _promotions
        End Get
    End Property

    ''' <summary>
    ''' Create and Gets the TicketDesigner instance with DESettings
    ''' </summary>
    ''' <value>The ticket designer instance.</value>
    Public ReadOnly Property TicketDesignerSettings() As TicketDesigner
        Get
            If (_ticketDesigner Is Nothing) Then
                _ticketDesigner = New TicketDesigner(_settings)
            End If
            Return _ticketDesigner
        End Get
    End Property

    ''' <summary>
    ''' Create and Gets the Sports instance with DESettings
    ''' </summary>
    ''' <value>The sports instance.</value>
    Public ReadOnly Property SportsSettings() As Sports
        Get
            If (_sports Is Nothing) Then
                _sports = New Sports(_settings)
            End If
            Return _sports
        End Get
    End Property

    ''' <summary>
    ''' Gets the tracking settings.
    ''' </summary>
    ''' <value>The tracking settings.</value>
    Public ReadOnly Property TrackingSettings() As Tracking
        Get
            If (_tracking Is Nothing) Then
                _tracking = New Tracking(_settings)
            End If
            Return _tracking
        End Get
    End Property

    ''' <summary>
    ''' Gets the payment settings.
    ''' </summary>
    ''' <value>The payment settings.</value>
    Public ReadOnly Property PaymentSettings() As Payment
        Get
            If (_payment Is Nothing) Then
                _payment = New Payment(_settings)
            End If
            Return _payment
        End Get
    End Property

    ''' <summary>
    ''' Gets the products settings.
    ''' </summary>
    ''' <value>The products settings.</value>
    Public ReadOnly Property ProductsSettings() As Products
        Get
            If (_products Is Nothing) Then
                _products = New Products(_settings)
            End If
            Return _products
        End Get
    End Property

    ''' <summary>
    ''' Gets the order settings.
    ''' </summary>
    ''' <value>The order settings.</value>
    Public ReadOnly Property OrderSettings() As Order
        Get
            If (_order Is Nothing) Then
                _order = New Order(_settings)
            End If
            Return _order
        End Get
    End Property

    ''' <summary>
    ''' Gets the feeds settings.
    ''' </summary>
    ''' <value>The feeds settings.</value>
    Public ReadOnly Property FeedsSettings() As Feeds
        Get
            If (_feeds Is Nothing) Then
                _feeds = New Feeds(_settings)
            End If
            Return _feeds
        End Get
    End Property

    ' ''' <summary>
    ' ''' Gets the TalentAdmin settings.
    ' ''' </summary>
    ' ''' <value>The TalentAdmin settings.</value>
    'Public ReadOnly Property TalentAdminSettings() As TalentAdmin
    '    Get
    '        If (_talentAdmin Is Nothing) Then
    '            _talentAdmin = New TalentAdmin(_settings)
    '        End If
    '        Return _talentAdmin
    '    End Get
    'End Property

    ''' <summary>
    ''' Gets the Talent Maintenance settings.
    ''' </summary>
    ''' <value>The Maintenance settings.</value>
    Public ReadOnly Property MaintenanceSettings() As Maintenance
        Get
            If (_maintenance Is Nothing) Then
                _maintenance = New Maintenance(_settings)
            End If
            Return _maintenance
        End Get
    End Property

    ''' <summary>
    ''' Gets the Delivery settings.
    ''' </summary>
    ''' <value>The Delivery settings.</value>
    Public ReadOnly Property DeliverySettings() As Delivery
        Get
            If (_delivery Is Nothing) Then
                _delivery = New Delivery(_settings)
            End If
            Return _delivery
        End Get
    End Property

    ''' <summary>
    ''' Gets the Agent settings.
    ''' </summary>
    ''' <value>The Agent settings.</value>
    Public ReadOnly Property AgentSettings() As Agent
        Get
            If (_agent Is Nothing) Then
                _agent = New Agent(_settings)
            End If
            Return _agent
        End Get
    End Property

    ''' <summary>
    ''' Gets the TalentDatabase settings.
    ''' </summary>
    ''' <value>The TalentDatabase settings.</value>
    Public ReadOnly Property TalentDatabaseSettings() As TalentDatabase
        Get
            If (_talentDatabase Is Nothing) Then
                _talentDatabase = New TalentDatabase(_settings)
            End If
            Return _talentDatabase
        End Get
    End Property

    ''' <summary>
    ''' Gets the Stadiums settings.
    ''' </summary>
    ''' <value>The Stadium settings.</value>
    Public ReadOnly Property StadiumSettings() As Stadiums
        Get
            If (_stadiums Is Nothing) Then
                _stadiums = New Stadiums(_settings)
            End If
            Return _stadiums
        End Get
    End Property

    ''' <summary>
    ''' Gets the Serialized settings.
    ''' </summary>
    ''' <value>The Serialized settings.</value>
    Public ReadOnly Property SerializedSettings() As Serialized
        Get
            If (_serialized Is Nothing) Then
                _serialized = New Serialized(_settings)
            End If
            Return _serialized
        End Get
    End Property

    ''' <summary>
    ''' Gets the Fees settings.
    ''' </summary>
    ''' <value>The Fees settings.</value>
    Public ReadOnly Property FeesSettings() As Fees
        Get
            If (_fees Is Nothing) Then
                _fees = New Fees(_settings)
            End If
            Return _fees
        End Get
    End Property

    ''' <summary>
    ''' Get the talent definition settings
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property TalentDefinitionsSettings() As TalentDefinitions
        Get
            If (_talentDefinitions Is Nothing) Then
                _talentDefinitions = New TalentDefinitions(_settings)
            End If
            Return _talentDefinitions
        End Get
    End Property

#End Region

#Region "Public Methods"

    
    ''' <summary>
    ''' Provides functionality to copy the records from many tables based on given from business unit and to business unit.
    ''' If the to business unit exists this will delete and copy the records 
    ''' Returns true if successfully copied otherwise false
    ''' </summary>
    ''' <param name="fromBusinessUnit">From business unit.</param>
    ''' <param name="toBusinessUnit">To business unit.</param>
    ''' <param name="toBusinessUnitDesc">To business unit desc.</param>
    ''' <param name="tblUrlBuURL">The TBL URL bu URL.</param>
    ''' <param name="fromStadiumName">Name of from stadium.</param>
    ''' <param name="toStadiumName">Name of to stadium.</param>
    ''' <param name="defaultNamesAndValues">The default names and values.</param><returns></returns>
    Public Function CopyByBUAll(ByVal fromBusinessUnit As String, ByVal toBusinessUnit As String, ByVal toBusinessUnitDesc As String, ByVal tblUrlBuURL As String, ByVal fromStadiumName As String, ByVal toStadiumName As String, ByVal defaultNamesAndValues As Dictionary(Of TblEcommerceModuleDefaultsBuEnum, String)) As Boolean
        Dim isCopyByBUSuccess As Boolean = False
        Dim talentSqlAccessDetail As New TalentDataAccess
        Dim err As New ErrorObj
        Dim sqlTrans As SqlTransaction
        Dim affectedRows As Integer
        Dim logContent As String = String.Empty
        Dim sourceClassName As String = String.Empty
        Dim SOURCEMETHOD As String = "COPYBYBUALL"

        Try
            talentSqlAccessDetail.Settings = _settings

            'check functionality is active or not
            If Not (Me.LoggingSettings.TblLogHeader.IsActive(SOURCEAPPLICATION, SOURCECLASS, SOURCEMETHOD, ActivityStatusEnum.INPROGRESS)) Then
                Dim logHeaderId As Integer = 0
                Dim isErrorinTransaction As Boolean = False

                Dim additionalDetails As String = "Copy By BU All transaction started" & _
                    " fromBusinessUnit : " & fromBusinessUnit & _
                    " toBusinessUnit : " & toBusinessUnit & _
                    " toBusinessUnitDesc : " & toBusinessUnitDesc & _
                    " tblUrlBuURL : " & tblUrlBuURL & _
                    " fromStadiumName : " & fromStadiumName & _
                    " toStadiumName : " & toStadiumName & " "

                affectedRows = Me.LoggingSettings.TblLogHeader.Create(SOURCEAPPLICATION, SOURCECLASS, SOURCEMETHOD, ActivityStatusEnum.INPROGRESS, additionalDetails, logHeaderId)
                If (logHeaderId > 0) Then

                    'create and get the transaction object
                    sqlTrans = talentSqlAccessDetail.BeginTransaction(err)


                    'call the stored proc from here
                    'get sql exception if any
                    'update the tbl log header with that

                    'then continue from ecom module updates and then urlbu updates

                    'no error then execute the statements using transaction object
                    If (Not (err.HasError)) Then
                        'usp_CopyBU_BUTables_SelAndInsByBu
                        If Not (isErrorinTransaction) Then
                            sourceClassName = "ProcessCopyByBUStoredProc"
                            If (Not (sqlTrans.Connection Is Nothing)) Then
                                err = ProcessCopyByBUStoredProc(sqlTrans, affectedRows, fromBusinessUnit, toBusinessUnit, toBusinessUnitDesc, tblUrlBuURL, fromStadiumName, toStadiumName)
                            End If
                            If (Not (sqlTrans.Connection Is Nothing)) Then
                                logContent = "Number of rows inserted: " & affectedRows
                                affectedRows = Me.LoggingSettings.TblLogs.WriteLog(logHeaderId, sourceClassName, "StoredProc", Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, logContent, sqlTrans)
                            Else
                                isErrorinTransaction = True
                                logContent = "Failed while inserting records to " & sourceClassName & ";" & err.ErrorMessage
                                affectedRows = Me.LoggingSettings.TblLogHeader.Update(logHeaderId, ActivityStatusEnum.FAILED, logContent)
                            End If
                        End If

                        'UPDATE TBL_ECOMMERCE_MODULE_DEFAULTS_BU VALUE FOR GIVEN DEFAULT_NAME and BUSINESS UNIT
                        If Not (isErrorinTransaction) Then
                            sourceClassName = "tbl_ecommerce_module_defaults_bu"
                            If (Not (sqlTrans.Connection Is Nothing)) Then
                                affectedRows = Me.AppVariableSettings.TblEcommerceModuleDefaultsBu.UpdateMultipleByBUAndDefaultName(toBusinessUnit, defaultNamesAndValues, sqlTrans)
                            End If
                            If (Not (sqlTrans.Connection Is Nothing)) Then
                                logContent = "Number of rows updated: " & affectedRows
                                affectedRows = Me.LoggingSettings.TblLogs.WriteLog(logHeaderId, sourceClassName, "UpdateMultipleByBUAndDefaultName", Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, logContent, sqlTrans)
                            Else
                                isErrorinTransaction = True
                                affectedRows = Me.LoggingSettings.TblLogHeader.Update(logHeaderId, ActivityStatusEnum.FAILED, "Failed while updating records to " & sourceClassName)
                            End If
                        End If

                        'This table is gateway to application
                        'TBL_URL_BU
                        If Not (isErrorinTransaction) Then
                            sourceClassName = "tbl_url_bu"
                            If (Not (sqlTrans.Connection Is Nothing)) Then
                                affectedRows = Me.AppVariableSettings.TblUrlBu.Update(toBusinessUnit, tblUrlBuURL, sqlTrans)
                            End If
                            If (Not (sqlTrans.Connection Is Nothing)) Then
                                logContent = "Number of rows updated: " & affectedRows
                                affectedRows = Me.LoggingSettings.TblLogs.WriteLog(logHeaderId, sourceClassName, "Update", Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, logContent, sqlTrans)
                            Else
                                isErrorinTransaction = True
                                affectedRows = Me.LoggingSettings.TblLogHeader.Update(logHeaderId, ActivityStatusEnum.FAILED, "Failed while updating records to " & sourceClassName)
                            End If
                        End If
                        If Not (isErrorinTransaction) Then
                            If (Not (sqlTrans.Connection Is Nothing)) Then
                                sqlTrans.Commit()
                                talentSqlAccessDetail.EndTransaction(sqlTrans)
                                affectedRows = Me.LoggingSettings.TblLogHeader.Update(logHeaderId, ActivityStatusEnum.SUCCESS, "Successfully Copied New Business Unit")
                                isCopyByBUSuccess = True
                            End If
                        End If
                    Else
                        'transaction object not able to create error

                    End If 'begin transaction error checking if ends
                Else
                    'failed to get log header id inform user

                End If ' logHeaderId checking if ends
            Else
                'functionality is running inform user

            End If ' functionality is active if ends
        Catch ex As Exception
        Finally
            talentSqlAccessDetail = Nothing
        End Try

        Return isCopyByBUSuccess
    End Function

    Private Function ProcessCopyByBUStoredProc(ByVal givenTransaction As SqlTransaction, ByRef affectedRows As Integer, ByVal fromBusinessUnit As String, ByVal toBusinessUnit As String, ByVal toBusinessUnitDesc As String, ByVal tblUrlBuURL As String, ByVal fromStadiumName As String, ByVal toStadiumName As String) As ErrorObj
        Dim err As New ErrorObj
        Dim talentSqlAccessDetail As New TalentDataAccess

        'Construct The Call
        talentSqlAccessDetail.Settings = _settings
        talentSqlAccessDetail.Settings.Cacheing = False
        talentSqlAccessDetail.Settings.CacheStringExtension = ""
        talentSqlAccessDetail.CommandElements.CommandType = CommandType.StoredProcedure
        talentSqlAccessDetail.CommandElements.CommandText = "usp_CopyByBU_BUTables_SelAndInsByBU"
        talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
        talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@pa_BusinessUnit_From", fromBusinessUnit))
        talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@pa_BusinessUnit_To", toBusinessUnit))
        talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@pa_BusinessUnit_To_Desc", toBusinessUnitDesc))
        talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@pa_StadiumName_From", fromStadiumName))
        talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@pa_StadiumName_To", toStadiumName))

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

        Return err
    End Function
#End Region

End Class
