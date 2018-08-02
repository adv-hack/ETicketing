Imports Microsoft.VisualBasic
Imports System.Data
Imports Talent.Common
Imports TEBUtilities = Talent.eCommerce.Utilities

Namespace Talent.eCommerce

    Public Class Agent
        Inherits ClassBase01

#Region "Class Level Fields"

        Private _reservationCodes As Generic.Dictionary(Of String, String)
        Private _ucr As UserControlResource
        Private _agentPermissions As DEAgentPermissions
        Private _sellAvailable As String

#End Region

#Region "Public Properties"

        Public ReadOnly Property IsAgent() As Boolean
            Get
                If HttpContext.Current.Session("Agent") IsNot Nothing Then
                    Return True
                Else
                    Return False
                End If
            End Get
        End Property
        Public ReadOnly Property PrintTransactionReceiptDefault() As Boolean
            Get
                Dim agentDetails As DataTable = GetAgentDetails()
                If agentDetails IsNot Nothing AndAlso agentDetails.Rows.Count > 0 Then
                    Return TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(agentDetails.Rows(0).Item("PRINT_TRANSACTION_RECEIPTS_DEFAULT"))
                Else
                    Return False
                End If
            End Get
        End Property
        Public ReadOnly Property PrintAddressLabelDefault() As Boolean
            Get
                Dim agentDetails As DataTable = GetAgentDetails()
                If agentDetails IsNot Nothing AndAlso agentDetails.Rows.Count > 0 Then
                    Return TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(agentDetails.Rows(0).Item("PRINT_ADDRESS_LABELS_DEFAULT"))
                Else
                    Return False
                End If

            End Get
        End Property
        Public Property Name() As String
            Get
                If HttpContext.Current.Session("Agent") IsNot Nothing Then
                    Return HttpContext.Current.Session("Agent")
                Else
                    Return String.Empty
                End If
            End Get
            Set(ByVal value As String)
                HttpContext.Current.Session("Agent") = value
            End Set
        End Property
        Public Property Type() As String
            Get
                If HttpContext.Current.Session("AgentType") IsNot Nothing Then
                    Return HttpContext.Current.Session("AgentType")
                Else
                    Return String.Empty
                End If
            End Get
            Set(ByVal value As String)
                HttpContext.Current.Session("AgentType") = value
            End Set
        End Property
        Public ReadOnly Property SellAvailableTickets() As Boolean
            Get
                If Not Name.Equals("") Then
                    RetrieveApprovedReservationCodes()
                    If _reservationCodes.ContainsKey(GlobalConstants.AGENT_SELL_AVAILABLE) Then
                        Return True
                    End If
                End If
                Return False
            End Get
        End Property
        Public ReadOnly Property ReservationCodes() As Generic.Dictionary(Of String, String)
            Get
                RetrieveApprovedReservationCodes()
                Return _reservationCodes
            End Get
        End Property
        Public ReadOnly Property IsAvailableToSell03() As Boolean
            Get
                Return GetAvailableToSell("03")
            End Get
        End Property
        Public ReadOnly Property Department(ByVal agentName As String) As String
            Get
                Dim agentDetails As DataTable = GetAgentDetails(agentName)
                If agentDetails IsNot Nothing AndAlso agentDetails.Rows.Count > 0 Then
                    Return TEBUtilities.CheckForDBNull_String(agentDetails.Rows(0).Item("DEPARTMENT"))
                Else
                    Return String.Empty
                End If
            End Get
        End Property
        Public ReadOnly Property BulkSalesMode() As Boolean
            Get
                Dim agentDetails As DataTable = GetAgentDetails()
                If agentDetails IsNot Nothing AndAlso agentDetails.Rows.Count > 0 Then
                    Return TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(agentDetails.Rows(0).Item("BULK_SALES_MODE"))
                Else
                    Return False
                End If
            End Get
        End Property
        Public ReadOnly Property AgentAuthorityGroupID() As Integer
            Get
                Dim agentDetails As DataTable = GetAgentDetails()
                If agentDetails IsNot Nothing AndAlso agentDetails.Rows.Count > 0 Then
                    Return TEBUtilities.CheckForDBNull_Int(agentDetails.Rows(0).Item("GROUP_ID"))
                Else
                    Return 0
                End If
            End Get
        End Property
        Public ReadOnly Property AgentPermissions() As DEAgentPermissions
            Get
                If _agentPermissions Is Nothing Then PopulateAgentPermissions()
                Return _agentPermissions
            End Get
        End Property

        Public ReadOnly Property DefaultCaptureMethod() As String
            Get
                Dim agentDetails As DataTable = GetAgentDetails()
                If agentDetails IsNot Nothing AndAlso agentDetails.Rows.Count > 0 Then
                    Return TEBUtilities.CheckForDBNull_String(agentDetails.Rows(0).Item("CAPTURE_METHOD"))
                Else
                    Return String.Empty
                End If
            End Get
        End Property

        Public ReadOnly Property PrintAlways() As Boolean
            Get
                Dim agentDetails As DataTable = GetAgentDetails()
                If agentDetails IsNot Nothing AndAlso agentDetails.Rows.Count > 0 Then
                    Return TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(agentDetails.Rows(0).Item("PRINT_ALWAYS"))
                Else
                    Return False
                End If
            End Get
        End Property
        Public ReadOnly Property CorporateHospitalityMode() As Boolean
            Get
                Dim agentDetails As DataTable = GetAgentDetails()
                If agentDetails IsNot Nothing AndAlso agentDetails.Rows.Count > 0 Then
                    Return TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(agentDetails.Rows(0).Item("CORPORATE_HOSPITALITY_MODE"))
                Else
                    Return False
                End If
            End Get
        End Property

#End Region

#Region "Constructors"

        Public Sub New()
            _ucr = New UserControlResource
            With _ucr
                .BusinessUnit = TalentCache.GetBusinessUnit()
                .PageCode = Talent.eCommerce.Utilities.GetCurrentPageName
                .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile, .BusinessUnit)
                .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
                .KeyCode = "Agent.vb"
            End With
        End Sub

#End Region

#Region "Public Methods"

        ''' <summary>
        ''' Populate the public properties for the current agent based on the given values
        ''' </summary>
        ''' <param name="isAgent">Is this an agent</param>
        ''' <param name="agentName">The given agent name</param>
        ''' <param name="agentType">The given agent type</param>
        ''' <remarks></remarks>
        Public Sub Populate(ByVal isAgent As Boolean, ByVal agentName As String, ByVal agentType As String)
            If isAgent Then
                Name = agentName
                Type = agentType
            Else
                HttpContext.Current.Session("AgentType") = Nothing
                HttpContext.Current.Session("Agent") = Nothing
            End If
        End Sub

        ''' <summary>
        ''' Clear the agent session objects
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub Clear()
            Type = String.Empty
            Name = String.Empty
            HttpContext.Current.Session("AgentType") = Nothing
            HttpContext.Current.Session("Agent") = Nothing
            If (HttpContext.Current.Session("LoggedInCompanyNumber") IsNot Nothing) Then HttpContext.Current.Session("LoggedInCompanyNumber") = Nothing
            If (HttpContext.Current.Session("LoggedInCompanyName") IsNot Nothing) Then HttpContext.Current.Session("LoggedInCompanyName") = Nothing
            If (HttpContext.Current.Session("CompanyNumber") IsNot Nothing) Then HttpContext.Current.Session("CompanyNumber") = Nothing
            If (HttpContext.Current.Session("SelectedCompanyNumber") IsNot Nothing) Then HttpContext.Current.Session("SelectedCompanyNumber") = Nothing
        End Sub

        ''' <summary>
        ''' Perform an agent logout. This clears the local session and logs the agent off.
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub Logout()
            Dim err As New ErrorObj
            Dim settings As DESettings = TEBUtilities.GetSettingsObject()
            Dim talAgent As New TalentAgent
            Dim agentDataEntity As New DEAgent
            agentDataEntity.AgentUsername = Name
            agentDataEntity.Source = GlobalConstants.SOURCE
            agentDataEntity.AgentCompany = GetAgentCompany(Name)
            agentDataEntity.SessionID = CType(HttpContext.Current.Profile, TalentProfile).Basket.Basket_Header_ID
            agentDataEntity.DeleteSessionRecord = True
            talAgent.AgentDataEntity = agentDataEntity
            talAgent.Settings = settings
            err = talAgent.AgentLogout

            TDataObjects.AgentSettings.TblAgent.Delete(HttpContext.Current.Session.SessionID, Name)
            Clear()
            Dim defaultsObject As New ECommerceModuleDefaults
            Dim moduleDefaults As ECommerceModuleDefaults.DefaultValues = defaultsObject.GetDefaults
            Dim noiseSettings As Talent.Common.DESettings = Talent.eCommerce.Utilities.GetSettingsObject
            Dim myNoise As New Talent.Common.TalentNoise(noiseSettings, HttpContext.Current.Session.SessionID, Now, _
                                                        Now.AddMinutes(-moduleDefaults.NOISE_THRESHOLD_MINUTES), moduleDefaults.NOISE_MAX_SESSION_KEEP_ALIVE_MINUTES)
            myNoise.RemoveSpecificNoiseSession()
        End Sub

        ''' <summary>
        ''' Populates the agent permissions per group. The permissions are cached per group.
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub PopulateAgentPermissions()
            Dim agentGroupPermissions As New Generic.Dictionary(Of String, String)
            Dim groupId As Integer = 0
            groupId = AgentAuthorityGroupID
            agentGroupPermissions = TDataObjects.TalentDefinitionsSettings.GetAgentGroupPermissions(groupId)
            _agentPermissions = New DEAgentPermissions
            If agentGroupPermissions IsNot Nothing AndAlso agentGroupPermissions.Count > 0 Then
                For Each permission As String In agentGroupPermissions.Keys
                    Try
                        CallByName(_agentPermissions, permission, CallType.Set, True)
                    Catch ex As MissingMemberException
                        'The permission in the table has not been found in the application
                    End Try
                Next
            End If
        End Sub

#End Region

#Region "Public Functions"

        ''' <summary>
        ''' Get the default printer name for the current agent
        ''' </summary>
        ''' <returns>The default printer name</returns>
        ''' <remarks></remarks>
        Public Function GetAgentDefaultPrinterName() As String
            Dim printerNameDefault As String = String.Empty
            Dim dtTblAgent As DataTable = TDataObjects.AgentSettings.TblAgent.GetByAgentName(HttpContext.Current.Session.SessionID, Me.Name)
            If dtTblAgent IsNot Nothing AndAlso dtTblAgent.Rows.Count > 0 Then
                printerNameDefault = Talent.eCommerce.Utilities.CheckForDBNull_String(dtTblAgent.Rows(0)("PRINTER_NAME_DEFAULT"))
            End If
            Return printerNameDefault
        End Function

        ''' <summary>
        ''' Get the current agent company
        ''' </summary>
        ''' <returns>The company for the current agent</returns>
        ''' <remarks></remarks>
        Public Function GetAgentCompany() As String
            Dim agentCompany As String = String.Empty
            Dim dtTblAgent As DataTable = TDataObjects.AgentSettings.TblAgent.GetByAgentName(HttpContext.Current.Session.SessionID, Me.Name)
            If dtTblAgent IsNot Nothing AndAlso dtTblAgent.Rows.Count > 0 Then
                agentCompany = Talent.eCommerce.Utilities.CheckForDBNull_String(dtTblAgent.Rows(0)("COMPANY"))
            End If
            Return agentCompany
        End Function

        ''' <summary>
        ''' Get the agent company
        ''' </summary>
        ''' <param name="agentName">The given agent name</param>
        ''' <returns>The company for the given agent</returns>
        ''' <remarks></remarks>
        Public Function GetAgentCompany(ByVal agentName As String) As String
            Dim agentCompany As String = String.Empty
            Dim dtTblAgent As DataTable = GetAgentDetails(agentName)
            If dtTblAgent IsNot Nothing AndAlso dtTblAgent.Rows.Count > 0 Then
                agentCompany = Talent.eCommerce.Utilities.CheckForDBNull_String(dtTblAgent.Rows(0)("COMPANY"))
            End If
            Return agentCompany
        End Function

        ''' <summary>
        ''' Get the agent data table based on the current agent
        ''' </summary>
        ''' <returns>The agent datatable</returns>
        ''' <remarks></remarks>
        Public Function GetAgentDetails() As DataTable
            Dim dtTblAgent As DataTable = TDataObjects.AgentSettings.TblAgent.GetByAgentName(HttpContext.Current.Session.SessionID, Me.Name)
            Return dtTblAgent
        End Function

        ''' <summary>
        ''' Get the agent data table based on the given agent
        ''' </summary>
        ''' <param name="AgentName">The given agent</param>
        ''' <returns>The agent datatable</returns>
        ''' <remarks></remarks>
        Public Function GetAgentDetails(ByVal AgentName As String) As DataTable
            Dim dtTblAgent As DataTable = Nothing
            Dim err As New ErrorObj
            Dim settings As DESettings = TEBUtilities.GetSettingsObject()
            Dim talAgent As New TalentAgent
            Dim agentDataEntity As New DEAgent
            agentDataEntity.Source = GlobalConstants.SOURCE
            talAgent.AgentDataEntity = agentDataEntity
            talAgent.Settings = settings
            err = talAgent.RetrieveAllAgents()

            If Not err.HasError Then
                If talAgent.ResultDataSet.Tables("AgentUsers") IsNot Nothing AndAlso talAgent.ResultDataSet.Tables("AgentUsers").Rows.Count > 0 Then
                    Dim drTblAgent() As DataRow = talAgent.ResultDataSet.Tables("AgentUsers").Select("USERCODE = '" & AgentName & "'")
                    If drTblAgent.Length > 0 Then
                        dtTblAgent = drTblAgent.CopyToDataTable
                    End If
                End If
            End If
            Return dtTblAgent
        End Function

        ''' <summary>
        ''' Check to see if the agent can sell the given reservation code
        ''' </summary>
        ''' <param name="reservationCode">The reservation code to check</param>
        ''' <returns>True if the agent can sell this reservation code</returns>
        ''' <remarks></remarks>
        Public Function GetAvailableToSell(ByVal reservationCode As String) As String
            RetrieveApprovedReservationCodes()
            If _reservationCodes.ContainsKey(reservationCode) Then
                Return True
            End If
            Return False
        End Function

        ''' <summary>
        ''' Get the department description for display based on the given department code
        ''' </summary>
        ''' <param name="departmentCode">The given department code</param>
        ''' <returns>The correct description</returns>
        ''' <remarks></remarks>
        Public Function GetAgentDepartmentDescription(ByVal departmentCode As String) As String
            Dim departmentDescription As String = String.Empty
            Dim settings As DESettings = TEBUtilities.GetSettingsObject()
            Dim talAgent As New TalentAgent
            Dim agentDataEntity As New DEAgent
            Dim err As New ErrorObj
            agentDataEntity.Source = GlobalConstants.SOURCE
            talAgent.AgentDataEntity = agentDataEntity
            talAgent.Settings = settings
            err = talAgent.RetrieveAgentDepartments()

            If Not err.HasError AndAlso talAgent.ResultDataSet IsNot Nothing Then
                If talAgent.ResultDataSet.Tables("Departments") IsNot Nothing AndAlso talAgent.ResultDataSet.Tables("Departments").Rows.Count > 0 Then
                    For Each row As DataRow In talAgent.ResultDataSet.Tables("Departments").Rows
                        If row("DepartmentReference").ToString().Trim() = departmentCode Then
                            departmentDescription = row("DepartmentDescription").ToString().Trim()
                            Exit For
                        End If
                    Next
                End If
            End If
            Return departmentDescription
        End Function

#End Region

#Region "Private Methods"

        ''' <summary>
        ''' Retrieve all the approved reservations codes for this agent
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub RetrieveApprovedReservationCodes()
            If _reservationCodes Is Nothing Then
                _reservationCodes = New Generic.Dictionary(Of String, String)

                'call the reservation
                If Not Name.Equals("") Then
                    Dim err As New ErrorObj
                    Dim agentDataEntity As New DEAgent
                    Dim talAgent As New TalentAgent
                    talAgent.Settings = Talent.eCommerce.Utilities.GetSettingsObject
                    talAgent.Settings.Cacheing = TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(_ucr.Attribute("ApprovedReservationCodesCacheing"))
                    talAgent.Settings.CacheTimeMinutes = TEBUtilities.CheckForDBNull_Int(_ucr.Attribute("ApprovedReservationCodesCacheTimeMinutes"))
                    talAgent.AgentDataEntity = agentDataEntity
                    agentDataEntity.AgentUsername = Name
                    err = talAgent.RetrieveApprovedReservationCodes()

                    If Not talAgent.TalDataSet.DictionaryDataSet Is Nothing Then
                        _reservationCodes = talAgent.TalDataSet.DictionaryDataSet
                    End If
                Else
                    _reservationCodes.Add("03", "03")
                    _reservationCodes.Add("07", "07")
                End If
            End If
        End Sub

#End Region

    End Class

End Namespace