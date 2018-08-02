Imports Microsoft.VisualBasic
Imports System.Data.SqlClient
Imports IBM.Data.DB2.iSeries
Imports System.Text
Imports Talent.Common
Imports Talent.Common.Utilities
Imports Talent.Common.UtilityExtension

<Serializable()> _
Public Class DBCustomer
    Inherits DBAccess

#Region "Class Level Fields"

    Private Const VerifyPassword As String = "VerifyPassword"
    Private Const UpdatePassword As String = "UpdatePassword"
    Private Const RetrievePassword As String = "RetrievePassword"
    Private Const ResetPassword As String = "ResetPassword"
    Private Const CheckCustomer As String = "CheckCustomer"
    Private Const CustomerRetrieval As String = "CustomerRetrieval"
    Private Const CustomerProfileRetrieval As String = "CustomerProfileRetrieval"
    Private Const ValidateCustomer As String = "ValidateCustomer"
    Private Const SetCustomer As String = "SetCustomer"
    Private Const AddCustomerAssociation As String = "AddCustomerAssociation"
    Private Const DeleteCustomerAssociation As String = "DeleteCustomerAssociation"
    Private Const CustomerAssociations As String = "CustomerAssociations"
    Private Const CanMoreCustomerAssociationsCanBeAdded As String = "CanMoreCustomerAssociationsCanBeAdded"
    Private Const VerifyAndRetrieveCustomerDetails As String = "VerifyAndRetrieveCustomerDetails"
    Private Const GeneratePassword As String = "GeneratePassword"
    Private Const SetParticipantsAndBasket As String = "SetParticipantsAndBasket"
    Private Const UpdateCustomerDetailsSingleMode As String = "UpdateCustomerDetailsSingleMode"
    Private Const AttributeDefinitionRetrieval As String = "AttributeDefinitionRetrieval"
    Private Const RetrieveAddressCapture As String = "RetrieveAddressCapture"
    Private Const RetrieveReservedAndSoldSeats As String = "RetrieveReservedAndSoldSeats"
    Private Const RetrieveCourseDetails As String = "RetrieveCourseDetails"
    Private Const UpdateCourseDetails As String = "UpdateCourseDetails"
    Private Const CustomerMembershipsRetrieval As String = "CustomerMembershipsRetrieval"
    Private Const CustomerActivitiesRetrieval As String = "CustomerActivitiesRetrieval"
    Private Const RetrieveCustomersatAddress As String = "RetrieveCustomersatAddress"
    Private Const UpdateCustomerAddresses As String = "UpdateCustomerAddresses"

    Private _deV11 As New DECustomerV11
    Private _customers As New List(Of DECustomerV11)
    Private _deAddTicketingItems As DEAddTicketingItems
    Private _saveCardFunctions As New DBSaveCardFunctions
    Private _foundAddressOnlyFromCRM As Boolean = False
    Private _de As DECustomer
    Private _contactShortTitle As String = String.Empty
    Private _contactLongTitle As String = String.Empty

#End Region

#Region "Public Properties"

    Public AgentDataEntity As DEAgent
    Public Property DeV11() As DECustomerV11
        Get
            Return _deV11
        End Get
        Set(ByVal value As DECustomerV11)
            _deV11 = value
        End Set
    End Property
    Public Property Customers() As ICollection(Of DECustomerV11)
        Get
            Return _customers
        End Get
        Set(ByVal value As ICollection(Of DECustomerV11))
            _customers = value
        End Set
    End Property
    Public Property DeAddTicketingItems() As DEAddTicketingItems
        Get
            Return _deAddTicketingItems
        End Get
        Set(ByVal value As DEAddTicketingItems)
            _deAddTicketingItems = value
        End Set
    End Property
    Public Property RefreshUserAttributesOnLogin() As Boolean = False
    Public Property CourseDetailsFanFlag() As Boolean = False
    Public Property CourseDetailsContactName() As String
    Public Property CourseDetailsContactNumber() As String
    Public Property CourseDetailsMedicalInfo() As String

#End Region

#Region "Protected Functions"

    Protected Overrides Function AccessDataBaseTALENTCRM() As ErrorObj
        Dim err As New ErrorObj
        _de = DeV11.DECustomersV1(0)

        Select Case _settings.ModuleName
            Case Is = SetCustomer : err = AccessDataBaseTALENTCRM_SetCustomer()
            Case Is = VerifyAndRetrieveCustomerDetails : err = AccessDatabaseTALENTCRM_VerifyAndRetrieveCustomerDetails()
        End Select
        Return err
    End Function

    Protected Overrides Function AccessDataBaseTALENTTKT() As ErrorObj
        Dim err As New ErrorObj
        If DeV11.DECustomersV1.Count > 0 Then
            _de = DeV11.DECustomersV1(0)
        End If

        Select Case _settings.ModuleName
            Case Is = VerifyPassword : err = AccessDatabaseWS120R()
            Case Is = UpdatePassword : err = AccessDatabaseWS120R()
            Case Is = RetrievePassword : err = AccessDatabaseWS120R()
            Case Is = ResetPassword : err = AccessDatabaseWS120R()
            Case Is = AddCustomerAssociation : err = AccessDatabaseWS027R()
            Case Is = DeleteCustomerAssociation : err = AccessDatabaseWS027R()
            Case Is = CustomerAssociations : err = AccessDatabaseWS026R()
            Case Is = CanMoreCustomerAssociationsCanBeAdded : err = AccessDatabaseWS026R()
            Case Is = VerifyAndRetrieveCustomerDetails : err = AccessDatabaseWS607R()
            Case Is = CustomerProfileRetrieval : err = CustomerAttributes()
            Case Is = ValidateCustomer : err = AccessDatabaseWS009R()
            Case Is = CustomerRetrieval : err = AccessDatabaseWS009R()
            Case Is = GeneratePassword : err = AccessDatabaseWS120R()
            Case Is = SetParticipantsAndBasket : err = AccessDatabaseWS616R()
            Case Is = UpdateCustomerDetailsSingleMode : err = AccessDatabaseWS003R()
            Case Is = AttributeDefinitionRetrieval : err = AccessDatabaseWS059R()
            Case Is = RetrieveAddressCapture : err = AccessDatabaseWS140R()
            Case Is = RetrieveReservedAndSoldSeats : err = AccessDatabaseWS133R()
            Case Is = RetrieveCourseDetails : err = AccessDatabase_RetrieveCourseDetails()
            Case Is = UpdateCourseDetails : err = AccessDatabase_UpdateCourseDetails()
            Case Is = CustomerMembershipsRetrieval : err = AccessDatabaseWS272R()
            Case Is = CustomerActivitiesRetrieval : err = RetrieveActivities()
            Case Is = RetrieveCustomersAtAddress : err = AccessDatabaseCD020S()
            Case Is = UpdateCustomerAddresses : err = AccessDatabaseWS165R()
            Case Else : err = AddUpdateCustomerDetails()
        End Select

        Return err
    End Function

    Protected Overrides Function AccessDataBaseSQL2005() As ErrorObj
        Dim err As New ErrorObj
        Select Case _settings.ModuleName
            Case Is = SetCustomer : err = ProcessCustomerSql2005()
            Case Is = CustomerRetrieval : err = CustomerRetrievalSql2005()
            Case Is = RetrievePassword : err = RetrievePasswordSql2005()
            Case Is = ResetPassword : err = ResetPasswordSql2005()
            Case Is = VerifyPassword : err = VerifyPasswordSql2005()
            Case Else : err = ProcessCustomerSql2005()
        End Select
        Return err
    End Function

    Protected Function CustomerRetrievalSql2005() As ErrorObj
        Dim err As New ErrorObj
        Dim dRow As DataRow = Nothing
        ResultDataSet = New DataSet
        Dim DtStatusResults As New DataTable(GlobalConstants.STATUS_RESULTS_TABLE_NAME)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With
        Dim DtCustomerRetrieval As New DataTable("CustomerResults")
        ResultDataSet.Tables.Add(DtStatusResults)
        ResultDataSet.Tables.Add(DtCustomerRetrieval)
        AddCustomerRetrievalColumns(DtCustomerRetrieval)
        Dim SQL As New StringBuilder
        Dim cmd As New SqlCommand()
        cmd.Connection = conSql2005
        SQL.Append("SELECT pu.LOGINID, pu.TITLE, pu.INITIALS, pu.FORENAME, pu.SURNAME, pu.SALUTATION, pu.POSITION, ")
        SQL.Append("a.ADDRESS_LINE_1, a.ADDRESS_LINE_2, a.ADDRESS_LINE_3, a.ADDRESS_LINE_4, a.ADDRESS_LINE_5, a.POST_CODE, ")
        SQL.Append("pu.SEX, pu.TELEPHONE_NUMBER, pu.WORK_NUMBER, pu.MOBILE_NUMBER, pu.EMAIL, pu.DOB, ")
        SQL.Append("pu.SUBSCRIBE_NEWSLETTER, pu.SUBSCRIBE_2, pu.SUBSCRIBE_3, pu.BIT0, pu.BIT1, pu.BIT2, pu.BIT3, pu.BIT4, pu.BIT5, ")
        SQL.Append("pu.PASSPORT, pu.GREENCARD, pu.TICKETING_LOYALTY_POINTS, pu.OWNS_AUTO_MEMBERSHIP, pu.BOND_HOLDER, pu.PIN, ")
        SQL.Append("pu.USER_ID_4, pu.USER_ID_5, pu.USER_ID_6, pu.USER_ID_7, pu.USER_ID_8, pu.USER_ID_9 ")
        SQL.Append("FROM tbl_partner_user pu ")
        SQL.Append("INNER JOIN tbl_address a ON pu.LOGINID = a.LOGINID ")

        If String.IsNullOrEmpty(DeV11.DECustomersV1(0).CustomerNumber) Then
            SQL.Append("WHERE pu.EMAIL = @EMAIL")
            cmd.Parameters.Add("@EMAIL", SqlDbType.NVarChar).Value = DeV11.DECustomersV1(0).EmailAddress
        Else
            SQL.Append("WHERE pu.LOGINID = @LOGINID")
            cmd.Parameters.Add("@LOGINID", SqlDbType.NVarChar).Value = DeV11.DECustomersV1(0).CustomerNumber
        End If
        cmd.CommandText = SQL.ToString()

        Try
            Dim da As New SqlDataAdapter(cmd)
            Dim ds As New DataSet()
            dRow = DtStatusResults.NewRow
            da.Fill(ds)
            If ds.Tables.Count > 0 Then
                Dim dt As DataTable = ds.Tables(0)
                If dt.Rows.Count > 0 Then
                    Dim dr As DataRow = dt.Rows(0)
                    Dim row As DataRow = DtCustomerRetrieval.NewRow
                    row("CustomerNumber") = dr.Item("LOGINID")
                    row("ContactTitle") = dr.Item("TITLE")
                    row("ContactInitials") = dr.Item("INITIALS")
                    row("ContactForename") = dr.Item("FORENAME")
                    row("ContactSurname") = dr.Item("SURNAME")
                    row("Salutation") = dr.Item("SALUTATION")
                    row("CompanyName") = ""
                    row("PositionInCompany") = dr.Item("POSITION")
                    row("AddressLine1") = dr.Item("ADDRESS_LINE_1")
                    row("AddressLine2") = dr.Item("ADDRESS_LINE_2")
                    row("AddressLine3") = dr.Item("ADDRESS_LINE_3")
                    row("AddressLine4") = dr.Item("ADDRESS_LINE_4")
                    row("AddressLine5") = dr.Item("ADDRESS_LINE_5")
                    row("PostCode") = dr.Item("POST_CODE")
                    row("Gender") = dr.Item("SEX")
                    row("HomeTelephoneNumber") = dr.Item("TELEPHONE_NUMBER")
                    row("WorkTelephoneNumber") = dr.Item("WORK_NUMBER")
                    row("MobileNumber") = dr.Item("MOBILE_NUMBER")
                    row("EmailAddress") = dr.Item("EMAIL")
                    row("DateBirth") = dr.Item("DOB")
                    row("ContactViaMail") = dr.Item("BIT0")
                    row("Subscription1") = dr.Item("SUBSCRIBE_NEWSLETTER")
                    row("Subscription2") = dr.Item("SUBSCRIBE_2")
                    row("Subscription3") = dr.Item("SUBSCRIBE_3")
                    row("ContactViaMail1") = dr.Item("BIT1")
                    row("ContactViaMail2") = dr.Item("BIT2")
                    row("ContactViaMail3") = dr.Item("BIT3")
                    row("ContactViaMail4") = dr.Item("BIT4")
                    row("ContactViaMail5") = dr.Item("BIT5")
                    row("LoyaltyPoints") = dr.Item("TICKETING_LOYALTY_POINTS")
                    row("PassportNumber") = dr.Item("PASSPORT")
                    row("GreenCardNumber") = dr.Item("GREENCARD")
                    row("OwnsAutoMembership") = dr.Item("OWNS_AUTO_MEMBERSHIP")
                    row("BondHolder") = dr.Item("BOND_HOLDER")
                    row("PINNumber") = dr.Item("PIN")
                    row("ExternalId1") = ""
                    row("ExternalId2") = ""
                    row("PasswordHint") = ""
                    row("OldPassword") = ""
                    row("ATSReady") = ""
                    row("PriceBand") = ""
                    row("WebReady") = ""
                    row("STHolder") = ""
                    row("SCHolder") = ""
                    row("UserID4") = dr.Item("USER_ID_4")
                    row("UserID5") = dr.Item("USER_ID_5")
                    row("UserID6") = dr.Item("USER_ID_6")
                    row("UserID7") = dr.Item("USER_ID_7")
                    row("UserID8") = dr.Item("USER_ID_8")
                    row("UserID9") = dr.Item("USER_ID_9")
                    DtCustomerRetrieval.Rows.Add(row)

                    dRow("ErrorOccurred") = ""
                    dRow("ReturnCode") = ""
                Else
                    dRow("ErrorOccurred") = GlobalConstants.ERRORFLAG
                    dRow("ReturnCode") = "MC"
                End If
            Else
                dRow("ErrorOccurred") = GlobalConstants.ERRORFLAG
                dRow("ReturnCode") = "MC"
            End If
            DtStatusResults.Rows.Add(dRow)
        Catch ex As Exception
            Const strError As String = "Error Retrieving Authorised Users details "
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBUDAU-98"
                .HasError = True
            End With
        End Try

        Return err
    End Function

    Protected Function RetrievePasswordSql2005() As ErrorObj
        Dim err As New ErrorObj
        ResultDataSet = New DataSet
        Dim DtPasswordResults As New DataTable("RetrievePasswordResults")
        ResultDataSet.Tables.Add(DtPasswordResults)
        AddPasswordResultsColumns(DtPasswordResults)

        Dim SQL As New StringBuilder
        SQL.Append("SELECT LOGINID as UserName, [PASSWORD] as Password ")
        SQL.Append("FROM tbl_authorized_users ")
        SQL.Append("WHERE [LOGINID] = @LOGINID")
        Dim cmd As New SqlCommand(SQL.ToString, conSql2005)
        With cmd.Parameters
            .Add("@LOGINID", SqlDbType.NVarChar).Value = DeV11.DECustomersV1(0).CustomerNumber
        End With

        Try
            Dim da As New SqlDataAdapter(cmd)
            Dim ds As New DataSet()
            da.Fill(ds)
            If ds.Tables.Count > 0 Then
                Dim dt As DataTable = ds.Tables(0)
                If dt.Rows.Count > 0 Then
                    Dim dr As DataRow = DtPasswordResults.NewRow()
                    dr("UserName") = dt.Rows(0)("UserName")
                    dr("Password") = dt.Rows(0)("Password")
                    DtPasswordResults.Rows.Add(dr)
                End If
            End If
        Catch ex As Exception
            Const strError As String = "Error Retrieving Authorised Users' password "
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBUDAU-99"
                .HasError = True
            End With
        End Try

        Return err
    End Function

    Protected Function ProcessCustomerSql2005() As ErrorObj
        Dim err As New ErrorObj

        Me.ResultDataSet = New DataSet

        'Create the Status data table
        Dim DtStatusResults As New DataTable
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With


        Dim pt As New DataTable("Partners")
        Dim ut As New DataTable("Users")
        Dim at As New DataTable("Addresses")

        With pt.Columns
            .Add("Partner", GetType(String))
            .Add("PartnerSuccess", GetType(Boolean))
            .Add("AddressSuccess", GetType(Boolean))
        End With

        With ut.Columns
            .Add("Partner", GetType(String))
            .Add("LoginID", GetType(String))
            .Add("AuthoriseSuccess", GetType(Boolean))
            .Add("UserSuccess", GetType(Boolean))
        End With

        With at.Columns
            .Add("Partner", GetType(String))
            .Add("LoginID", GetType(String))
            .Add("Postcode", GetType(String))
            .Add("AddressSuccess", GetType(Boolean))
        End With

        Me.ResultDataSet.Tables.Add(pt)
        Me.ResultDataSet.Tables.Add(ut)
        Me.ResultDataSet.Tables.Add(at)

        Dim cmd As New SqlCommand("", conSql2005)

        For Each dePart As DECustomerV11.DECustomerSite In DeV11.Sites

            Dim pr As DataRow = pt.NewRow
            pt.Rows.Add(pr)
            pr("Partner") = dePart.Name
            pr("PartnerSuccess") = False
            pr("AddressSuccess") = False

            If String.IsNullOrEmpty(dePart.UpdateMode) Then
                err = AddPartnerSql2005(cmd, dePart)
            Else
                Select Case dePart.UpdateMode
                    Case "UPDATE"
                        err = UpdatePartnerSql2005(cmd, dePart)
                    Case "DELETE"
                        err = DeletePartnerSql2005(cmd, dePart)
                End Select
            End If

            If Not err.HasError Then
                pr("PartnerSuccess") = True
            End If


            If String.IsNullOrEmpty(dePart.Address.UpdateMode) Then
                err = AddAddressSql2005(cmd, dePart.Address, dePart.Name, "", True)
            Else
                Select Case dePart.Address.UpdateMode
                    Case "UPDATE"
                        err = UpdateAddressSql2005(cmd, dePart.Address, dePart.Name, "", True)
                    Case "DELETE"
                        err = DeleteAddressSql2005(cmd, dePart.Address, dePart.Name, "", True)
                End Select
            End If

            If Not err.HasError Then
                pr("AddressSuccess") = True
            End If

            For Each deUsr As DECustomerV11.DECustomerSiteContact In dePart.Contacts

                Dim ur As DataRow = ut.NewRow
                ut.Rows.Add(ur)
                ur("Partner") = dePart.Name
                ur("LoginID") = deUsr.LoginID
                ur("AuthoriseSuccess") = False
                ur("UserSuccess") = False

                If Not String.IsNullOrEmpty(deUsr.LoginID) Then
                    If Not String.IsNullOrEmpty(deUsr.Password) Then
                        If String.IsNullOrEmpty(deUsr.UpdateMode) Then
                            err = AddAuthorisedUserSql2005(cmd, deUsr, dePart.Name)
                        Else
                            Select Case deUsr.UpdateMode
                                Case "UPDATE"
                                    err = UpdateAuthorisedUserSql2005(cmd, deUsr, dePart.Name)
                                Case "DELETE"
                                    err = DeleteAuthorisedUserSql2005(cmd, deUsr, dePart.Name)
                            End Select
                        End If
                        If Not err.HasError Then
                            ur("AuthoriseSuccess") = True
                        End If
                    End If


                    If String.IsNullOrEmpty(deUsr.UpdateMode) Then
                        err = AddPartnerUserSql2005(cmd, deUsr, dePart.Name)
                    Else
                        Select Case deUsr.UpdateMode
                            Case "UPDATE"
                                err = UpdatePartnerUserSql2005(cmd, deUsr, dePart.Name)
                            Case "DELETE"
                                err = DeletePartnerUserSql2005(cmd, deUsr, dePart.Name)
                        End Select
                    End If

                    If Not err.HasError Then
                        ur("UserSuccess") = True
                    End If

                    For Each deAdd As DECustomerV11.DECustomerSiteAddress In deUsr.Addresses
                        Dim ar As DataRow = at.NewRow
                        at.Rows.Add(ar)
                        ar("Partner") = dePart.Name
                        ar("LoginID") = deUsr.LoginID
                        ar("Postcode") = deAdd.Postcode
                        ar("AddressSuccess") = False

                        If String.IsNullOrEmpty(deAdd.UpdateMode) Then
                            err = AddAddressSql2005(cmd, deAdd, dePart.Name, deUsr.LoginID, False)
                        Else
                            Select Case deAdd.UpdateMode
                                Case "UPDATE"
                                    err = UpdateAddressSql2005(cmd, deAdd, dePart.Name, deUsr.LoginID, False)
                                Case "DELETE"
                                    err = DeleteAddressSql2005(cmd, deAdd, dePart.Name, deUsr.LoginID, False)
                            End Select
                        End If

                        If Not err.HasError Then
                            ar("AddressSuccess") = True
                        End If

                    Next
                End If
            Next

        Next

        Return err
    End Function

    Protected Function ResetPasswordSql2005() As ErrorObj
        Dim err As New ErrorObj
        Dim dRow As DataRow = Nothing
        Dim passwordString As String = String.Empty
        ResultDataSet = New DataSet
        Dim DtStatusResults As New DataTable(GlobalConstants.STATUS_RESULTS_TABLE_NAME)
        Dim SQL As New StringBuilder

        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With
        ResultDataSet.Tables.Add(DtStatusResults)
        If String.IsNullOrEmpty(DeV11.DECustomersV1(0).NewHashedPassword) Then
            passwordString = DeV11.DECustomersV1(0).Password
        Else
            passwordString = DeV11.DECustomersV1(0).NewHashedPassword
        End If

        SQL.Append("UPDATE tbl_authorized_users SET [PASSWORD] = @PASSWORD ")
        SQL.Append("WHERE [LOGINID] = @LOGINID")
        Dim cmd As New SqlCommand(SQL.ToString, conSql2005)
        With cmd.Parameters
            .Add("@PASSWORD", SqlDbType.NVarChar).Value = passwordString
            .Add("@LOGINID", SqlDbType.NVarChar).Value = DeV11.DECustomersV1(0).CustomerNumber
        End With
        Try
            dRow = DtStatusResults.NewRow
            cmd.ExecuteNonQuery()
            dRow("ErrorOccurred") = String.Empty
            dRow("ReturnCode") = String.Empty
        Catch ex As Exception
            dRow("ErrorOccurred") = GlobalConstants.ERRORFLAG
            dRow("ReturnCode") = "XX"
            Const strError As String = "Error resetting the customer password"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBUDAU-97"
                .HasError = True
            End With
        End Try
        DtStatusResults.Rows.Add(dRow)
        Return err
    End Function

    Protected Function DeletePartnerSql2005(ByVal cmd As SqlCommand, ByVal dePart As DECustomerV11.DECustomerSite) As ErrorObj
        Dim err As New ErrorObj

        Const deleteStr As String = " DELETE FROM [tbl_partner] " & _
                                    " WHERE [PARTNER] = @PARTNER "

        cmd.CommandText = deleteStr

        With cmd.Parameters
            .Clear()
            .Add("@PARTNER", SqlDbType.NVarChar).Value = dePart.Name
        End With

        Try
            cmd.ExecuteNonQuery()
        Catch ex As Exception
            Const strError As String = "Error Deleting Partner Record "
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBDP-01"
                .HasError = True
            End With
        End Try

        Return err
    End Function

    Protected Function DeleteAddressSql2005(ByVal cmd As SqlCommand, ByVal deAdd As DECustomerV11.DECustomerSiteAddress, ByVal partnerStr As String, ByVal loginID As String, ByVal isPartner As Boolean) As ErrorObj
        Dim err As New ErrorObj

        Const deleteStr As String = " DELETE FROM [tbl_address] " & _
                                    " WHERE [PARTNER] = @PARTNER " & _
                                    " AND {0} "

        cmd.CommandText = deleteStr

        If isPartner Then
            cmd.CommandText = String.Format(cmd.CommandText, "(LOGINID = '' OR LOGINID IS NULL)")
        Else
            cmd.CommandText = String.Format(cmd.CommandText, "LOGINID = @LOGINID")
        End If

        With cmd.Parameters
            .Clear()
            .Add("@PARTNER", SqlDbType.NVarChar).Value = partnerStr
            .Add("@LOGINID", SqlDbType.NVarChar).Value = loginID
        End With

        Try
            cmd.ExecuteNonQuery()
        Catch ex As Exception
            Const strError As String = "Error Deleteing Address Record "
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBDA-01"
                .HasError = True
            End With
        End Try

        Return err
    End Function

    Protected Function DeleteAuthorisedUserSql2005(ByVal cmd As SqlCommand, ByVal deUsr As DECustomerV11.DECustomerSiteContact, ByVal partnerStr As String) As ErrorObj
        Dim err As New ErrorObj

        Const deleteStr As String = " DELETE FROM [tbl_authorized_users] " & _
                                    " WHERE [BUSINESS_UNIT] = @BUSINESS_UNIT " & _
                                    " AND [PARTNER] = @PARTNER " & _
                                    " AND [LOGINID] = @LOGINID " & _
                                    " "

        cmd.CommandText = deleteStr

        With cmd.Parameters
            .Clear()
            .Add("@BUSINESS_UNIT", SqlDbType.NVarChar).Value = DeV11.BusinessUnit
            .Add("@PARTNER", SqlDbType.NVarChar).Value = partnerStr
            .Add("@LOGINID", SqlDbType.NVarChar).Value = deUsr.LoginID
        End With

        Try
            cmd.ExecuteNonQuery()
        Catch ex As Exception
            Const strError As String = "Error Deleteing Authorised Users Record "
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBUDAU-01"
                .HasError = True
            End With
        End Try

        Return err
    End Function

    Protected Function DeletePartnerUserSql2005(ByVal cmd As SqlCommand, ByVal deUsr As DECustomerV11.DECustomerSiteContact, ByVal partnerStr As String) As ErrorObj
        Dim err As New ErrorObj

        Const deleteStr As String = " DELETE FROM [tbl_partner_user] " & _
                                    " WHERE [PARTNER] = @PARTNER " & _
                                    " AND   [LOGINID] = @LOGINID "


        cmd.CommandText = deleteStr

        With cmd.Parameters
            .Clear()
            .Add("@PARTNER", SqlDbType.NVarChar).Value = partnerStr
            .Add("@LOGINID", SqlDbType.NVarChar).Value = deUsr.LoginID
        End With

        Try
            cmd.ExecuteNonQuery()
        Catch ex As Exception
            Const strError As String = "Error Deleting Partner User Record "
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBDU-01"
                .HasError = True
            End With
        End Try

        Return err
    End Function

    Protected Function UpdatePartnerSql2005(ByVal cmd As SqlCommand, ByVal dePart As DECustomerV11.DECustomerSite) As ErrorObj
        Dim err As New ErrorObj

        Const updateStr As String = " UPDATE [tbl_partner] " & _
                                    "   SET [PARTNER] = @PARTNER " & _
                                    "      ,[PARTNER_DESC] = @PARTNER_DESC " & _
                                    "      ,[DESTINATION_DATABASE] = @DESTINATION_DATABASE " & _
                                    "      ,[CACHEING_ENABLED] = @CACHEING_ENABLED " & _
                                    "      ,[CACHE_TIME_MINUTES] = @CACHE_TIME_MINUTES " & _
                                    "      ,[LOGGING_ENABLED] = @LOGGING_ENABLED " & _
                                    "      ,[STORE_XML] = @STORE_XML " & _
                                    "      ,[ACCOUNT_NO_1] = @ACCOUNT_NO_1 " & _
                                    "      ,[ACCOUNT_NO_2] = @ACCOUNT_NO_2 " & _
                                    "      ,[ACCOUNT_NO_3] = @ACCOUNT_NO_3 " & _
                                    "      ,[ACCOUNT_NO_4] = @ACCOUNT_NO_4 " & _
                                    "      ,[ACCOUNT_NO_5] = @ACCOUNT_NO_5 " & _
                                    "      ,[EMAIL] = @EMAIL " & _
                                    "      ,[TELEPHONE_NUMBER] = @TELEPHONE_NUMBER " & _
                                    "      ,[FAX_NUMBER] = @FAX_NUMBER " & _
                                    "      ,[PARTNER_URL] = @PARTNER_URL " & _
                                    "      ,[PARTNER_NUMBER] = @PARTNER_NUMBER " & _
                                    "      ,[ORIGINATING_BUSINESS_UNIT] = @ORIGINATING_BUSINESS_UNIT " & _
                                    "      ,[CRM_BRANCH] = @CRM_BRANCH " & _
                                    "      ,[VAT_NUMBER] = @VAT_NUMBER " & _
                                    " WHERE [PARTNER] = @PARTNER "

        cmd.CommandText = updateStr

        With cmd.Parameters
            .Clear()
            .Add("@PARTNER", SqlDbType.NVarChar).Value = dePart.Name
            .Add("@PARTNER_DESC", SqlDbType.NVarChar).Value = ""
            .Add("@DESTINATION_DATABASE", SqlDbType.NVarChar).Value = ""
            .Add("@CACHEING_ENABLED", SqlDbType.Bit).Value = True
            .Add("@CACHE_TIME_MINUTES", SqlDbType.Int).Value = 30
            .Add("@LOGGING_ENABLED", SqlDbType.Bit).Value = True
            .Add("@STORE_XML", SqlDbType.Bit).Value = False
            .Add("@ACCOUNT_NO_1", SqlDbType.NVarChar).Value = dePart.AccountNumber1
            .Add("@ACCOUNT_NO_2", SqlDbType.NVarChar).Value = dePart.AccountNumber2
            .Add("@ACCOUNT_NO_3", SqlDbType.NVarChar).Value = dePart.AccountNumber3
            .Add("@ACCOUNT_NO_4", SqlDbType.NVarChar).Value = dePart.AccountNumber4
            .Add("@ACCOUNT_NO_5", SqlDbType.NVarChar).Value = dePart.AccountNumber5
            .Add("@EMAIL", SqlDbType.NVarChar).Value = ""
            .Add("@TELEPHONE_NUMBER", SqlDbType.NVarChar).Value = dePart.TelephoneNumber
            .Add("@FAX_NUMBER", SqlDbType.NVarChar).Value = dePart.FaxNumber
            .Add("@PARTNER_URL", SqlDbType.NVarChar).Value = dePart.URL
            .Add("@PARTNER_NUMBER", SqlDbType.NVarChar).Value = dePart.ID
            .Add("@ORIGINATING_BUSINESS_UNIT", SqlDbType.NVarChar).Value = DeV11.BusinessUnit
            .Add("@CRM_BRANCH", SqlDbType.NVarChar).Value = dePart.CRMBranch
            .Add("@VAT_NUMBER", SqlDbType.NVarChar).Value = dePart.VATNumber
        End With

        Try
            cmd.ExecuteNonQuery()
        Catch ex As Exception
            Const strError As String = "Error Updating Partner Record "
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBUP-01"
                .HasError = True
            End With
        End Try

        Return err
    End Function

    Protected Function UpdateAddressSql2005(ByVal cmd As SqlCommand, ByVal deAdd As DECustomerV11.DECustomerSiteAddress, ByVal partnerStr As String, ByVal loginID As String, ByVal isPartner As Boolean) As ErrorObj
        Dim err As New ErrorObj

        Const updateStr As String = " UPDATE [tbl_address] " & _
                                    "   SET [PARTNER] = @PARTNER " & _
                                    "      ,[LOGINID] = @LOGINID " & _
                                    "      ,[TYPE] = @TYPE " & _
                                    "      ,[REFERENCE] = @REFERENCE " & _
                                    "      ,[SEQUENCE] = @SEQUENCE " & _
                                    "      ,[DEFAULT_ADDRESS] = @DEFAULT_ADDRESS " & _
                                    "      ,[ADDRESS_LINE_1] = @ADDRESS_LINE_1 " & _
                                    "      ,[ADDRESS_LINE_2] = @ADDRESS_LINE_2 " & _
                                    "      ,[ADDRESS_LINE_3] = @ADDRESS_LINE_3 " & _
                                    "      ,[ADDRESS_LINE_4] = @ADDRESS_LINE_4 " & _
                                    "      ,[ADDRESS_LINE_5] = @ADDRESS_LINE_5 " & _
                                    "      ,[POST_CODE] = @POST_CODE " & _
                                    "      ,[COUNTRY] = @COUNTRY " & _
                                    " WHERE [PARTNER] = @PARTNER " & _
                                    " AND {0} "

        cmd.CommandText = updateStr

        If isPartner Then
            cmd.CommandText = String.Format(cmd.CommandText, "(LOGINID = '' OR LOGINID IS NULL)")
        Else
            cmd.CommandText = String.Format(cmd.CommandText, "LOGINID = @LOGINID")
        End If

        With cmd.Parameters
            .Clear()
            .Add("@PARTNER", SqlDbType.NVarChar).Value = partnerStr
            .Add("@LOGINID", SqlDbType.NVarChar).Value = loginID
            .Add("@TYPE", SqlDbType.NVarChar).Value = ""
            .Add("@REFERENCE", SqlDbType.NVarChar).Value = deAdd.Line1 & " " & deAdd.Postcode
            .Add("@SEQUENCE", SqlDbType.Int).Value = deAdd.SequenceNumber
            .Add("@DEFAULT_ADDRESS", SqlDbType.NVarChar).Value = deAdd.IsDefault
            .Add("@ADDRESS_LINE_1", SqlDbType.NVarChar).Value = deAdd.Line1
            .Add("@ADDRESS_LINE_2", SqlDbType.NVarChar).Value = deAdd.Line2
            .Add("@ADDRESS_LINE_3", SqlDbType.NVarChar).Value = deAdd.Line3
            .Add("@ADDRESS_LINE_4", SqlDbType.NVarChar).Value = deAdd.Line4
            .Add("@ADDRESS_LINE_5", SqlDbType.NVarChar).Value = deAdd.Line5
            .Add("@POST_CODE", SqlDbType.NVarChar).Value = deAdd.Postcode
            .Add("@COUNTRY", SqlDbType.NVarChar).Value = deAdd.Country

        End With

        Try
            cmd.ExecuteNonQuery()
        Catch ex As Exception
            Const strError As String = "Error Updating Address Record "
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBUA-01"
                .HasError = True
            End With
        End Try

        Return err
    End Function

    Protected Function UpdateAuthorisedUserSql2005(ByVal cmd As SqlCommand, ByVal deUsr As DECustomerV11.DECustomerSiteContact, ByVal partnerStr As String) As ErrorObj
        Dim err As New ErrorObj

        Const updateStr As String = " UPDATE [tbl_authorized_users] " & _
                                    "   SET [PASSWORD] @PASSWORD " & _
                                    " WHERE [BUSINESS_UNIT] = @BUSINESS_UNIT " & _
                                    " AND [PARTNER] = @PARTNER " & _
                                    " AND [LOGINID] = @LOGINID " & _
                                    " "

        cmd.CommandText = updateStr

        With cmd.Parameters
            .Clear()
            .Add("@BUSINESS_UNIT", SqlDbType.NVarChar).Value = DeV11.BusinessUnit
            .Add("@PARTNER", SqlDbType.NVarChar).Value = partnerStr
            .Add("@LOGINID", SqlDbType.NVarChar).Value = deUsr.LoginID
            .Add("@PASSWORD", SqlDbType.NVarChar).Value = deUsr.Password
        End With

        Try
            cmd.ExecuteNonQuery()
        Catch ex As Exception
            Const strError As String = "Error Updating Authorised Users Record "
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBUAU-01"
                .HasError = True
            End With
        End Try

        Return err
    End Function

    Protected Function UpdatePartnerUserSql2005(ByVal cmd As SqlCommand, ByVal deUsr As DECustomerV11.DECustomerSiteContact, ByVal partnerStr As String) As ErrorObj
        Dim err As New ErrorObj

        Const updateStr As String = " UPDATE [tbl_partner_user] " & _
                                    "   SET [PARTNER] = @PARTNER " & _
                                    "      ,[LOGINID] = @LOGINID " & _
                                    "      ,[EMAIL] = @EMAIL " & _
                                    "      ,[TITLE] = @TITLE " & _
                                    "      ,[INITIALS] = @INITIALS " & _
                                    "      ,[FORENAME] = @FORENAME " & _
                                    "      ,[SURNAME] = @SURNAME " & _
                                    "      ,[FULL_NAME] = @FULL_NAME " & _
                                    "      ,[SALUTATION] = @SALUTATION " & _
                                    "      ,[POSITION] = @POSITION " & _
                                    "      ,[DOB] = @DOB " & _
                                    "      ,[MOBILE_NUMBER] = @MOBILE_NUMBER " & _
                                    "      ,[TELEPHONE_NUMBER] = @TELEPHONE_NUMBER " & _
                                    "      ,[WORK_NUMBER] = @WORK_NUMBER " & _
                                    "      ,[FAX_NUMBER] = @FAX_NUMBER " & _
                                    "      ,[OTHER_NUMBER] = @OTHER_NUMBER " & _
                                    "      ,[MESSAGING_ID] = @MESSAGING_ID " & _
                                    "      ,[USER_NUMBER] = @USER_NUMBER " & _
                                    "      ,[ORIGINATING_BUSINESS_UNIT] = @ORIGINATING_BUSINESS_UNIT " & _
                                    "      ,[ACCOUNT_NO_1] = @ACCOUNT_NO_1 " & _
                                    "      ,[ACCOUNT_NO_2] = @ACCOUNT_NO_2 " & _
                                    "      ,[ACCOUNT_NO_3] = @ACCOUNT_NO_3 " & _
                                    "      ,[ACCOUNT_NO_4] = @ACCOUNT_NO_4 " & _
                                    "      ,[ACCOUNT_NO_5] = @ACCOUNT_NO_5 " & _
                                    "      ,[SUBSCRIBE_NEWSLETTER] = @SUBSCRIBE_NEWSLETTER " & _
                                    "      ,[HTML_NEWSLETTER] = @HTML_NEWSLETTER " & _
                                    "      ,[BIT0] = @BIT0 " & _
                                    "      ,[BIT1] = @BIT1 " & _
                                    "      ,[BIT2] = @BIT2 " & _
                                    "      ,[BIT3] = @BIT3 " & _
                                    "      ,[BIT4] = @BIT4 " & _
                                    "      ,[BIT5] = @BIT5 " & _
                                    "      ,[SEX] = @SEX " & _
                                    "      ,[USER_NUMBER_PREFIX] = @USER_NUMBER_PREFIX " & _
                                    "      ,[PREFERRED_BUSINESS_UNIT] = @PREFERRED_BUSINESS_UNIT " & _
                                    "      ,[PREFERRED_LANGUAGE] = @PREFERRED_LANGUAGE " & _
                                    "      ,[RESTRICTED_PAYMENT_METHOD] = @RESTRICTED_PAYMENT_METHOD " & _
                                    "      ,[SUBSCRIBE_2] = @SUBSCRIBE_2 " & _
                                    "      ,[SUBSCRIBE_3] = @SUBSCRIBE_3 " & _
                                    "      ,[TICKETING_LOYALTY_POINTS] = @TICKETING_LOYALTY_POINTS " & _
                                    "      ,[ATTRIBUTES_LIST] = @ATTRIBUTES_LIST " & _
                                    "      ,[BOND_HOLDER] = @BOND_HOLDER " & _
                                    " WHERE [PARTNER] = @PARTNER " & _
                                    " AND   [LOGINID] = @LOGINID "


        cmd.CommandText = updateStr

        With cmd.Parameters
            .Clear()
            .Add("@PARTNER", SqlDbType.NVarChar).Value = partnerStr
            .Add("@LOGINID", SqlDbType.NVarChar).Value = deUsr.LoginID
            .Add("@EMAIL", SqlDbType.NVarChar).Value = deUsr.EmailAddress
            .Add("@TITLE", SqlDbType.NVarChar).Value = deUsr.Title
            .Add("@INITIALS", SqlDbType.NVarChar).Value = deUsr.Initials
            .Add("@FORENAME", SqlDbType.NVarChar).Value = deUsr.Forename
            .Add("@SURNAME", SqlDbType.NVarChar).Value = deUsr.Surname
            .Add("@FULL_NAME", SqlDbType.NVarChar).Value = deUsr.FullName
            .Add("@SALUTATION", SqlDbType.NVarChar).Value = deUsr.Salutation
            .Add("@POSITION", SqlDbType.NVarChar).Value = deUsr.Position
            .Add("@DOB", SqlDbType.DateTime).Value = deUsr.DateOfBirth
            .Add("@MOBILE_NUMBER", SqlDbType.NVarChar).Value = deUsr.TelephoneNumber2
            .Add("@TELEPHONE_NUMBER", SqlDbType.NVarChar).Value = deUsr.TelephoneNumber1
            .Add("@WORK_NUMBER", SqlDbType.NVarChar).Value = deUsr.TelephoneNumber3
            .Add("@FAX_NUMBER", SqlDbType.NVarChar).Value = deUsr.TelephoneNumber4
            .Add("@OTHER_NUMBER", SqlDbType.NVarChar).Value = deUsr.TelephoneNumber5
            .Add("@MESSAGING_ID", SqlDbType.NVarChar).Value = deUsr.MessagingID
            If String.IsNullOrEmpty(deUsr.ID) Then deUsr.ID = 0
            .Add("@USER_NUMBER", SqlDbType.BigInt).Value = deUsr.ID
            .Add("@ORIGINATING_BUSINESS_UNIT", SqlDbType.NVarChar).Value = DeV11.BusinessUnit
            .Add("@ACCOUNT_NO_1", SqlDbType.NVarChar).Value = deUsr.AccountNumber1
            .Add("@ACCOUNT_NO_2", SqlDbType.NVarChar).Value = deUsr.AccountNumber2
            .Add("@ACCOUNT_NO_3", SqlDbType.NVarChar).Value = deUsr.AccountNumber3
            .Add("@ACCOUNT_NO_4", SqlDbType.NVarChar).Value = deUsr.AccountNumber4
            .Add("@ACCOUNT_NO_5", SqlDbType.NVarChar).Value = deUsr.AccountNumber5
            .Add("@SUBSCRIBE_NEWSLETTER", SqlDbType.Bit).Value = deUsr.Subscription1
            .Add("@HTML_NEWSLETTER", SqlDbType.Bit).Value = deUsr.HTMLNewsletter
            .Add("@BIT0", SqlDbType.Bit).Value = deUsr.ContactViaEmail
            .Add("@BIT1", SqlDbType.Bit).Value = deUsr.Boolean1
            .Add("@BIT2", SqlDbType.Bit).Value = deUsr.Boolean2
            .Add("@BIT3", SqlDbType.Bit).Value = deUsr.Boolean3
            .Add("@BIT4", SqlDbType.Bit).Value = deUsr.Boolean4
            .Add("@BIT5", SqlDbType.Bit).Value = deUsr.Boolean5
            .Add("@SEX", SqlDbType.NVarChar).Value = deUsr.Gender
            .Add("@USER_NUMBER_PREFIX", SqlDbType.NVarChar).Value = ""
            .Add("@PREFERRED_BUSINESS_UNIT", SqlDbType.NVarChar).Value = ""
            .Add("@PREFERRED_LANGUAGE", SqlDbType.NVarChar).Value = ""

            Dim rptStr As String = ""
            If deUsr.RestrictedPaymentTypes.Count > 0 Then
                For Each rpt As DECustomerV11.DECustomerSiteRestrictedPaymentType In deUsr.RestrictedPaymentTypes
                    rptStr += rpt.PaymentType & ","
                Next
                rptStr = rptStr.TrimEnd(",")
            End If

            .Add("@RESTRICTED_PAYMENT_METHOD", SqlDbType.NVarChar).Value = rptStr
            .Add("@SUBSCRIBE_2", SqlDbType.Bit).Value = deUsr.Subscription2
            .Add("@SUBSCRIBE_3", SqlDbType.Bit).Value = deUsr.Subscription3
            If String.IsNullOrEmpty(deUsr.LoyaltyPoints) Then deUsr.LoyaltyPoints = 0
            .Add("@TICKETING_LOYALTY_POINTS", SqlDbType.BigInt).Value = deUsr.LoyaltyPoints

            Dim attStr As String = ""
            If deUsr.RestrictedPaymentTypes.Count > 0 Then
                For Each att As DECustomerV11.DECustomerSiteAttributes In deUsr.Attributes
                    attStr += att.Attribute & ","
                Next
                attStr = attStr.TrimEnd(",")
            End If

            .Add("@ATTRIBUTES_LIST", SqlDbType.NVarChar).Value = attStr
            .Add("@BOND_HOLDER", SqlDbType.Bit).Value = False

        End With

        Try
            cmd.ExecuteNonQuery()
        Catch ex As Exception
            Const strError As String = "Error Updating Partner User Record "
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBUU-01"
                .HasError = True
            End With
        End Try

        Return err
    End Function

    Protected Function AddPartnerSql2005(ByVal cmd As SqlCommand, ByVal dePart As DECustomerV11.DECustomerSite) As ErrorObj
        Dim err As New ErrorObj

        Const insertStr As String = " IF NOT EXISTS (SELECT * FROM tbl_partner WHERE [PARTNER] = @PARTNER) " & _
                                    " BEGIN " & _
                                    " INSERT INTO [tbl_partner] " & _
                                    "            ([PARTNER] " & _
                                    "            ,[PARTNER_DESC] " & _
                                    "            ,[DESTINATION_DATABASE] " & _
                                    "            ,[CACHEING_ENABLED] " & _
                                    "            ,[CACHE_TIME_MINUTES] " & _
                                    "            ,[LOGGING_ENABLED] " & _
                                    "            ,[STORE_XML] " & _
                                    "            ,[ACCOUNT_NO_1] " & _
                                    "            ,[ACCOUNT_NO_2] " & _
                                    "            ,[ACCOUNT_NO_3] " & _
                                    "            ,[ACCOUNT_NO_4] " & _
                                    "            ,[ACCOUNT_NO_5] " & _
                                    "            ,[EMAIL] " & _
                                    "            ,[TELEPHONE_NUMBER] " & _
                                    "            ,[FAX_NUMBER] " & _
                                    "            ,[PARTNER_URL] " & _
                                    "            ,[PARTNER_NUMBER] " & _
                                    "            ,[ORIGINATING_BUSINESS_UNIT] " & _
                                    "            ,[CRM_BRANCH] " & _
                                    "            ,[VAT_NUMBER]) " & _
                                    "      VALUES " & _
                                    "            (@PARTNER " & _
                                    "            ,@PARTNER_DESC " & _
                                    "            ,@DESTINATION_DATABASE " & _
                                    "            ,@CACHEING_ENABLED " & _
                                    "            ,@CACHE_TIME_MINUTES " & _
                                    "            ,@LOGGING_ENABLED " & _
                                    "            ,@STORE_XML " & _
                                    "            ,@ACCOUNT_NO_1 " & _
                                    "            ,@ACCOUNT_NO_2 " & _
                                    "            ,@ACCOUNT_NO_3 " & _
                                    "            ,@ACCOUNT_NO_4 " & _
                                    "            ,@ACCOUNT_NO_5 " & _
                                    "            ,@EMAIL " & _
                                    "            ,@TELEPHONE_NUMBER " & _
                                    "            ,@FAX_NUMBER " & _
                                    "            ,@PARTNER_URL " & _
                                    "            ,@PARTNER_NUMBER " & _
                                    "            ,@ORIGINATING_BUSINESS_UNIT " & _
                                    "            ,@CRM_BRANCH " & _
                                    "            ,@VAT_NUMBER) " & _
                                    " END "

        cmd.CommandText = insertStr

        With cmd.Parameters
            .Clear()
            .Add("@PARTNER", SqlDbType.NVarChar).Value = dePart.Name
            .Add("@PARTNER_DESC", SqlDbType.NVarChar).Value = ""
            .Add("@DESTINATION_DATABASE", SqlDbType.NVarChar).Value = ""
            .Add("@CACHEING_ENABLED", SqlDbType.Bit).Value = True
            .Add("@CACHE_TIME_MINUTES", SqlDbType.Int).Value = 30
            .Add("@LOGGING_ENABLED", SqlDbType.Bit).Value = True
            .Add("@STORE_XML", SqlDbType.Bit).Value = False
            .Add("@ACCOUNT_NO_1", SqlDbType.NVarChar).Value = dePart.AccountNumber1
            .Add("@ACCOUNT_NO_2", SqlDbType.NVarChar).Value = dePart.AccountNumber2
            .Add("@ACCOUNT_NO_3", SqlDbType.NVarChar).Value = dePart.AccountNumber3
            .Add("@ACCOUNT_NO_4", SqlDbType.NVarChar).Value = dePart.AccountNumber4
            .Add("@ACCOUNT_NO_5", SqlDbType.NVarChar).Value = dePart.AccountNumber5
            .Add("@EMAIL", SqlDbType.NVarChar).Value = ""
            .Add("@TELEPHONE_NUMBER", SqlDbType.NVarChar).Value = dePart.TelephoneNumber
            .Add("@FAX_NUMBER", SqlDbType.NVarChar).Value = dePart.FaxNumber
            .Add("@PARTNER_URL", SqlDbType.NVarChar).Value = dePart.URL
            .Add("@PARTNER_NUMBER", SqlDbType.NVarChar).Value = dePart.ID
            .Add("@ORIGINATING_BUSINESS_UNIT", SqlDbType.NVarChar).Value = DeV11.BusinessUnit
            .Add("@CRM_BRANCH", SqlDbType.NVarChar).Value = dePart.CRMBranch
            .Add("@VAT_NUMBER", SqlDbType.NVarChar).Value = dePart.VATNumber
        End With

        Try
            cmd.ExecuteNonQuery()
        Catch ex As Exception
            Const strError As String = "Error adding Partner Record "
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBAP-01"
                .HasError = True
            End With
        End Try

        Return err
    End Function

    Protected Function AddAddressSql2005(ByVal cmd As SqlCommand, ByVal deAdd As DECustomerV11.DECustomerSiteAddress, ByVal partnerStr As String, ByVal loginID As String, ByVal isPartner As Boolean) As ErrorObj
        Dim err As New ErrorObj

        Const insertStr As String = " IF NOT EXISTS (SELECT * FROM tbl_address WHERE PARTNER = @PARTNER " & _
                                    " AND {0}) " & _
                                    " BEGIN " & _
                                    " INSERT INTO [tbl_address] " & _
                                    "            ([PARTNER] " & _
                                    "           ,[LOGINID] " & _
                                    "           ,[TYPE] " & _
                                    "           ,[REFERENCE] " & _
                                    "           ,[SEQUENCE] " & _
                                    "           ,[DEFAULT_ADDRESS] " & _
                                    "           ,[ADDRESS_LINE_1] " & _
                                    "           ,[ADDRESS_LINE_2] " & _
                                    "           ,[ADDRESS_LINE_3] " & _
                                    "           ,[ADDRESS_LINE_4] " & _
                                    "           ,[ADDRESS_LINE_5] " & _
                                    "           ,[POST_CODE] " & _
                                    "           ,[COUNTRY]) " & _
                                    "     VALUES " & _
                                    "            (@PARTNER " & _
                                    "           ,@LOGINID " & _
                                    "           ,@TYPE " & _
                                    "           ,@REFERENCE " & _
                                    "           ,@SEQUENCE " & _
                                    "           ,@DEFAULT_ADDRESS " & _
                                    "           ,@ADDRESS_LINE_1 " & _
                                    "           ,@ADDRESS_LINE_2 " & _
                                    "           ,@ADDRESS_LINE_3 " & _
                                    "           ,@ADDRESS_LINE_4 " & _
                                    "           ,@ADDRESS_LINE_5 " & _
                                    "           ,@POST_CODE " & _
                                    "           ,@COUNTRY) " & _
                                    " END "

        cmd.CommandText = insertStr

        If isPartner Then
            cmd.CommandText = String.Format(cmd.CommandText, "(LOGINID = '' OR LOGINID IS NULL)")
        Else
            cmd.CommandText = String.Format(cmd.CommandText, "LOGINID = @LOGINID")
        End If

        With cmd.Parameters
            .Clear()
            .Add("@PARTNER", SqlDbType.NVarChar).Value = partnerStr
            .Add("@LOGINID", SqlDbType.NVarChar).Value = loginID
            .Add("@TYPE", SqlDbType.NVarChar).Value = ""
            .Add("@REFERENCE", SqlDbType.NVarChar).Value = deAdd.Line1 & " " & deAdd.Postcode
            .Add("@SEQUENCE", SqlDbType.Int).Value = deAdd.SequenceNumber
            .Add("@DEFAULT_ADDRESS", SqlDbType.NVarChar).Value = deAdd.IsDefault
            .Add("@ADDRESS_LINE_1", SqlDbType.NVarChar).Value = deAdd.Line1
            .Add("@ADDRESS_LINE_2", SqlDbType.NVarChar).Value = deAdd.Line2
            .Add("@ADDRESS_LINE_3", SqlDbType.NVarChar).Value = deAdd.Line3
            .Add("@ADDRESS_LINE_4", SqlDbType.NVarChar).Value = deAdd.Line4
            .Add("@ADDRESS_LINE_5", SqlDbType.NVarChar).Value = deAdd.Line5
            .Add("@POST_CODE", SqlDbType.NVarChar).Value = deAdd.Postcode
            .Add("@COUNTRY", SqlDbType.NVarChar).Value = deAdd.Country

        End With

        Try
            cmd.ExecuteNonQuery()
        Catch ex As Exception
            Const strError As String = "Error adding Address Record "
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBAA-01"
                .HasError = True
            End With
        End Try

        Return err
    End Function

    Protected Function AddAuthorisedUserSql2005(ByVal cmd As SqlCommand, ByVal deUsr As DECustomerV11.DECustomerSiteContact, ByVal partnerStr As String) As ErrorObj
        Dim err As New ErrorObj

        Const insertStr As String = " IF NOT EXISTS (SELECT * FROM tbl_authorized_users WHERE LOGINID = @LOGINID AND PARTNER = @PARTNER AND BUSINESS_UNIT = @BUSINESS_UNIT) " & _
                                    " BEGIN " & _
                                    " INSERT INTO [tbl_authorized_users] " & _
                                    "            ([BUSINESS_UNIT] " & _
                                    "            ,[PARTNER] " & _
                                    "            ,[LOGINID] " & _
                                    "            ,[PASSWORD] " & _
                                    "            ,[AUTO_PROCESS_DEFAULT_USER] " & _
                                    "            ,[IS_APPROVED] " & _
                                    "            ,[IS_LOCKED_OUT] " & _
                                    "            ,[CREATED_DATE] " & _
                                    "            ,[LAST_LOGIN_DATE] " & _
                                    "            ,[LAST_PASSWORD_CHANGED_DATE] " & _
                                    "            ,[LAST_LOCKED_OUT_DATE]) " & _
                                    "      VALUES " & _
                                    "            (@BUSINESS_UNIT " & _
                                    "            ,@PARTNER " & _
                                    "            ,@LOGINID " & _
                                    "            ,@PASSWORD " & _
                                    "            ,@AUTO_PROCESS_DEFAULT_USER " & _
                                    "            ,@IS_APPROVED " & _
                                    "            ,@IS_LOCKED_OUT " & _
                                    "            ,@CREATED_DATE " & _
                                    "            ,@LAST_LOGIN_DATE " & _
                                    "            ,@LAST_PASSWORD_CHANGED_DATE " & _
                                    "            ,@LAST_LOCKED_OUT_DATE) " & _
                                    " END "

        cmd.CommandText = insertStr

        With cmd.Parameters
            .Clear()
            .Add("@BUSINESS_UNIT", SqlDbType.NVarChar).Value = DeV11.BusinessUnit
            .Add("@PARTNER", SqlDbType.NVarChar).Value = partnerStr
            .Add("@LOGINID", SqlDbType.NVarChar).Value = deUsr.LoginID
            .Add("@PASSWORD", SqlDbType.NVarChar).Value = deUsr.Password
            .Add("@AUTO_PROCESS_DEFAULT_USER", SqlDbType.Bit).Value = False
            .Add("@IS_APPROVED", SqlDbType.Bit).Value = False
            .Add("@IS_LOCKED_OUT", SqlDbType.Bit).Value = deUsr.IsLockedOut
            .Add("@CREATED_DATE", SqlDbType.DateTime).Value = Now
            .Add("@LAST_LOGIN_DATE", SqlDbType.DateTime).Value = Now
            .Add("@LAST_PASSWORD_CHANGED_DATE", SqlDbType.DateTime).Value = Now
            .Add("@LAST_LOCKED_OUT_DATE", SqlDbType.DateTime).Value = Now
        End With

        Try
            cmd.ExecuteNonQuery()
        Catch ex As Exception
            Const strError As String = "Error adding Authorised Users Record "
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBAU-01"
                .HasError = True
            End With
        End Try

        Return err
    End Function

    Protected Function AddPartnerUserSql2005(ByVal cmd As SqlCommand, ByVal deUsr As DECustomerV11.DECustomerSiteContact, ByVal partnerStr As String) As ErrorObj
        Dim err As New ErrorObj

        Const insertStr As String = " IF NOT EXISTS (SELECT * FROM tbl_partner_user WHERE PARTNER = @PARTNER AND LOGINID = @LOGINID) " & _
                                    " BEGIN " & _
                                    " INSERT INTO [tbl_partner_user] " & _
                                    "            ([PARTNER] " & _
                                    "            ,[LOGINID] " & _
                                    "            ,[EMAIL] " & _
                                    "            ,[TITLE] " & _
                                    "            ,[INITIALS] " & _
                                    "            ,[FORENAME] " & _
                                    "            ,[SURNAME] " & _
                                    "            ,[FULL_NAME] " & _
                                    "            ,[SALUTATION] " & _
                                    "            ,[POSITION] " & _
                                    "            ,[DOB] " & _
                                    "            ,[MOBILE_NUMBER] " & _
                                    "            ,[TELEPHONE_NUMBER] " & _
                                    "            ,[WORK_NUMBER] " & _
                                    "            ,[FAX_NUMBER] " & _
                                    "            ,[OTHER_NUMBER] " & _
                                    "            ,[MESSAGING_ID] " & _
                                    "            ,[USER_NUMBER] " & _
                                    "            ,[ORIGINATING_BUSINESS_UNIT] " & _
                                    "            ,[ACCOUNT_NO_1] " & _
                                    "            ,[ACCOUNT_NO_2] " & _
                                    "            ,[ACCOUNT_NO_3] " & _
                                    "            ,[ACCOUNT_NO_4] " & _
                                    "            ,[ACCOUNT_NO_5] " & _
                                    "            ,[SUBSCRIBE_NEWSLETTER] " & _
                                    "            ,[HTML_NEWSLETTER] " & _
                                    "            ,[BIT0] " & _
                                    "            ,[BIT1] " & _
                                    "            ,[BIT2] " & _
                                    "            ,[BIT3] " & _
                                    "            ,[BIT4] " & _
                                    "            ,[BIT5] " & _
                                    "            ,[SEX] " & _
                                    "            ,[USER_NUMBER_PREFIX] " & _
                                    "            ,[PREFERRED_BUSINESS_UNIT] " & _
                                    "            ,[PREFERRED_LANGUAGE] " & _
                                    "            ,[RESTRICTED_PAYMENT_METHOD] " & _
                                    "            ,[SUBSCRIBE_2] " & _
                                    "            ,[SUBSCRIBE_3] " & _
                                    "            ,[TICKETING_LOYALTY_POINTS] " & _
                                    "            ,[ATTRIBUTES_LIST] " & _
                                    "            ,[BOND_HOLDER]) " & _
                                    "      VALUES " & _
                                    "            (@PARTNER " & _
                                    "            ,@LOGINID " & _
                                    "            ,@EMAIL " & _
                                    "            ,@TITLE " & _
                                    "            ,@INITIALS " & _
                                    "            ,@FORENAME " & _
                                    "            ,@SURNAME " & _
                                    "            ,@FULL_NAME " & _
                                    "            ,@SALUTATION " & _
                                    "            ,@POSITION " & _
                                    "            ,@DOB " & _
                                    "            ,@MOBILE_NUMBER " & _
                                    "            ,@TELEPHONE_NUMBER " & _
                                    "            ,@WORK_NUMBER " & _
                                    "            ,@FAX_NUMBER " & _
                                    "            ,@OTHER_NUMBER " & _
                                    "            ,@MESSAGING_ID " & _
                                    "            ,@USER_NUMBER " & _
                                    "            ,@ORIGINATING_BUSINESS_UNIT " & _
                                    "            ,@ACCOUNT_NO_1 " & _
                                    "            ,@ACCOUNT_NO_2 " & _
                                    "            ,@ACCOUNT_NO_3 " & _
                                    "            ,@ACCOUNT_NO_4 " & _
                                    "            ,@ACCOUNT_NO_5 " & _
                                    "            ,@SUBSCRIBE_NEWSLETTER " & _
                                    "            ,@HTML_NEWSLETTER " & _
                                    "            ,@BIT0 " & _
                                    "            ,@BIT1 " & _
                                    "            ,@BIT2 " & _
                                    "            ,@BIT3 " & _
                                    "            ,@BIT4 " & _
                                    "            ,@BIT5 " & _
                                    "            ,@SEX " & _
                                    "            ,@USER_NUMBER_PREFIX " & _
                                    "            ,@PREFERRED_BUSINESS_UNIT " & _
                                    "            ,@PREFERRED_LANGUAGE " & _
                                    "            ,@RESTRICTED_PAYMENT_METHOD " & _
                                    "            ,@SUBSCRIBE_2 " & _
                                    "            ,@SUBSCRIBE_3 " & _
                                    "            ,@TICKETING_LOYALTY_POINTS " & _
                                    "            ,@ATTRIBUTES_LIST " & _
                                    "            ,@BOND_HOLDER) " & _
                                    " END  "


        cmd.CommandText = insertStr

        With cmd.Parameters
            .Clear()
            .Add("@PARTNER", SqlDbType.NVarChar).Value = partnerStr
            .Add("@LOGINID", SqlDbType.NVarChar).Value = deUsr.LoginID
            .Add("@EMAIL", SqlDbType.NVarChar).Value = deUsr.EmailAddress
            .Add("@TITLE", SqlDbType.NVarChar).Value = deUsr.Title
            .Add("@INITIALS", SqlDbType.NVarChar).Value = deUsr.Initials
            .Add("@FORENAME", SqlDbType.NVarChar).Value = deUsr.Forename
            .Add("@SURNAME", SqlDbType.NVarChar).Value = deUsr.Surname
            .Add("@FULL_NAME", SqlDbType.NVarChar).Value = deUsr.FullName
            .Add("@SALUTATION", SqlDbType.NVarChar).Value = deUsr.Salutation
            .Add("@POSITION", SqlDbType.NVarChar).Value = deUsr.Position
            .Add("@DOB", SqlDbType.DateTime).Value = deUsr.DateOfBirth
            .Add("@MOBILE_NUMBER", SqlDbType.NVarChar).Value = deUsr.TelephoneNumber2
            .Add("@TELEPHONE_NUMBER", SqlDbType.NVarChar).Value = deUsr.TelephoneNumber1
            .Add("@WORK_NUMBER", SqlDbType.NVarChar).Value = deUsr.TelephoneNumber3
            .Add("@FAX_NUMBER", SqlDbType.NVarChar).Value = deUsr.TelephoneNumber4
            .Add("@OTHER_NUMBER", SqlDbType.NVarChar).Value = deUsr.TelephoneNumber5
            .Add("@MESSAGING_ID", SqlDbType.NVarChar).Value = deUsr.MessagingID
            If String.IsNullOrEmpty(deUsr.ID) Then deUsr.ID = 0
            .Add("@USER_NUMBER", SqlDbType.BigInt).Value = deUsr.ID
            .Add("@ORIGINATING_BUSINESS_UNIT", SqlDbType.NVarChar).Value = DeV11.BusinessUnit
            .Add("@ACCOUNT_NO_1", SqlDbType.NVarChar).Value = deUsr.AccountNumber1
            .Add("@ACCOUNT_NO_2", SqlDbType.NVarChar).Value = deUsr.AccountNumber2
            .Add("@ACCOUNT_NO_3", SqlDbType.NVarChar).Value = deUsr.AccountNumber3
            .Add("@ACCOUNT_NO_4", SqlDbType.NVarChar).Value = deUsr.AccountNumber4
            .Add("@ACCOUNT_NO_5", SqlDbType.NVarChar).Value = deUsr.AccountNumber5
            .Add("@SUBSCRIBE_NEWSLETTER", SqlDbType.Bit).Value = deUsr.Subscription1
            .Add("@HTML_NEWSLETTER", SqlDbType.Bit).Value = deUsr.HTMLNewsletter
            .Add("@BIT0", SqlDbType.Bit).Value = deUsr.ContactViaEmail
            .Add("@BIT1", SqlDbType.Bit).Value = deUsr.Boolean1
            .Add("@BIT2", SqlDbType.Bit).Value = deUsr.Boolean2
            .Add("@BIT3", SqlDbType.Bit).Value = deUsr.Boolean3
            .Add("@BIT4", SqlDbType.Bit).Value = deUsr.Boolean4
            .Add("@BIT5", SqlDbType.Bit).Value = deUsr.Boolean5
            .Add("@SEX", SqlDbType.NVarChar).Value = deUsr.Gender
            .Add("@USER_NUMBER_PREFIX", SqlDbType.NVarChar).Value = ""
            .Add("@PREFERRED_BUSINESS_UNIT", SqlDbType.NVarChar).Value = ""
            .Add("@PREFERRED_LANGUAGE", SqlDbType.NVarChar).Value = ""

            Dim rptStr As String = ""
            If deUsr.RestrictedPaymentTypes.Count > 0 Then
                For Each rpt As DECustomerV11.DECustomerSiteRestrictedPaymentType In deUsr.RestrictedPaymentTypes
                    rptStr += rpt.PaymentType & ","
                Next
                rptStr = rptStr.TrimEnd(",")
            End If

            .Add("@RESTRICTED_PAYMENT_METHOD", SqlDbType.NVarChar).Value = rptStr
            .Add("@SUBSCRIBE_2", SqlDbType.Bit).Value = deUsr.Subscription2
            .Add("@SUBSCRIBE_3", SqlDbType.Bit).Value = deUsr.Subscription3
            If String.IsNullOrEmpty(deUsr.LoyaltyPoints) Then deUsr.LoyaltyPoints = 0
            .Add("@TICKETING_LOYALTY_POINTS", SqlDbType.BigInt).Value = deUsr.LoyaltyPoints

            Dim attStr As String = ""
            If deUsr.RestrictedPaymentTypes.Count > 0 Then
                For Each att As DECustomerV11.DECustomerSiteAttributes In deUsr.Attributes
                    attStr += att.Attribute & ","
                Next
                attStr = attStr.TrimEnd(",")
            End If

            .Add("@ATTRIBUTES_LIST", SqlDbType.NVarChar).Value = attStr
            .Add("@BOND_HOLDER", SqlDbType.Bit).Value = False

        End With

        Try
            cmd.ExecuteNonQuery()
        Catch ex As Exception
            Const strError As String = "Error adding Partner User Record "
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBAU-01"
                .HasError = True
            End With
        End Try

        Return err
    End Function

    Protected Function AccessDatabaseWS009R(Optional ByVal parmWS009R As String = "", Optional ByVal parmWS009R2 As String = "") As ErrorObj
        Dim err As New ErrorObj
        Dim dRow As DataRow = Nothing
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & "/WS009R(@PARAM1,@PARAM2,@PARAM3)"
        Dim parmIO As iDB2Parameter
        Dim parmIO2 As iDB2Parameter
        Dim parmIO3 As iDB2Parameter
        Dim PARAMOUT As String = String.Empty
        Dim PARAMOUT2 As String = String.Empty
        Dim PARAMOUT3 As String = String.Empty
        Dim DtCustomerResults As New DataTable("CustomerResults")
        Dim DtStatusResults As New DataTable(GlobalConstants.STATUS_RESULTS_TABLE_NAME)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With
        If parmWS009R.Trim = "" Then
            ResultDataSet = New DataSet
        End If
        If Not ResultDataSet.Tables.Contains(GlobalConstants.STATUS_RESULTS_TABLE_NAME) Then ResultDataSet.Tables.Add(DtStatusResults)
        ResultDataSet.Tables.Add(DtCustomerResults)

        If Not err.HasError Then
            Try
                'Has the return parameter been passed into the function
                If parmWS009R.Trim <> "" Then
                    PARAMOUT = parmWS009R
                End If
                If parmWS009R2.Trim <> "" Then
                    PARAMOUT2 = parmWS009R2
                End If

                If parmWS009R.Trim = "" And parmWS009R2 = "" Then

                    'Set the connection string
                    cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)

                    'Populate the parameter
                    parmIO = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 1024)
                    parmIO.Value = WS009Parm()
                    parmIO.Direction = ParameterDirection.InputOutput
                    parmIO2 = cmdSELECT.Parameters.Add(Param2, iDB2DbType.iDB2Char, 1024)
                    parmIO2.Value = WS009Parm2()
                    parmIO2.Direction = ParameterDirection.InputOutput
                    parmIO3 = cmdSELECT.Parameters.Add(Param3, iDB2DbType.iDB2Char, 6000)
                    parmIO3.Value = WS009Parm3()
                    parmIO3.Direction = ParameterDirection.InputOutput

                    'Execute
                    TalentCommonLog("AccessDatabaseWS009R", _de.CustomerNumber, "Backend Request: strHEADER=" & strHEADER & ", parmIO.Value=" & parmIO.Value & ", parmIO2.Value=" & parmIO2.Value)
                    cmdSELECT.ExecuteNonQuery()
                    PARAMOUT = cmdSELECT.Parameters(Param1).Value.ToString
                    PARAMOUT2 = cmdSELECT.Parameters(Param2).Value.ToString
                    PARAMOUT3 = cmdSELECT.Parameters(Param3).Value.ToString
                    TalentCommonLog("AccessDatabaseWS009R", _de.CustomerNumber, "Backend Response: PARAMOUT=" & PARAMOUT & ", PARAMOUT2=" & PARAMOUT2)

                    'Set the response data 
                    dRow = DtStatusResults.NewRow
                    ' Don't count Internet Ready error as a fail (IR) - Only for WS009
                    If (PARAMOUT.Substring(1023, 1) = GlobalConstants.ERRORFLAG Or PARAMOUT.Substring(1021, 2).Trim <> "") AndAlso (PARAMOUT.Substring(1021, 2).Trim <> "IR") Then
                        dRow("ErrorOccurred") = GlobalConstants.ERRORFLAG
                        dRow("ReturnCode") = PARAMOUT.Substring(1021, 2)
                        _de.ErrorFlag = GlobalConstants.ERRORFLAG
                        _de.ErrorCode = PARAMOUT.Substring(1021, 2)
                    Else
                        dRow("ErrorOccurred") = ""
                        dRow("ReturnCode") = ""
                        If Not String.IsNullOrEmpty(PARAMOUT.Substring(1002, 12)) Then
                            _de.CustomerNumber = PARAMOUT.Substring(1002, 12)
                        End If
                    End If
                    DtStatusResults.Rows.Add(dRow)
                End If

                If PARAMOUT.Substring(1023, 1) <> GlobalConstants.ERRORFLAG OrElse (PARAMOUT.Substring(1021, 2).Trim = "IR") Then
                    ' No errors - Build return dataset 
                    AddCustomerRetrievalColumns(DtCustomerResults)
                    dRow = Nothing
                    dRow = DtCustomerResults.NewRow
                    WS009Values(_de, PARAMOUT, PARAMOUT2, PARAMOUT3, dRow)
                    DtCustomerResults.Rows.Add(dRow)
                End If

            Catch ex As Exception
                Const strError As String = "Error during database access"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TACDBCC-02"
                    .HasError = True
                End With
            End Try
        End If
        Return err
    End Function


    Protected Function VerifyPasswordSql2005() As ErrorObj
        Dim err As New ErrorObj
        ResultDataSet = New DataSet
        Dim DtVerifyPasswordResults As New DataTable("VerifyPasswordResults")
        With DtVerifyPasswordResults.Columns
            .Add("UserName", GetType(String))
            .Add("Success", GetType(Boolean))
            .Add("Password", GetType(String))
        End With
        Dim DtStatusResults As New DataTable(GlobalConstants.STATUS_RESULTS_TABLE_NAME)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With
        ResultDataSet.Tables.Add(DtStatusResults)
        ResultDataSet.Tables.Add(DtVerifyPasswordResults)

        Dim SQL As New StringBuilder
        If DeV11.DECustomersV1(0).EmailAddress.Length > 0 Then
            SQL.Append("SELECT AU.[LOGINID] AS UserName, AU.[PASSWORD] AS Password ")
            SQL.Append("FROM [tbl_authorized_users] AU, [tbl_partner_user] P ")
            SQL.Append("WHERE P.[LOGINID] = AU.[LOGINID] ")
            SQL.Append("AND AU.[PASSWORD] = @Password ")
            SQL.Append("AND P.[EMAIL] = @Email")
        Else
            SQL.Append("SELECT [LOGINID] AS UserName, [PASSWORD] AS Password ")
            SQL.Append("FROM [tbl_authorized_users] ")
            SQL.Append("WHERE [PASSWORD] = @Password ")
            SQL.Append("AND [LOGINID] = @LoginID")
        End If
        Dim cmd As New SqlCommand(SQL.ToString, conSql2005)
        With cmd.Parameters
            If DeV11.DECustomersV1(0).UseEncryptedPassword Then
                .Add("@Password", SqlDbType.NVarChar).Value = DeV11.DECustomersV1(0).HashedPassword
            Else
                .Add("@Password", SqlDbType.NVarChar).Value = DeV11.DECustomersV1(0).Password
            End If
            If DeV11.DECustomersV1(0).EmailAddress.Length > 0 Then
                .Add("@Email", SqlDbType.NVarChar).Value = DeV11.DECustomersV1(0).EmailAddress
            Else
                .Add("@LoginID", SqlDbType.NVarChar).Value = DeV11.DECustomersV1(0).UserName
            End If
        End With

        Try
            Dim da As New SqlDataAdapter(cmd)
            Dim ds As New DataSet()
            da.Fill(ds)
            If ds.Tables.Count > 0 Then
                Dim dt As DataTable = ds.Tables(0)
                If dt.Rows.Count > 0 Then
                    Dim dr As DataRow = DtVerifyPasswordResults.NewRow()
                    dr("UserName") = dt.Rows(0)("UserName")
                    dr("Success") = True
                    dr("Password") = dt.Rows(0)("Password")
                    DtVerifyPasswordResults.Rows.Add(dr)
                Else
                    Dim dr As DataRow = DtStatusResults.NewRow()
                    dr("ErrorOccurred") = GlobalConstants.ERRORFLAG
                    dr("ReturnCode") = "PW"
                    DtStatusResults.Rows.Add(dr)
                End If
            Else
                Dim dr As DataRow = DtStatusResults.NewRow()
                dr("ErrorOccurred") = GlobalConstants.ERRORFLAG
                dr("ReturnCode") = "PW"
                DtStatusResults.Rows.Add(dr)
            End If
        Catch ex As Exception
            Const strError As String = "Error Retrieving Authorised Users' password "
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBUDAU-99"
                .HasError = True
            End With
        End Try

        Return err
    End Function

#End Region

    Private Sub AddPasswordResultsColumns(ByRef dtPasswordResults As DataTable)
        With dtPasswordResults.Columns
            .Add("UserName", GetType(String))
            .Add("Password", GetType(String))
        End With
    End Sub

    Public Sub AddCustomerRetrievalColumns(ByRef dtCustomerRetrieval As Data.DataTable)
        With dtCustomerRetrieval.Columns
            .Add("CustomerNumber", GetType(String))
            .Add("ContactTitle", GetType(String))
            .Add("ContactInitials", GetType(String))
            .Add("ContactForename", GetType(String))
            .Add("ContactSurname", GetType(String))
            .Add("Salutation", GetType(String))
            .Add("PositionInCompany", GetType(String))
            .Add("AddressLine1", GetType(String))
            .Add("AddressLine2", GetType(String))
            .Add("AddressLine3", GetType(String))
            .Add("AddressLine4", GetType(String))
            .Add("AddressLine5", GetType(String))
            .Add("PostCode", GetType(String))
            .Add("Gender", GetType(String))
            .Add("HomeTelephoneNumber", GetType(String))
            .Add("WorkTelephoneNumber", GetType(String))
            .Add("MobileNumber", GetType(String))
            .Add("EmailAddress", GetType(String))
            .Add("DateBirth", GetType(String))
            .Add("ContactViaMail", GetType(String))
            .Add("Subscription1", GetType(String))
            .Add("Subscription2", GetType(String))
            .Add("Subscription3", GetType(String))
            .Add("ContactViaMail1", GetType(String))
            .Add("ContactViaMail2", GetType(String))
            .Add("ContactViaMail3", GetType(String))
            .Add("ContactViaMail4", GetType(String))
            .Add("ContactViaMail5", GetType(String))
            .Add("ExternalId1", GetType(String))
            .Add("ExternalId2", GetType(String))
            .Add("PasswordHint", GetType(String))
            .Add("EncryptedPassword", GetType(String))
            .Add("OldPassword", GetType(String))
            .Add("ATSReady", GetType(String))
            .Add("PriceBand", GetType(String))
            .Add("LoyaltyPoints", GetType(String))
            .Add("BondHolder", GetType(String))
            .Add("OwnsAutoMembership", GetType(String))
            .Add("WebReady", GetType(String))
            .Add("PassportNumber", GetType(String))
            .Add("GreenCardNumber", GetType(String))
            .Add("PINNumber", GetType(String))
            .Add("STHolder", GetType(String))
            .Add("SCHolder", GetType(String))
            .Add("FavouriteSeat", GetType(String))
            .Add("ContactbyPost", GetType(Boolean))
            .Add("ContactbyTelephoneHome", GetType(Boolean))
            .Add("ContactbyTelephoneWork", GetType(Boolean))
            .Add("ContactbyMobile", GetType(Boolean))
            .Add("ContactbyEmail", GetType(Boolean))
            .Add("StopCode", GetType(String))
            .Add("BookNumber", GetType(String))
            .Add("PaymentReference", GetType(String)) ' Returned whe search on Corporate Sale ID


            'PARAMOUT2
            .Add("UserID4", GetType(String))
            .Add("UserID5", GetType(String))
            .Add("UserID6", GetType(String))
            .Add("UserID7", GetType(String))
            .Add("UserID8", GetType(String))
            .Add("UserID9", GetType(String))
            .Add("FanFlag", GetType(String))
            .Add("EmergencyName", GetType(String))
            .Add("EmergencyNumber", GetType(String))
            .Add("MedicalInfo", GetType(String))
            .Add("SmartcardNumber", GetType(String))
            .Add("NickName", GetType(String))
            .Add("AltUserName", GetType(String))
            .Add("Suffix", GetType(String))
            .Add("SLNumber", GetType(String))
            .Add("CompanyNumber", GetType(String))
            .Add("CompanyName", GetType(String))
            .Add("CRMCompanyName", GetType(String))
            .Add("ConsentStatus", GetType(String))
            .Add("ParentPhone", GetType(String))
            .Add("ParentEmail", GetType(String))
        End With
    End Sub

    Public Sub AddWS121Columns(ByRef dtWS121 As Data.DataTable)
        With dtWS121.Columns
            .Add("Attribute1", GetType(String))
            .Add("Attribute2", GetType(String))
            .Add("Attribute3", GetType(String))
            .Add("Attribute4", GetType(String))
            .Add("Attribute5", GetType(String))
            .Add("Attribute6", GetType(String))
            .Add("Attribute7", GetType(String))
            .Add("Attribute8", GetType(String))
            .Add("Attribute9", GetType(String))
            .Add("Attribute10", GetType(String))
            .Add("Attribute11", GetType(String))
            .Add("Attribute12", GetType(String))
            .Add("Attribute13", GetType(String))
            .Add("Attribute14", GetType(String))
            .Add("Attribute15", GetType(String))
            .Add("Attribute16", GetType(String))
            .Add("Attribute17", GetType(String))
            .Add("Attribute18", GetType(String))
            .Add("Attribute19", GetType(String))
            .Add("Attribute20", GetType(String))
            .Add("Attribute1Exists", GetType(Boolean))
            .Add("Attribute2Exists", GetType(Boolean))
            .Add("Attribute3Exists", GetType(Boolean))
            .Add("Attribute4Exists", GetType(Boolean))
            .Add("Attribute5Exists", GetType(Boolean))
            .Add("Attribute6Exists", GetType(Boolean))
            .Add("Attribute7Exists", GetType(Boolean))
            .Add("Attribute8Exists", GetType(Boolean))
            .Add("Attribute9Exists", GetType(Boolean))
            .Add("Attribute10Exists", GetType(Boolean))
            .Add("Attribute11Exists", GetType(Boolean))
            .Add("Attribute12Exists", GetType(Boolean))
            .Add("Attribute13Exists", GetType(Boolean))
            .Add("Attribute14Exists", GetType(Boolean))
            .Add("Attribute15Exists", GetType(Boolean))
            .Add("Attribute16Exists", GetType(Boolean))
            .Add("Attribute17Exists", GetType(Boolean))
            .Add("Attribute18Exists", GetType(Boolean))
            .Add("Attribute19Exists", GetType(Boolean))
            .Add("Attribute20Exists", GetType(Boolean))
            'STUART
            .Add("OwnsAutoMembership", GetType(String))
            .Add("FavouriteSport", GetType(String))
            .Add("FavouriteTeam", GetType(String))
            .Add("FavouriteSupportersClub", GetType(String))
            .Add("SportMailFlag1", GetType(String))
            .Add("SportMailFlag2", GetType(String))
            .Add("SportMailFlag3", GetType(String))
            .Add("SportMailFlag4", GetType(String))
            .Add("SportMailFlag5", GetType(String))
            .Add("PreferredContactMethod", GetType(String))
            .Add("MothersName", GetType(String))
            .Add("FathersName", GetType(String))
            .Add("RegisteredAddress1", GetType(String))
            .Add("RegisteredAddress2", GetType(String))
            .Add("RegisteredAddress3", GetType(String))
            .Add("RegisteredAddress4", GetType(String))
            .Add("RegisteredAddress5", GetType(String))
            .Add("RegisteredPostcode", GetType(String))
            .Add("EncryptedPassword", GetType(String))
        End With
    End Sub

    Protected Function AddUpdateCustomerDetails() As ErrorObj

        Dim err As New ErrorObj
        Dim dRow As DataRow = Nothing

        ResultDataSet = New DataSet
        Dim strProgram As String = ""
        'If the hashedPassword is not already set by now. Set it here.
        If _de.HashedPassword Is String.Empty AndAlso _de.UseEncryptedPassword = True Then
            If Not _de.Password Is String.Empty Then
                Dim passHash As New Talent.Common.PasswordHash
                _de.HashedPassword = passHash.HashSalt(_de.Password, Settings.EcommerceModuleDefaultsValues.SaltString)
            End If
        End If

        '----------------------------------------------------------------------------------
        '   Create command object
        '
        Dim cmdSELECT As iDB2Command = Nothing

        strProgram = "WS612R"

        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                     "/" & strProgram & "(@PARAM1,@PARAM2,@PARAM3)"
        Dim parmIO As iDB2Parameter
        Dim parmIO2 As iDB2Parameter
        Dim parmIO3 As iDB2Parameter
        Dim PARAMOUT As String = String.Empty

        If Not err.HasError Then
            Try

                'Create the Status data table
                Dim DtStatusResults As New DataTable
                ResultDataSet.Tables.Add(DtStatusResults)
                With DtStatusResults.Columns
                    .Add("ErrorOccurred", GetType(String))
                    .Add("ReturnCode", GetType(String))
                End With

                '
                ' If we are logging specific supplyNetRequests then create a new
                ' log record for this call.
                '
                createSupplyNetRequest(Settings.Partner, Settings.LoginId, Settings.SupplyNetRequestName, Settings.TransactionID, DeV11.DECustomersV1.Count, 0, Now, Nothing, True)

                Dim progressCount As Integer
                progressCount = 0

                For Each customer As DECustomer In DeV11.DECustomersV1

                    progressCount += 1
                    _de = customer
                    If _de.ContactTitle.Length > 6 Then
                        _contactLongTitle = _de.ContactTitle
                    Else
                        _contactShortTitle = _de.ContactTitle
                    End If

                    'Set the connection string
                    cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)

                    'Populate the parameter
                    parmIO = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 1024)
                    parmIO.Value = WS612Parm1()
                    parmIO.Direction = ParameterDirection.InputOutput

                    parmIO2 = cmdSELECT.Parameters.Add(Param2, iDB2DbType.iDB2Char, 1024)
                    parmIO2.Value = WS612Parm2()
                    parmIO2.Direction = ParameterDirection.InputOutput

                    parmIO3 = cmdSELECT.Parameters.Add(Param3, iDB2DbType.iDB2Char, 1024)
                    parmIO3.Value = WS612Parm3()
                    parmIO3.Direction = ParameterDirection.InputOutput


                    'Execute
                    TalentCommonLog("AddUpdateCustomerDetails", _de.CustomerNumber, "Backend Request: strHEADER=" & strHEADER & ", parmIO.Value=" & parmIO.Value)

                    cmdSELECT.ExecuteNonQuery()
                    PARAMOUT = cmdSELECT.Parameters(Param1).Value.ToString

                    '
                    ' If we are logging specific supplyNetRequests then update the progress count for
                    ' this call.
                    '
                    updateSupplyNetProgressCount(Settings.TransactionID, progressCount)

                    TalentCommonLog("AddUpdateCustomerDetails", _de.CustomerNumber, "Backend Response: PARAMOUT=" & PARAMOUT)

                    'Set the response data 
                    dRow = DtStatusResults.NewRow
                    'If (PARAMOUT.Substring(1023, 1) = "E" Or PARAMOUT.Substring(1021, 2).Trim <> "") Then
                    If (PARAMOUT.Substring(1023, 1) = "E" Or PARAMOUT.Substring(1021, 2).Trim <> "") AndAlso _
                        (PARAMOUT.Substring(1021, 2).Trim <> "IR" OrElse strProgram = "WS003R") Then
                        dRow("ErrorOccurred") = "E"
                        dRow("ReturnCode") = PARAMOUT.Substring(1021, 2)
                        _de.ErrorFlag = "E"
                        _de.ErrorCode = PARAMOUT.Substring(1021, 2)
                    Else
                        dRow("ErrorOccurred") = ""
                        dRow("ReturnCode") = ""
                        If Not String.IsNullOrEmpty(PARAMOUT.Substring(1002, 12)) Then
                            _de.CustomerNumber = PARAMOUT.Substring(1002, 12)
                        End If
                    End If
                    DtStatusResults.Rows.Add(dRow)

                    If PARAMOUT.Substring(1023, 1) <> "E" Then
                        '---------------------------------
                        ' No errors - Build return dataset 
                        ' 
                        Dim DtCustomerResults As New DataTable
                        ResultDataSet.Tables.Add(DtCustomerResults)
                        With DtCustomerResults.Columns
                            .Add("CustomerNo", GetType(String))
                            .Add("ContactTitle", GetType(String))
                            .Add("ContactInitials", GetType(String))
                            .Add("ContactForename", GetType(String))
                            .Add("ContactSurname", GetType(String))
                            .Add("Salutation", GetType(String))
                            .Add("CompanyName", GetType(String))
                            .Add("PositionInCompany", GetType(String))
                            .Add("AddressLine1", GetType(String))
                            .Add("AddressLine2", GetType(String))
                            .Add("AddressLine3", GetType(String))
                            .Add("AddressLine4", GetType(String))
                            .Add("AddressLine5", GetType(String))
                            .Add("PostCode", GetType(String))
                            .Add("Gender", GetType(String))
                            .Add("HomeTelephoneNumber", GetType(String))
                            .Add("WorkTelephoneNumber", GetType(String))
                            .Add("MobileNumber", GetType(String))
                            .Add("EmailAddress", GetType(String))
                            .Add("DateBirth", GetType(String))
                            .Add("ContactViaMail", GetType(String))
                            .Add("Subscription1", GetType(String))
                            .Add("Subscription2", GetType(String))
                            .Add("Subscription3", GetType(String))
                            .Add("ContactViaMail1", GetType(String))
                            .Add("ContactViaMail2", GetType(String))
                            .Add("ContactViaMail3", GetType(String))
                            .Add("ContactViaMail4", GetType(String))
                            .Add("ContactViaMail5", GetType(String))
                            .Add("ExternalId1", GetType(String))
                            .Add("ExternalId2", GetType(String))
                            .Add("PasswordHint", GetType(String))
                            .Add("OldPassword", GetType(String))
                            .Add("ATSReady", GetType(String))
                            .Add("PriceBand", GetType(String))
                            .Add("LoyaltyPoints", GetType(String))
                            .Add("BondHolder", GetType(String))
                            .Add("OwnsAutoMembership", GetType(String))
                            .Add("WebReady", GetType(String))
                            .Add("OtherMembersatAddress", GetType(String))

                        End With

                        'Set the data 
                        dRow = Nothing
                        dRow = DtCustomerResults.NewRow
                        WS003Values(PARAMOUT, dRow)

                        DtCustomerResults.Rows.Add(dRow)
                    End If
                Next
                Me.markSupplyNetTransactionAsCompleted(Settings.TransactionID)

            Catch ex As Exception
                Me.markSupplyNetTransactionAsCompleted(Settings.TransactionID)
                Const strError As String = "Error during database access"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TACDBCC-02"
                    .HasError = True
                End With
            End Try
        End If

        Return err

    End Function

    Protected Function AdditionalCustomerDetails(ByVal parmWS121R As String) As ErrorObj

        Dim err As New ErrorObj
        Dim dRow As DataRow = Nothing

        Try
            If parmWS121R.Substring(1023, 1) <> "E" Then
                '
                ' No errors - Build return dataset 
                ' 
                Dim DtAdditionalCustomerResults As New DataTable
                ResultDataSet.Tables.Add(DtAdditionalCustomerResults)
                AddWS121Columns(DtAdditionalCustomerResults)

                'Set the data 
                dRow = Nothing
                dRow = DtAdditionalCustomerResults.NewRow

                WS121Values(parmWS121R, dRow)

                DtAdditionalCustomerResults.Rows.Add(dRow)

            End If

        Catch ex As Exception
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBCC-05"
                .HasError = True
            End With
        End Try

        Return err

    End Function

    Private Function WS612Parm1() As String

        Dim myString As String
        Dim dob As String
        Dim homeTel As String = ""
        Dim homeStd As String = ""
        Dim workTel As String = ""
        Dim workStd As String = ""
        Dim mobileTel As String = ""
        Dim mobileStd As String = ""
        Dim stopcodeforid3 As String = ""
        Dim stopcode As String = ""
        'Set the Date Of Birth to a ticketing format
        If _de.DateBirth.Length < 8 Or _de.DateBirth = "19000101" Then
            dob = "00000000"
        Else
            dob = _de.DateBirth.Substring(6, 2) & _de.DateBirth.Substring(4, 2) & _de.DateBirth.Substring(0, 4)
        End If

        'set the stopcode by default value if the passport id has been entered by user 
        If _de.PIN_Number <> String.Empty Then
            stopcode = _de.stopcodeforid3
        Else
            stopcode = _de.StopCode
        End If

        'Format the telephone number
        'Stuart 20090527
        If _de.PhoneNoFormatting IsNot Nothing AndAlso Not _de.PhoneNoFormatting.Equals(String.Empty) Then
            Select Case _de.PhoneNoFormatting
                Case Is = "1"
                    homeStd = ""
                    homeTel = _de.HomeTelephoneNumber
                    workStd = ""
                    workTel = _de.WorkTelephoneNumber
                    mobileStd = ""
                    mobileTel = _de.MobileNumber
            End Select
        Else
            'Stuart 20090527
            FormatTelephone(_de.HomeTelephoneNumber, homeTel, homeStd)
            FormatTelephone(_de.WorkTelephoneNumber, workTel, workStd)
            FormatTelephone(_de.MobileNumber, mobileTel, mobileStd)
            'Stuart 20090527
        End If
        'Stuart 20090527

        Select Case _de.ContactViaMail1
            Case Is = "1"
                _de.ContactViaMail1 = "Y"
            Case Is = "0"
                _de.ContactViaMail1 = "N"
        End Select
        Select Case _de.ContactViaMail2
            Case Is = "1"
                _de.ContactViaMail2 = "Y"
            Case Is = "0"
                _de.ContactViaMail2 = "N"
        End Select
        Select Case _de.ContactViaMail3
            Case Is = "1"
                _de.ContactViaMail3 = "Y"
            Case Is = "0"
                _de.ContactViaMail3 = "N"
        End Select
        Select Case _de.ContactViaMail4
            Case Is = "1"
                _de.ContactViaMail4 = "Y"
            Case Is = "0"
                _de.ContactViaMail4 = "N"
        End Select
        Select Case _de.ContactViaMail5
            Case Is = "1"
                _de.ContactViaMail5 = "Y"
            Case Is = "0"
                _de.ContactViaMail5 = "N"
        End Select


        'Construct the parameter
        myString = Utilities.FixStringLength(_de.ContactForename, 20) & _
                    Utilities.FixStringLength(_de.ContactSurname, 30) & _
                    Utilities.FixStringLength(_de.AddressLine1, 30) & _
                    Utilities.FixStringLength(_de.AddressLine2, 30) & _
                    Utilities.FixStringLength(_de.AddressLine3, 25) & _
                    Utilities.FixStringLength(_de.AddressLine4, 25) & _
                    Utilities.FixStringLength(_de.AddressLine5, 20) & _
                    Utilities.FixStringLength(_de.PostCode, 8) & _
                    Utilities.FixStringLength(dob, 8) & _
                    Utilities.FixStringLength(_de.Gender, 1) & _
                    Utilities.FixStringLength(homeStd, 7) & _
                    Utilities.FixStringLength(homeTel, 15) & _
                    Utilities.FixStringLength(workStd, 7) & _
                    Utilities.FixStringLength(workTel, 15) & _
                    Utilities.FixStringLength(mobileStd, 7) & _
                    Utilities.FixStringLength(mobileTel, 15) & _
                    Utilities.FixStringLength(_de.CompanyName, 50) & _
                    Utilities.FixStringLength(_de.PositionInCompany, 40) & _
                    Utilities.FixStringLength(_de.EmailAddress, 60) & _
                    Utilities.FixStringLength(_de.Password, 60) & _
                    Utilities.FixStringLength("", 8) & _
                    Utilities.FixStringLength(_de.Salutation, 30) & _
                    Utilities.FixStringLength("", 18) & _
                    Utilities.FixStringLength(_de.ContactViaMail, 1) & _
                    Utilities.FixStringLength("", 7) & _
                    Utilities.FixStringLength(_contactShortTitle, 6) & _
                    Utilities.FixStringLength(_de.Subscription1, 1) & _
                    Utilities.FixStringLength(_de.Subscription2, 1) & _
                    Utilities.FixStringLength(_de.Subscription3, 1) & _
                    Utilities.FixStringLength("", 2) & _
                    Utilities.FixStringLength(_de.ContactViaMail1, 1) & _
                    Utilities.FixStringLength(_de.ContactViaMail2, 1) & _
                    Utilities.FixStringLength(_de.ContactViaMail3, 1) & _
                    Utilities.FixStringLength(_de.ContactViaMail4, 1) & _
                    Utilities.FixStringLength(_de.ContactViaMail5, 1) & _
                    Utilities.FixStringLength("", 52) & _
                    Utilities.FixStringLength(_de.Attribute01, 12) & _
                    Utilities.FixStringLength(_de.Attribute01Action, 1) & _
                    Utilities.FixStringLength(_de.Attribute04, 12) & _
                    Utilities.FixStringLength(_de.Attribute04Action, 1) & _
                    Utilities.FixStringLength(_de.Attribute05, 12) & _
                    Utilities.FixStringLength(_de.Attribute05Action, 1) & _
                    Utilities.FixStringLength(_de.PassportNumber, 20) & _
                    Utilities.FixStringLength(_de.GreenCardNumber, 20) & _
                    Utilities.FixStringLength(_de.PIN_Number, 20) & _
                    Utilities.FixStringLength(_de.SUPPORTER_CLUB_CODE, 20) & _
                    Utilities.FixStringLength(_de.FAVOURITE_TEAM_CODE, 20) & _
                    Utilities.FixStringLength(_de.MAIL_TEAM_CODE_1, 20) & _
                    Utilities.FixStringLength(_de.MAIL_TEAM_CODE_2, 20) & _
                    Utilities.FixStringLength(_de.MAIL_TEAM_CODE_3, 20) & _
                    Utilities.FixStringLength(_de.MAIL_TEAM_CODE_4, 20) & _
                    Utilities.FixStringLength(_de.MAIL_TEAM_CODE_5, 20) & _
                    Utilities.FixStringLength(_de.PREFERRED_CONTACT_METHOD, 20) & _
                    Utilities.FixStringLength("", 1) & _
                    Utilities.FixStringLength(_de.MothersName, 40) & _
                    Utilities.FixStringLength(_de.FathersName, 40) & _
                    Utilities.FixStringLength(_de.FAVOURITE_SPORT, 20) & _
                    Utilities.FixStringLength(_de.Agent, 12) & _
                    Utilities.FixStringLength("", 16) & _
                    Utilities.FixStringLength(ConvertToYN(_de.ContactbyPost), 1) & _
                    Utilities.FixStringLength(ConvertToYN(_de.ContactbyTelephoneHome), 1) & _
                    Utilities.FixStringLength(ConvertToYN(_de.ContactbyTelephoneWork), 1) & _
                    Utilities.FixStringLength(ConvertToYN(_de.ContactbyMobile), 1) & _
                    Utilities.FixStringLength(ConvertToYN(_de.ContactbyEmail), 1) & _
                    Utilities.FixStringLength(_de.PriceBand, 1) & _
                    Utilities.FixStringLength(stopcode, 2) & _
                    Utilities.FixStringLength(_de.UpdateCompanyInformation, 1) & _
                    Utilities.FixStringLength(_de.CustomerNumber, 12) & _
                    Utilities.FixStringLength("", 6)
        If Settings.OriginatingSourceCode = String.Empty Then
            myString = myString & "W"
        Else
            myString = myString & Utilities.FixStringLength(Settings.OriginatingSourceCode, 1)
        End If

        Return myString

    End Function

    Private Function WS612Parm2() As String

        Dim myString As String

        'Construct the parameter
        myString = Utilities.FixStringLength(_de.RegisteredAddress1, 30) & _
                    Utilities.FixStringLength(_de.RegisteredAddress2, 30) & _
                    Utilities.FixStringLength(_de.RegisteredAddress3, 30) & _
                    Utilities.FixStringLength(_de.RegisteredAddress4, 30) & _
                    Utilities.FixStringLength(_de.RegisteredAddress5, 30) & _
                    Utilities.FixStringLength(_de.RegisteredPostcode, 10) & _
                    Utilities.FixStringLength(getProcessFlags(), 300) & _
                     Utilities.FixStringLength(" ", 542) & _
                    Utilities.FixStringLength(_de.CustomerNumber, 12) & _
                    Utilities.FixStringLength("", 6)
        If Settings.OriginatingSourceCode = String.Empty Then
            myString = myString & "W"
        Else
            myString = myString & Utilities.FixStringLength(Settings.OriginatingSourceCode, 1)
        End If
        Return myString

    End Function

    Private Function getProcessFlags() As String
        Dim processFlagsString As String = String.Empty
        processFlagsString = Utilities.FixStringLength(_de.UseOptionalFields, 1) & _
        Utilities.FixStringLength(_de.ProcessContactForename, 1) & _
        Utilities.FixStringLength(_de.ProcessContactSurname, 1) & _
        Utilities.FixStringLength(_de.ProcessAddressLine1, 1) & _
        Utilities.FixStringLength(_de.ProcessAddressLine2, 1) & _
        Utilities.FixStringLength(_de.ProcessAddressLine3, 1) & _
        Utilities.FixStringLength(_de.ProcessAddressLine4, 1) & _
        Utilities.FixStringLength(_de.ProcessAddressLine5, 1) & _
        Utilities.FixStringLength(_de.ProcessPostCode, 1) & _
        Utilities.FixStringLength(_de.ProcessDateBirth, 1) & _
        Utilities.FixStringLength(_de.ProcessGender, 1) & _
        Utilities.FixStringLength(_de.ProcessHomeTelephoneNumber, 1) & _
        Utilities.FixStringLength(_de.ProcessWorkTelephoneNumber, 1) & _
        Utilities.FixStringLength(_de.ProcessMobileNumber, 1) & _
        Utilities.FixStringLength(_de.ProcessCompanyName, 1) & _
        Utilities.FixStringLength(_de.ProcessPositionInCompany, 1) & _
        Utilities.FixStringLength(_de.ProcessEmailAddress, 1) & _
        Utilities.FixStringLength(_de.ProcessPassword, 1) & _
        Utilities.FixStringLength(_de.ProcessSalutation, 1) & _
        Utilities.FixStringLength(_de.ProcessContactViaMail, 1) & _
        Utilities.FixStringLength(_de.ProcessSubscription1, 1) & _
        Utilities.FixStringLength(_de.ProcessSubscription2, 1) & _
        Utilities.FixStringLength(_de.ProcessSubscription3, 1) & _
        Utilities.FixStringLength(_de.ProcessContactViaMail1, 1) & _
        Utilities.FixStringLength(_de.ProcessContactViaMail2, 1) & _
        Utilities.FixStringLength(_de.ProcessContactViaMail3, 1) & _
        Utilities.FixStringLength(_de.ProcessContactViaMail4, 1) & _
        Utilities.FixStringLength(_de.ProcessContactViaMail5, 1) & _
        Utilities.FixStringLength(_de.ProcessAttributes, 1) & _
        Utilities.FixStringLength(_de.ProcessPassportNumber, 1) & _
        Utilities.FixStringLength(_de.ProcessGreenCardNumber, 1) & _
        Utilities.FixStringLength(_de.ProcessPinNumber, 1) & _
        Utilities.FixStringLength(_de.ProcessSupporterClubCode, 1) & _
        Utilities.FixStringLength(_de.ProcessFavouriteTeamCode, 1) & _
        Utilities.FixStringLength(_de.ProcessMailTeamCode1, 1) & _
        Utilities.FixStringLength(_de.ProcessMailTeamCode2, 1) & _
        Utilities.FixStringLength(_de.ProcessMailTeamCode3, 1) & _
        Utilities.FixStringLength(_de.ProcessMailTeamCode4, 1) & _
        Utilities.FixStringLength(_de.ProcessMailTeamCode5, 1) & _
        Utilities.FixStringLength(_de.ProcessPreferredContactMethod, 1) & _
        Utilities.FixStringLength(_de.ProcessMothersName, 1) & _
        Utilities.FixStringLength(_de.ProcessFathersName, 1) & _
        Utilities.FixStringLength(_de.ProcessFavouriteSport, 1) & _
        Utilities.FixStringLength(_de.ProcessUpdateCompanyInformation, 1) & _
        Utilities.FixStringLength(_de.ProcessCustomerNumber, 1) & _
        Utilities.FixStringLength(_de.ProcessContactTitle, 1) & _
        Utilities.FixStringLength(_de.ProcessSuffix, 1) & _
        Utilities.FixStringLength(_de.ProcessNickname, 1) & _
        Utilities.FixStringLength(_de.ProcessAltUserName, 1) & _
        Utilities.FixStringLength(_de.ProcessContactSLAccount, 1)
        Return processFlagsString
    End Function

    Private Function WS612Parm3() As String
        Dim myString As New StringBuilder
        myString.Append(Utilities.FixStringLength(FormatUserIDs(), 120))
        myString.Append(Utilities.FixStringLength("", 1))
        myString.Append(Utilities.FixStringLength(_de.NewHashedPassword, 150))
        myString.Append(Utilities.FixStringLength(_de.BranchCode, 3))
        myString.Append(Utilities.FixStringLength(_de.SaltString, 150))
        myString.Append(Utilities.FixStringLength(" ", 211))
        myString.Append(Utilities.FixStringLength(_de.Suffix, 40))
        myString.Append(Utilities.FixStringLength(_de.Nickname, 40))
        myString.Append(Utilities.FixStringLength(_de.AltUserName, 40))
        myString.Append(Utilities.FixStringLength(_de.ContactSLAccount, 20))
        myString.Append(Utilities.FixStringLength(ConvertToYN(_de.MinimalRegistration), 1))
        myString.Append(Utilities.FixStringLength(_de.ContactSource, 10))
        myString.Append(Utilities.FixStringLength(_contactLongTitle, 30))
        myString.Append(Utilities.PadLeadingZeros(_de.CompanyNumber, 13))
        myString.Append(Utilities.FixStringLength("", 72))
        myString.Append(Utilities.FixStringLength(_de.ConsentStatus, 1))
        myString.Append(Utilities.FixStringLength(_de.ParentPhone, 15))
        myString.Append(Utilities.FixStringLength(_de.ParentEmail, 60))
        Return myString.ToString()

    End Function
  


    Private Sub WS003Values(ByVal PARAMOUT As String, ByRef dRow As DataRow)

        'Extract the values from the outbound parameter
        dRow("CustomerNo") = PARAMOUT.Substring(1002, 12)
        dRow("ContactTitle") = PARAMOUT.Substring(537, 6)
        dRow("ContactInitials") = ""
        dRow("ContactForename") = PARAMOUT.Substring(0, 20)
        dRow("ContactSurname") = PARAMOUT.Substring(20, 30)
        dRow("Salutation") = PARAMOUT.Substring(481, 30)
        dRow("CompanyName") = PARAMOUT.Substring(263, 50)
        dRow("PositionInCompany") = PARAMOUT.Substring(313, 40)
        dRow("AddressLine1") = PARAMOUT.Substring(50, 30)
        dRow("AddressLine2") = PARAMOUT.Substring(80, 30)
        dRow("AddressLine3") = PARAMOUT.Substring(110, 25)
        dRow("AddressLine4") = PARAMOUT.Substring(135, 25)
        dRow("AddressLine5") = PARAMOUT.Substring(160, 20)

        'Stuart 20090527
        If _de.PostCodeFormatting IsNot Nothing AndAlso Not _de.PostCodeFormatting.Equals(String.Empty) Then
            Select Case _de.PostCodeFormatting
                '1 = Remove all spaces
                Case Is = "1"
                    dRow("PostCode") = Replace(PARAMOUT.Substring(180, 8), " ", "")
            End Select
        Else
            'Stuart 20090527
            dRow("PostCode") = PARAMOUT.Substring(180, 8)
            'Stuart 20090527
        End If
        'Stuart 20090527

        dRow("Gender") = PARAMOUT.Substring(196, 1)

        'Stuart 20090527
        If _de.PhoneNoFormatting IsNot Nothing AndAlso Not _de.PhoneNoFormatting.Equals(String.Empty) Then
            Select Case _de.PhoneNoFormatting
                '1 = Just use phone number part returned numbers
                Case Is = "1"
                    dRow("HomeTelephoneNumber") = PARAMOUT.Substring(204, 15).Trim
                    dRow("WorkTelephoneNumber") = PARAMOUT.Substring(226, 15).Trim
                    dRow("MobileNumber") = PARAMOUT.Substring(248, 15).Trim
            End Select
        Else
            'Stuart 20090527
            dRow("HomeTelephoneNumber") = PARAMOUT.Substring(197, 7).Trim & " " & PARAMOUT.Substring(204, 15).Trim
            dRow("WorkTelephoneNumber") = PARAMOUT.Substring(219, 7).Trim & " " & PARAMOUT.Substring(226, 15).Trim
            dRow("MobileNumber") = PARAMOUT.Substring(241, 7).Trim & " " & PARAMOUT.Substring(248, 15).Trim
            'Stuart 20090527
        End If
        'Stuart 20090527

        dRow("EmailAddress") = PARAMOUT.Substring(353, 60)
        dRow("DateBirth") = PARAMOUT.Substring(192, 4) & PARAMOUT.Substring(190, 2) & PARAMOUT.Substring(188, 2)
        dRow("ContactViaMail") = PARAMOUT.Substring(529, 1)
        dRow("Subscription1") = PARAMOUT.Substring(543, 1)
        dRow("Subscription2") = PARAMOUT.Substring(544, 1)
        dRow("Subscription3") = PARAMOUT.Substring(545, 1)
        dRow("ContactViaMail1") = PARAMOUT.Substring(548, 1)
        dRow("ContactViaMail2") = PARAMOUT.Substring(549, 1)
        dRow("ContactViaMail3") = PARAMOUT.Substring(550, 1)
        dRow("ContactViaMail4") = PARAMOUT.Substring(551, 1)
        dRow("ContactViaMail5") = PARAMOUT.Substring(552, 1)
        dRow("ExternalId1") = ""
        dRow("ExternalId2") = ""
        dRow("OldPassword") = ""
        dRow("ATSReady") = ""
        dRow("PriceBand") = ""
        dRow("OwnsAutoMembership") = PARAMOUT.Substring(553, 1).Trim
        dRow("OtherMembersatAddress") = PARAMOUT.Substring(986, 1).Trim

    End Sub

    Private Function WS051Parm(ByVal sRecordTotal As String, ByVal sLastRecord As String) As String

        Dim myString As String

        'Construct the parameter

        myString = Utilities.FixStringLength("", 4900) &
                    Utilities.FixStringLength(_de.EmailAddress, 59) &
                    Utilities.FixStringLength("", 35) &
                    Utilities.PadLeadingZeros(_de.Category, 3) &
                    Utilities.PadLeadingZeros(_de.CustomerNumber, 12) &
                    Utilities.FixStringLength("", 1) &
                    Utilities.PadLeadingZeros(sRecordTotal, 5) &
                    Utilities.PadLeadingZeros(sLastRecord, 5) &
                    Utilities.FixStringLength("", 96) &
                    Utilities.FixStringLength(_de.Source, 1) &
                    Utilities.FixStringLength("", 2) &
                    Utilities.FixStringLength("", 1)
        Return myString

    End Function

    Protected Function CustomerAttributes(Optional ByVal PARAMOUT As String = "", Optional ByVal calledFromWS607R As Boolean = False) As ErrorObj
        Dim err As New ErrorObj
        Dim drow As DataRow = Nothing
        If Not calledFromWS607R Then ResultDataSet = New DataSet
        Dim strProgram As String = ""
        Dim sRecordTotal As String = "000"
        Dim sLastRecord As String = "000"
        Dim bMoreRecords As Boolean = True
        Dim sMore As String = String.Empty

        Dim strHeader As String = "CALL " & Settings.StoredProcedureGroup.Trim & "/WS051R" & "(@PARAM1)"

        If Not err.HasError Then
            Dim DtStatusResults As New DataTable
            If Not calledFromWS607R Then ResultDataSet.Tables.Add(DtStatusResults)
            With DtStatusResults.Columns
                .Add("ErrorOccured", GetType(String))
                .Add("ReturnCode", GetType(String))
            End With

            Dim DtCustomerAttributeResults As New DataTable("CustomerAttributes")
            ResultDataSet.Tables.Add(DtCustomerAttributeResults)
            With DtCustomerAttributeResults.Columns
                .Add("Attribute", GetType(String))
                .Add("AttributeId", GetType(String))
                .Add("AttributeName", GetType(String))
                .Add("AttributeData", GetType(String))
                .Add("LoginId", GetType(String))
            End With
            Try

                'Loop until no more attributes are available
                Do While bMoreRecords = True

                    'Call WS016R
                    If String.IsNullOrWhiteSpace(PARAMOUT) Then PARAMOUT = CallWS051R(sRecordTotal, sLastRecord)

                    'Set the response data on the first call to WS016R
                    If sLastRecord = "000" Then
                        drow = Nothing
                        drow = DtStatusResults.NewRow
                        If PARAMOUT.Substring(5119, 1) = "E" Or PARAMOUT.Substring(5117, 2).Trim <> "" Then
                            drow("ErrorOccured") = "E"
                            drow("ReturnCode") = PARAMOUT.Substring(5117, 2)
                            bMoreRecords = False
                        Else
                            drow("ErrorOccured") = ""
                            drow("ReturnCode") = ""
                        End If
                        DtStatusResults.Rows.Add(drow)
                    End If

                    'No errors 
                    If PARAMOUT.Substring(5119, 1) <> "E" And PARAMOUT.Substring(5117, 2).Trim = "" Then

                        'Extract the data from the parameter
                        Dim iPosition As Integer = 0
                        Dim iCounter As Integer = 1
                        Do While iCounter <= 200

                            ' Has a product been returned
                            If PARAMOUT.Substring(iPosition, 12).Trim = "" Then
                                Exit Do
                            Else

                                'Create a new row
                                drow = Nothing
                                drow = DtCustomerAttributeResults.NewRow
                                drow("Attribute") = PARAMOUT.Substring(iPosition, 12)
                                drow("AttributeId") = PARAMOUT.Substring(iPosition + 15, 13)
                                drow("AttributeName") = PARAMOUT.Substring(iPosition + 28, 30)
                                drow("AttributeData") = ""
                                drow("LoginId") = Utilities.PadLeadingZeros(_de.CustomerNumber, 12)
                                DtCustomerAttributeResults.Rows.Add(drow)

                                ''Increment
                                'If calledFromWS607R Then
                                '    iPosition = iPosition + 100
                                'Else
                                '    iPosition = iPosition + 25
                                'End If
                                iPosition = iPosition + 58
                                iCounter = iCounter + 1
                            End If
                        Loop

                        'Extract the footer information
                        sLastRecord = PARAMOUT.Substring(5113, 3)
                        sRecordTotal = PARAMOUT.Substring(5110, 3)
                        'If CInt(sLastRecord) >= CInt(sRecordTotal) Then
                        '    bMoreRecords = False
                        'End If
                        sMore = PARAMOUT.Substring(5109, 1)
                        If sMore = "Y" Then
                            bMoreRecords = True
                            PARAMOUT = ""
                        Else
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
                    .ErrorNumber = "TACDBCC-02"
                    .HasError = True
                End With
            End Try
        End If
        Return err
    End Function

    Private Function CallWS051R(ByVal sRecordTotal As String, ByVal sLastRecord As String) As String

        'Create command object
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strProgram As String = "WS051R"
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                    "/" & strProgram & "(@PARAM1)"
        Dim parmIO As iDB2Parameter
        Dim PARAMOUT As String = String.Empty

        'Set the connection string
        cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)

        'Populate the parameter
        parmIO = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 5120)
        parmIO.Value = WS051Parm(sRecordTotal, sLastRecord)
        parmIO.Direction = ParameterDirection.InputOutput

        'Execute
        TalentCommonLog("CallWS051R", "", "Backend Request: strHEADER=" & strHEADER & ", parmIO.Value=" & parmIO.Value)

        cmdSELECT.ExecuteNonQuery()
        PARAMOUT = cmdSELECT.Parameters(Param1).Value.ToString

        TalentCommonLog("CallWS051R", "", "Backend Response: PARAMOUT=" & PARAMOUT)

        Return PARAMOUT

    End Function

    Private Function WS009Parm() As String

        Dim myString As String
        Dim mode As String = ""
        Dim byPassPassword As String = "N"

        'Set the mode value
        If Settings.ModuleName = ValidateCustomer Then
            mode = "V"
        End If

        'By pass the password when in kiosk mode
        If Settings.TicketingKioskMode Or _de.PasswordType = "N" Then
            byPassPassword = "Y"
        End If

        ' Secure password fields are only relevant for the WS607R call (same entry parm is shared
        If Settings.ModuleName <> VerifyAndRetrieveCustomerDetails Then
            _de.HashedPassword = String.Empty
        End If


        'Construct the parameter
        myString = Utilities.FixStringLength(_de.BasketID, 36) & _
                    Utilities.FixStringLength("", 317) & _
                    Utilities.FixStringLength(_de.EmailAddress, 60) & _
                    Utilities.FixStringLength(_de.Password, 60) & _
                    Utilities.FixStringLength(_de.HashedPassword, 150) & _
                    Utilities.FixStringLength("", 1) & _
                    Utilities.FixStringLength("", 33) & _
                    Utilities.FixStringLength(mode, 1) & _
                    Utilities.FixStringLength("", 64) & _
                    Utilities.FixStringLength(_de.PassportNumber, 20) & _
                    Utilities.FixStringLength(_de.GreenCardNumber, 20) & _
                    Utilities.FixStringLength(_de.PIN_Number, 20) & _
                    Utilities.FixStringLength(_settings.Stadium, 2) & _
                    Utilities.FixStringLength(_de.PasswordType, 1) & _
                    Utilities.FixStringLength(_de.MembershipNumber, 30) & _
                    Utilities.FixStringLength(_de.PaymentReference, 15) & _
                    Utilities.FixStringLength(_de.CorporateSaleID, 13) & _
                    Utilities.FixStringLength("", 145) & _
                    Utilities.FixStringLength(byPassPassword, 1) & _
                    Utilities.PadLeadingZeros("0", 13) & _
                    Utilities.PadLeadingZeros(_de.CustomerNumber, 12) & "000000W   "


        Return myString

    End Function
   

    Private Function WS009Parm2() As String
        Dim mystring As StringBuilder = New StringBuilder
        mystring.Append(FormatUserIDs()) '620
        myString.Append(Utilities.FixStringLength(String.Empty, 146)) '620


        Return mystring.ToString

    End Function

    Private Function WS009Parm3() As String

        Return Utilities.FixStringLength("", 6000)

    End Function

    Public Sub WS009Values(ByVal parmDE As DECustomer, ByVal PARAMOUT As String, ByVal PARAMOUT2 As String, ByVal PARAMOUT3 As String, ByRef dRow As DataRow)

        'Extract the values from the outbound parameter
        If parmDE.Agent Is Nothing Then
            parmDE.HasReservedGameAvailable = convertToBool(PARAMOUT.Substring(1001, 1))
        Else
            parmDE.HasReservedGameAvailable = If(CType(parmDE.Agent, Boolean), False, convertToBool(PARAMOUT.Substring(1001, 1)))
        End If
        dRow("CustomerNumber") = PARAMOUT.Substring(1002, 12)
        dRow("ContactTitle") = PARAMOUT.Substring(602, 6)
        If PARAMOUT2.Length > 760 AndAlso PARAMOUT2.Substring(760, 30).Trim.Length > 0 Then
            dRow("ContactTitle") = PARAMOUT2.Substring(760, 30)
        End If
        dRow("ContactInitials") = ""
        dRow("ContactForename") = PARAMOUT.Substring(0, 20)
        dRow("ContactSurname") = PARAMOUT.Substring(20, 30)
        dRow("Salutation") = PARAMOUT.Substring(481, 30)
        dRow("CompanyName") = PARAMOUT.Substring(263, 50)
        dRow("PositionInCompany") = PARAMOUT.Substring(313, 40)
        dRow("AddressLine1") = PARAMOUT.Substring(50, 30)
        dRow("AddressLine2") = PARAMOUT.Substring(80, 30)
        dRow("AddressLine3") = PARAMOUT.Substring(110, 25)
        dRow("AddressLine4") = PARAMOUT.Substring(135, 25)
        dRow("AddressLine5") = PARAMOUT.Substring(160, 20)
        'Stuart 20090527
        If parmDE.PostCodeFormatting IsNot Nothing AndAlso Not parmDE.PostCodeFormatting.Equals(String.Empty) Then
            Select Case parmDE.PostCodeFormatting
                Case Is = "1"
                    dRow("PostCode") = Replace(PARAMOUT.Substring(180, 8), " ", "")
            End Select
        Else
            'Stuart 20090527
            dRow("PostCode") = PARAMOUT.Substring(180, 8)
            'Stuart 20090527
        End If
        'Stuart 20090527

        dRow("Gender") = PARAMOUT.Substring(196, 1)
        'Stuart 20090527
        If parmDE.PhoneNoFormatting IsNot Nothing AndAlso Not parmDE.PhoneNoFormatting.Equals(String.Empty) Then
            Select Case parmDE.PhoneNoFormatting
                Case Is = "1"
                    dRow("HomeTelephoneNumber") = PARAMOUT.Substring(204, 15).Trim
                    dRow("WorkTelephoneNumber") = PARAMOUT.Substring(226, 15).Trim
                    dRow("MobileNumber") = PARAMOUT.Substring(248, 15).Trim
            End Select
        Else
            'Stuart 20090527
            dRow("HomeTelephoneNumber") = (PARAMOUT.Substring(197, 7).Trim & " " & PARAMOUT.Substring(204, 15).Trim).ToString.Trim
            dRow("WorkTelephoneNumber") = (PARAMOUT.Substring(219, 7).Trim & " " & PARAMOUT.Substring(226, 15).Trim).ToString.Trim
            dRow("MobileNumber") = (PARAMOUT.Substring(241, 7).Trim & " " & PARAMOUT.Substring(248, 15).Trim).ToString.Trim
            'Stuart 20090527
        End If
        'Stuart 20090527

        dRow("EmailAddress") = PARAMOUT.Substring(353, 60)
        dRow("DateBirth") = PARAMOUT.Substring(188, 8)

        ' BF - 19/05/09 - Wrong positions. Never worked.
        'dRow("Subscription1") = PARAMOUT.Substring(511, 1)
        'dRow("Subscription2") = PARAMOUT.Substring(512, 1)
        'dRow("Subscription3") = PARAMOUT.Substring(513, 1)
        dRow("Subscription1") = PARAMOUT.Substring(784, 1)
        dRow("Subscription2") = PARAMOUT.Substring(785, 1)
        dRow("Subscription3") = PARAMOUT.Substring(786, 1)

        dRow("ContactViaMail") = PARAMOUT.Substring(529, 1)
        dRow("ContactViaMail1") = PARAMOUT.Substring(787, 1)
        dRow("ContactViaMail2") = PARAMOUT.Substring(788, 1)
        dRow("ContactViaMail3") = PARAMOUT.Substring(789, 1)
        dRow("ContactViaMail4") = PARAMOUT.Substring(790, 1)
        dRow("ContactViaMail5") = PARAMOUT.Substring(791, 1)
        dRow("ExternalId1") = PARAMOUT.Substring(812, 60).Trim
        dRow("ExternalId2") = PARAMOUT.Substring(872, 60).Trim
        dRow("PasswordHint") = PARAMOUT.Substring(413, 60).Trim
        dRow("OldPassword") = PARAMOUT.Substring(932, 1).Trim
        dRow("ATSReady") = PARAMOUT.Substring(599, 1).Trim
        dRow("PriceBand") = PARAMOUT.Substring(590, 1).Trim
        dRow("LoyaltyPoints") = PARAMOUT.Substring(592, 5).Trim
        'dRow("BondHolder") = PARAMOUT.Substring(785, 1).Trim
        dRow("BondHolder") = PARAMOUT.Substring(783, 1).Trim
        dRow("WebReady") = PARAMOUT.Substring(480, 1).Trim
        dRow("PassportNumber") = PARAMOUT.Substring(722, 20).Trim
        dRow("GreenCardNumber") = PARAMOUT.Substring(742, 20).Trim
        dRow("PINNumber") = PARAMOUT.Substring(762, 20).Trim
        dRow("STHolder") = PARAMOUT.Substring(609, 1).Trim
        dRow("SCHolder") = PARAMOUT.Substring(610, 1).Trim
        dRow("ContactbyPost") = convertToBool(PARAMOUT.Substring(611, 1).Trim)
        dRow("ContactbyTelephoneHome") = convertToBool(PARAMOUT.Substring(612, 1).Trim)
        dRow("ContactbyTelephoneWork") = convertToBool(PARAMOUT.Substring(613, 1).Trim)
        dRow("ContactbyMobile") = convertToBool(PARAMOUT.Substring(614, 1).Trim)
        dRow("ContactbyEmail") = convertToBool(PARAMOUT.Substring(615, 1).Trim)
        dRow("StopCode") = PARAMOUT.Substring(616, 2).Trim
        dRow("BookNumber") = PARAMOUT.Substring(618, 7).Trim
        dRow("PaymentReference") = PARAMOUT.Substring(625, 15).Trim

        'PARAMOUT2 Values
        If PARAMOUT2.Trim <> "" Then
            dRow("UserID4") = PARAMOUT2.Substring(0, 20).Trim
            dRow("UserID5") = PARAMOUT2.Substring(20, 20).Trim
            dRow("UserID6") = PARAMOUT2.Substring(40, 20).Trim
            dRow("UserID7") = PARAMOUT2.Substring(60, 20).Trim
            dRow("UserID8") = PARAMOUT2.Substring(80, 20).Trim
            dRow("UserID9") = PARAMOUT2.Substring(100, 20).Trim
            dRow("FanFlag") = PARAMOUT2.Substring(120, 1).Trim
            dRow("EmergencyNumber") = PARAMOUT2.Substring(121, 30).Trim
            dRow("CRMCompanyName") = PARAMOUT2.Substring(181, 50).Trim
            dRow("ConsentStatus") = PARAMOUT2.Substring(231, 1).Trim
            dRow("ParentPhone") = PARAMOUT2.Substring(232, 15).Trim
            dRow("ParentEmail") = PARAMOUT2.Substring(247, 60).Trim

            'big empty space
            dRow("EmergencyName") = PARAMOUT2.Substring(407, 30).Trim
            dRow("FavouriteSeat") = PARAMOUT2.Substring(437, 3) & PARAMOUT2.Substring(440, 4) & PARAMOUT2.Substring(444, 4) & PARAMOUT2.Substring(448, 5)
            dRow("SmartcardNumber") = PARAMOUT2.Substring(606, 14).Trim
            dRow("NickName") = PARAMOUT2.Substring(660, 40).Trim
            dRow("AltUserName") = PARAMOUT2.Substring(700, 40).Trim
            dRow("Suffix") = PARAMOUT2.Substring(620, 40).Trim
            dRow("SLNumber") = PARAMOUT2.Substring(740, 20).Trim
            dRow("CompanyNumber") = PARAMOUT2.Substring(790, 213).Trim

        End If

        'PARAMOUT3 Values
        If PARAMOUT3.Trim <> "" Then
            dRow("MedicalInfo") = PARAMOUT3.Substring(0, 750).Trim
            dRow("EncryptedPassword") = PARAMOUT3.Substring(5500, 500).Trim
        End If

    End Sub
    Public Sub WS105Values(ByVal parmDE As DECustomer, ByVal PARAMOUT As String, ByRef dRow As DataRow)

        'Extract the values from the outbound parameter
        If parmDE.Agent Is Nothing Then
            parmDE.HasReservedGameAvailable = convertToBool(PARAMOUT.Substring(1001, 1))
        Else
            parmDE.HasReservedGameAvailable = If(CType(parmDE.Agent, Boolean), False, convertToBool(PARAMOUT.Substring(1001, 1)))
        End If
        dRow("CustomerNumber") = PARAMOUT.Substring(1002, 12)
        dRow("ContactTitle") = PARAMOUT.Substring(602, 6).Trim

        dRow("ContactInitials") = ""
        dRow("ContactForename") = PARAMOUT.Substring(0, 20).Trim
        dRow("ContactSurname") = PARAMOUT.Substring(20, 30).Trim
        dRow("Salutation") = PARAMOUT.Substring(481, 30).Trim
        dRow("CompanyName") = PARAMOUT.Substring(263, 50).Trim
        dRow("PositionInCompany") = PARAMOUT.Substring(313, 40)
        dRow("AddressLine1") = PARAMOUT.Substring(50, 30).Trim
        dRow("AddressLine2") = PARAMOUT.Substring(80, 30).Trim
        dRow("AddressLine3") = PARAMOUT.Substring(110, 25).Trim
        dRow("AddressLine4") = PARAMOUT.Substring(135, 25).Trim
        dRow("AddressLine5") = PARAMOUT.Substring(160, 20).Trim

        If parmDE.PostCodeFormatting IsNot Nothing AndAlso Not parmDE.PostCodeFormatting.Equals(String.Empty) Then
            Select Case parmDE.PostCodeFormatting
                Case Is = "1"
                    dRow("PostCode") = Replace(PARAMOUT.Substring(180, 8), " ", "").Trim
            End Select
        Else

            dRow("PostCode") = PARAMOUT.Substring(180, 8).Trim

        End If


        dRow("Gender") = PARAMOUT.Substring(196, 1).Trim

        If parmDE.PhoneNoFormatting IsNot Nothing AndAlso Not parmDE.PhoneNoFormatting.Equals(String.Empty) Then
            Select Case parmDE.PhoneNoFormatting
                Case Is = "1"
                    dRow("HomeTelephoneNumber") = PARAMOUT.Substring(204, 15).Trim
                    dRow("WorkTelephoneNumber") = PARAMOUT.Substring(226, 15).Trim
                    dRow("MobileNumber") = PARAMOUT.Substring(248, 15).Trim
            End Select
        Else

            dRow("HomeTelephoneNumber") = (PARAMOUT.Substring(197, 7).Trim & " " & PARAMOUT.Substring(204, 15).Trim).ToString.Trim
            dRow("WorkTelephoneNumber") = (PARAMOUT.Substring(219, 7).Trim & " " & PARAMOUT.Substring(226, 15).Trim).ToString.Trim
            dRow("MobileNumber") = (PARAMOUT.Substring(241, 7).Trim & " " & PARAMOUT.Substring(248, 15).Trim).ToString.Trim

        End If


        dRow("EmailAddress") = PARAMOUT.Substring(353, 60).Trim
        dRow("DateBirth") = PARAMOUT.Substring(188, 8)
        dRow("ContactViaMail") = PARAMOUT.Substring(529, 1)
        dRow("Subscription1") = PARAMOUT.Substring(784, 1)
        dRow("Subscription2") = PARAMOUT.Substring(785, 1)
        dRow("Subscription3") = PARAMOUT.Substring(786, 1)
        dRow("ContactViaMail") = PARAMOUT.Substring(529, 1)
        dRow("ContactViaMail1") = PARAMOUT.Substring(787, 1)
        dRow("ContactViaMail2") = PARAMOUT.Substring(788, 1)
        dRow("ContactViaMail3") = PARAMOUT.Substring(789, 1)
        dRow("ContactViaMail4") = PARAMOUT.Substring(790, 1)
        dRow("ContactViaMail5") = PARAMOUT.Substring(791, 1)
        dRow("ExternalId1") = PARAMOUT.Substring(812, 60).Trim
        dRow("ExternalId2") = PARAMOUT.Substring(872, 60).Trim
        dRow("PasswordHint") = PARAMOUT.Substring(413, 60).Trim
        dRow("OldPassword") = PARAMOUT.Substring(932, 1).Trim
        dRow("ATSReady") = PARAMOUT.Substring(599, 1).Trim
        dRow("PriceBand") = PARAMOUT.Substring(590, 1).Trim
        dRow("LoyaltyPoints") = PARAMOUT.Substring(592, 5).Trim
        'dRow("BondHolder") = PARAMOUT.Substring(785, 1).Trim
        dRow("BondHolder") = PARAMOUT.Substring(783, 1).Trim
        dRow("WebReady") = PARAMOUT.Substring(480, 1).Trim
        dRow("PassportNumber") = PARAMOUT.Substring(722, 20).Trim
        dRow("GreenCardNumber") = PARAMOUT.Substring(742, 20).Trim
        dRow("PINNumber") = PARAMOUT.Substring(762, 20).Trim
        dRow("STHolder") = PARAMOUT.Substring(609, 1).Trim
        dRow("SCHolder") = PARAMOUT.Substring(610, 1).Trim
        dRow("ContactbyPost") = convertToBool(PARAMOUT.Substring(611, 1).Trim)
        dRow("ContactbyTelephoneHome") = convertToBool(PARAMOUT.Substring(612, 1).Trim)
        dRow("ContactbyTelephoneWork") = convertToBool(PARAMOUT.Substring(613, 1).Trim)
        dRow("ContactbyMobile") = convertToBool(PARAMOUT.Substring(614, 1).Trim)
        dRow("ContactbyEmail") = convertToBool(PARAMOUT.Substring(615, 1).Trim)
        dRow("StopCode") = PARAMOUT.Substring(616, 2).Trim
        dRow("BookNumber") = PARAMOUT.Substring(618, 7).Trim
        dRow("PaymentReference") = PARAMOUT.Substring(625, 15).Trim

    End Sub

    Public Sub WS121Values(ByVal PARAMOUT As String, ByRef dRow As DataRow)
        'Extract the values from the outbound parameter
        Dim position As Integer = 768
        For i As Integer = 1 To 20
            Dim value As Boolean = False
            Dim attribute As String = PARAMOUT.Substring(position, 12)
            position += 12
            If PARAMOUT.Substring(i - 1, 1).Equals("Y") Then
                value = True
            End If
            dRow("Attribute" & i & "Exists") = value
            dRow("Attribute" & i) = attribute
        Next
        'STUART 
        dRow("OwnsAutoMembership") = PARAMOUT.Substring(20, 1)
        dRow("FavouriteSport") = PARAMOUT.Substring(21, 20)
        dRow("FavouriteTeam") = PARAMOUT.Substring(41, 20)
        dRow("FavouriteSupportersClub") = PARAMOUT.Substring(61, 20)
        dRow("SportMailFlag1") = PARAMOUT.Substring(81, 20)
        dRow("SportMailFlag2") = PARAMOUT.Substring(101, 20)
        dRow("SportMailFlag3") = PARAMOUT.Substring(121, 20)
        dRow("SportMailFlag4") = PARAMOUT.Substring(141, 20)
        dRow("SportMailFlag5") = PARAMOUT.Substring(161, 20)
        dRow("PreferredContactMethod") = PARAMOUT.Substring(181, 20)
        dRow("MothersName") = PARAMOUT.Substring(201, 40)
        dRow("FathersName") = PARAMOUT.Substring(241, 40)
        dRow("RegisteredAddress1") = PARAMOUT.Substring(281, 30)
        dRow("RegisteredAddress2") = PARAMOUT.Substring(311, 30)
        dRow("RegisteredAddress3") = PARAMOUT.Substring(341, 30)
        dRow("RegisteredAddress4") = PARAMOUT.Substring(371, 30)
        dRow("RegisteredAddress5") = PARAMOUT.Substring(401, 30)
        dRow("RegisteredPostcode") = PARAMOUT.Substring(431, 10)
        dRow("EncryptedPassword") = PARAMOUT.Substring(617, 150)


    End Sub

    Private Function WS121Parm() As String

        Dim myString As String

        'Construct the parameter
        myString = Utilities.FixStringLength("", 767)
        If Settings.PerformWatchListCheck Then
            myString += Utilities.FixStringLength("Y", 1)
        Else
            myString += Utilities.FixStringLength("N", 1)
        End If
        myString += Utilities.FixStringLength(_de.Attribute01, 12) & _
                    Utilities.FixStringLength(_de.Attribute02, 12) & _
                    Utilities.FixStringLength(_de.Attribute03, 12) & _
                    Utilities.FixStringLength(_de.Attribute04, 12) & _
                    Utilities.FixStringLength(_de.Attribute05, 12) & _
                    Utilities.FixStringLength(_de.Attribute06, 12) & _
                    Utilities.FixStringLength(_de.Attribute07, 12) & _
                    Utilities.FixStringLength(_de.Attribute08, 12) & _
                    Utilities.FixStringLength(_de.Attribute09, 12) & _
                    Utilities.FixStringLength(_de.Attribute10, 12) & _
                    Utilities.FixStringLength(_de.Attribute11, 12) & _
                    Utilities.FixStringLength(_de.Attribute12, 12) & _
                    Utilities.FixStringLength(_de.Attribute13, 12) & _
                    Utilities.FixStringLength(_de.Attribute14, 12) & _
                    Utilities.FixStringLength(_de.Attribute15, 12) & _
                    Utilities.FixStringLength(_de.Attribute16, 12) & _
                    Utilities.FixStringLength(_de.Attribute17, 12) & _
                    Utilities.FixStringLength(_de.Attribute18, 12) & _
                    Utilities.FixStringLength(_de.Attribute19, 12) & _
                    Utilities.FixStringLength(_de.Attribute20, 12) & _
                    Utilities.PadLeadingZeros(_de.CustomerNumber, 12) & "W   "
        Return myString

    End Function

    Protected Overrides Function AccessDataBaseADL() As ErrorObj

        _de = DeV11.DECustomersV1(0)

        Dim err As New ErrorObj

        Dim decs As New DECustomerSettings
        decs = CType(MyBase.Settings, DECustomerSettings)

        ' Setup SQL Select Statements
        Dim sbSqlXXC2P100 As New StringBuilder
        With sbSqlXXC2P100
            .Append("INSERT INTO XXC2P100 VALUES (")
            .Append("@C2USER, ")
            .Append("@C2PGMD, ")
            .Append("@C2UPDT, ")
            .Append("@C2UPTM, ")
            .Append("@C2ACTR, ")
            .Append("@C2HDID, ")
            .Append("@C2FUID, ")
            .Append("@C2FLG1, ")
            .Append("@C2FLG2, ")
            .Append("@C2CHR1, ")
            .Append("@C2CHR2, ")
            .Append("@C2NUM1, ")
            .Append("@C2NUM2, ")
            .Append("@C2ULN2")
            .Append(")")
        End With
        Dim strSqlXXC2P100 As String = sbSqlXXC2P100.ToString

        Dim sbSqlXXC2P200 As New StringBuilder
        With sbSqlXXC2P200
            .Append("INSERT INTO XXC2P200 VALUES (")
            .Append("@C3USER, ")
            .Append("@C3PGMD, ")
            .Append("@C3UPDT, ")
            .Append("@C3UPTM, ")
            .Append("@C3ACTR, ")
            .Append("@C3C3ID, ")
            .Append("@C3HDID, ")
            .Append("@C3FDID, ")
            .Append("@C3TITL, ")
            .Append("@C3VALU, ")
            .Append("@C3VAL1, ")
            .Append("@C3VAL2, ")
            .Append("@C3FLG1, ")
            .Append("@C3FLG2, ")
            .Append("@C3CHR1, ")
            .Append("@C3CHR2, ")
            .Append("@C3NUM1, ")
            .Append("@C3NUM2, ")
            .Append("@C3ULN")
            .Append(")")
        End With
        Dim strSqlXXC2P200 As String = sbSqlXXC2P200.ToString

        Dim cmdSelectXXC2P100 As iDB2Command = Nothing
        Dim cmdSelectXXC2P200 As iDB2Command = Nothing

        Dim strSqlSelectXXC2P100ID As String = "SELECT MAX(C2HDID) AS C2HDID FROM XXC2P100"
        Dim strSqlSelectXXC2P200ID As String = "SELECT MAX(C3C3ID) AS C3C3ID FROM XXC2P200"
        Dim strSqlUpload As String = "Call " & Settings.StoredProcedureGroup.Trim & _
                                    "/UPLOADCUS(@PARAM1, @PARAM2)"

        Dim cmdSelectXXC2P100ID As iDB2Command = Nothing
        Dim cmdSelectXXC2P200ID As iDB2Command = Nothing
        Dim cmdUpload As iDB2Command = Nothing

        Dim drSelectXXC2P100ID As iDB2DataReader = Nothing
        Dim drSelectXXC2P200ID As iDB2DataReader = Nothing

        Dim headerID As Decimal = 0
        Dim detailID As Decimal = 0

        Try

            'Get Next Header ID
            cmdSelectXXC2P100ID = New iDB2Command(strSqlSelectXXC2P100ID, conADL)
            drSelectXXC2P100ID = cmdSelectXXC2P100ID.ExecuteReader
            If drSelectXXC2P100ID.HasRows Then
                While drSelectXXC2P100ID.Read
                    headerID = Utilities.CheckForDBNull_Decimal(drSelectXXC2P100ID("C2HDID"))
                End While
            End If
            drSelectXXC2P100ID.Close()
            headerID += 1

            'Get Next Detail ID - write the records backwards (ID-wise) to avoid updates from other users 
            ' clashing
            cmdSelectXXC2P200ID = New iDB2Command(strSqlSelectXXC2P200ID, conADL)
            drSelectXXC2P200ID = cmdSelectXXC2P200ID.ExecuteReader
            If drSelectXXC2P200ID.HasRows Then
                While drSelectXXC2P200ID.Read
                    detailID = Utilities.CheckForDBNull_Decimal(drSelectXXC2P200ID("C3C3ID"))
                End While
            End If
            Select Case decs.CreationType
                Case Is = "REGISTRATION"
                    detailID += 17
                Case Is = "UPDATE"
                    detailID += 15
                Case Is = "CONTACTUS"
                    detailID += 9
            End Select
            drSelectXXC2P200ID.Close()

            'XXC2P100
            cmdSelectXXC2P100 = New iDB2Command(strSqlXXC2P100, conADL)
            With cmdSelectXXC2P100.Parameters
                .Add("@C2USER", iDB2DbType.iDB2VarChar, 10).Value = "CALLCENT"
                .Add("@C2PGMD", iDB2DbType.iDB2VarChar, 10).Value = "ADE_TALN_R"
                .Add("@C2UPDT", iDB2DbType.iDB2Decimal).Value = "1" & Format(Date.Now, "yyMMdd")
                .Add("@C2UPTM", iDB2DbType.iDB2Decimal).Value = Format(Date.Now, "HHmmss")
                .Add("@C2ACTR", iDB2DbType.iDB2VarChar, 1).Value = "A"
                .Add("@C2HDID", iDB2DbType.iDB2Decimal).Value = headerID
                Select Case decs.CreationType
                    Case Is = "REGISTRATION"
                        .Add("@C2FUID", iDB2DbType.iDB2Decimal).Value = 1
                    Case Is = "UPDATE"
                        .Add("@C2FUID", iDB2DbType.iDB2Decimal).Value = 2
                    Case Is = "CONTACTUS"
                        .Add("@C2FUID", iDB2DbType.iDB2Decimal).Value = 3
                End Select
                .Add("@C2FLG1", iDB2DbType.iDB2VarChar, 1).Value = ""
                .Add("@C2FLG2", iDB2DbType.iDB2VarChar, 1).Value = ""
                .Add("@C2CHR1", iDB2DbType.iDB2VarChar, 30).Value = ""
                .Add("@C2CHR2", iDB2DbType.iDB2VarChar, 30).Value = ""
                .Add("@C2NUM1", iDB2DbType.iDB2Decimal).Value = 0
                .Add("@C2NUM2", iDB2DbType.iDB2Decimal).Value = 0
                '.Add("@C2ULN2", iDB2DbType.iDB2VarChar, 3).Value = ""
                .Add("@C2ULN2", iDB2DbType.iDB2VarChar, 3).Value = _de.Language
            End With
            cmdSelectXXC2P100.ExecuteNonQuery()

            'XXC2P200
            Select Case decs.CreationType

                Case Is = "REGISTRATION", "UPDATE"

                    cmdSelectXXC2P200 = New iDB2Command(strSqlXXC2P200, conADL)
                    Select Case decs.CreationType
                        Case Is = "REGISTRATION"
                            AddC2P200Parameters(cmdSelectXXC2P200, detailID, headerID, 5, "Action", "I")
                        Case Is = "UPDATE"
                            AddC2P200Parameters(cmdSelectXXC2P200, detailID, headerID, 5, "Action", "U")
                    End Select
                    cmdSelectXXC2P200.ExecuteNonQuery()

                    detailID -= 1
                    cmdSelectXXC2P200 = New iDB2Command(strSqlXXC2P200, conADL)
                    AddC2P200Parameters(cmdSelectXXC2P200, detailID, headerID, 8, "ThirdPartyRef", _
                        _de.ThirdPartyCompanyRef1Supplement & _de.ThirdPartyCompanyRef1.ToString.PadLeft(9, "0"))
                    cmdSelectXXC2P200.ExecuteNonQuery()

                    detailID -= 1
                    cmdSelectXXC2P200 = New iDB2Command(strSqlXXC2P200, conADL)
                    AddC2P200Parameters(cmdSelectXXC2P200, detailID, headerID, 100, "AddressLine1", _de.AddressLine1)
                    cmdSelectXXC2P200.ExecuteNonQuery()

                    detailID -= 1
                    cmdSelectXXC2P200 = New iDB2Command(strSqlXXC2P200, conADL)
                    AddC2P200Parameters(cmdSelectXXC2P200, detailID, headerID, 101, "AddressLine2", _de.AddressLine2)
                    cmdSelectXXC2P200.ExecuteNonQuery()

                    detailID -= 1
                    cmdSelectXXC2P200 = New iDB2Command(strSqlXXC2P200, conADL)
                    AddC2P200Parameters(cmdSelectXXC2P200, detailID, headerID, 102, "AddressLine3", _de.AddressLine3)
                    cmdSelectXXC2P200.ExecuteNonQuery()

                    detailID -= 1
                    cmdSelectXXC2P200 = New iDB2Command(strSqlXXC2P200, conADL)
                    AddC2P200Parameters(cmdSelectXXC2P200, detailID, headerID, 103, "AddressLine4", _de.AddressLine4)
                    cmdSelectXXC2P200.ExecuteNonQuery()

                    detailID -= 1
                    cmdSelectXXC2P200 = New iDB2Command(strSqlXXC2P200, conADL)
                    AddC2P200Parameters(cmdSelectXXC2P200, detailID, headerID, 104, "AddressLine5", _de.AddressLine5)
                    cmdSelectXXC2P200.ExecuteNonQuery()

                    detailID -= 1
                    cmdSelectXXC2P200 = New iDB2Command(strSqlXXC2P200, conADL)
                    AddC2P200Parameters(cmdSelectXXC2P200, detailID, headerID, 105, "PostCode", _de.PostCode)
                    cmdSelectXXC2P200.ExecuteNonQuery()

                    detailID -= 1
                    cmdSelectXXC2P200 = New iDB2Command(strSqlXXC2P200, conADL)
                    AddC2P200Parameters(cmdSelectXXC2P200, detailID, headerID, 200, "ContactTitle", _de.ContactTitle)
                    cmdSelectXXC2P200.ExecuteNonQuery()

                    detailID -= 1
                    cmdSelectXXC2P200 = New iDB2Command(strSqlXXC2P200, conADL)
                    AddC2P200Parameters(cmdSelectXXC2P200, detailID, headerID, 202, "ContactSurname", _de.ContactSurname)
                    cmdSelectXXC2P200.ExecuteNonQuery()

                    detailID -= 1
                    cmdSelectXXC2P200 = New iDB2Command(strSqlXXC2P200, conADL)
                    AddC2P200Parameters(cmdSelectXXC2P200, detailID, headerID, 203, "ContactForename", _de.ContactForename)
                    cmdSelectXXC2P200.ExecuteNonQuery()

                    detailID -= 1
                    cmdSelectXXC2P200 = New iDB2Command(strSqlXXC2P200, conADL)
                    AddC2P200Parameters(cmdSelectXXC2P200, detailID, headerID, 204, "Telephone1", _de.HomeTelephoneNumber)
                    cmdSelectXXC2P200.ExecuteNonQuery()

                    detailID -= 1
                    cmdSelectXXC2P200 = New iDB2Command(strSqlXXC2P200, conADL)
                    AddC2P200Parameters(cmdSelectXXC2P200, detailID, headerID, 206, "Telephone2", _de.WorkTelephoneNumber)
                    cmdSelectXXC2P200.ExecuteNonQuery()

                    detailID -= 1
                    cmdSelectXXC2P200 = New iDB2Command(strSqlXXC2P200, conADL)
                    AddC2P200Parameters(cmdSelectXXC2P200, detailID, headerID, 208, "Telephone3", _de.MobileNumber)
                    cmdSelectXXC2P200.ExecuteNonQuery()

                    detailID -= 1
                    cmdSelectXXC2P200 = New iDB2Command(strSqlXXC2P200, conADL)
                    AddC2P200Parameters(cmdSelectXXC2P200, detailID, headerID, 227, "TelephoneEmailAddress", _de.EmailAddress)
                    cmdSelectXXC2P200.ExecuteNonQuery()

                    If decs.CreationType.Equals("REGISTRATION") Then
                        detailID -= 1
                        cmdSelectXXC2P200 = New iDB2Command(strSqlXXC2P200, conADL)
                        AddC2P200Parameters(cmdSelectXXC2P200, detailID, headerID, 500, "Attribute", "ADE-DATAP")
                        cmdSelectXXC2P200.ExecuteNonQuery()
                        detailID -= 1
                        cmdSelectXXC2P200 = New iDB2Command(strSqlXXC2P200, conADL)
                        AddC2P200Parameters(cmdSelectXXC2P200, detailID, headerID, 501, "AttributeAct", "I")
                        cmdSelectXXC2P200.ExecuteNonQuery()
                    End If

                Case Is = "CONTACTUS"

                    cmdSelectXXC2P200 = New iDB2Command(strSqlXXC2P200, conADL)
                    'AddC2P200Parameters(cmdSelectXXC2P200, detailID, headerID, 8, "ThirdPartyRef", _de.ThirdPartyContactRef)
                    AddC2P200Parameters(cmdSelectXXC2P200, detailID, headerID, 8, "ThirdPartyRef", _
                            _de.ThirdPartyCompanyRef1Supplement & _de.ThirdPartyCompanyRef1.ToString.PadLeft(9, "0"))
                    cmdSelectXXC2P200.ExecuteNonQuery()

                    detailID -= 1
                    cmdSelectXXC2P200 = New iDB2Command(strSqlXXC2P200, conADL)
                    AddC2P200Parameters(cmdSelectXXC2P200, detailID, headerID, 600, "ActionCode", _de.ActionCodeFixed)
                    cmdSelectXXC2P200.ExecuteNonQuery()

                    detailID -= 1
                    cmdSelectXXC2P200 = New iDB2Command(strSqlXXC2P200, conADL)
                    AddC2P200Parameters(cmdSelectXXC2P200, detailID, headerID, 601, "ActionStatus", _de.ActionStatus)
                    cmdSelectXXC2P200.ExecuteNonQuery()

                    detailID -= 1
                    cmdSelectXXC2P200 = New iDB2Command(strSqlXXC2P200, conADL)
                    AddC2P200Parameters(cmdSelectXXC2P200, detailID, headerID, 602, "ActionPty", _de.ActionPty)
                    cmdSelectXXC2P200.ExecuteNonQuery()

                    detailID -= 1
                    cmdSelectXXC2P200 = New iDB2Command(strSqlXXC2P200, conADL)
                    AddC2P200Parameters(cmdSelectXXC2P200, detailID, headerID, 603, "ActionAgent", _de.ActionAgent)
                    cmdSelectXXC2P200.ExecuteNonQuery()

                    detailID -= 1
                    cmdSelectXXC2P200 = New iDB2Command(strSqlXXC2P200, conADL)
                    AddC2P200Parameters(cmdSelectXXC2P200, detailID, headerID, 604, "ActionComment1", _de.ActionComment01)
                    cmdSelectXXC2P200.ExecuteNonQuery()

                    detailID -= 1
                    cmdSelectXXC2P200 = New iDB2Command(strSqlXXC2P200, conADL)
                    AddC2P200Parameters(cmdSelectXXC2P200, detailID, headerID, 605, "ActionComment2", _de.ActionComment02)
                    cmdSelectXXC2P200.ExecuteNonQuery()

                    detailID -= 1
                    cmdSelectXXC2P200 = New iDB2Command(strSqlXXC2P200, conADL)
                    AddC2P200Parameters(cmdSelectXXC2P200, detailID, headerID, 606, "ActionComment3", _de.ActionComment03)
                    cmdSelectXXC2P200.ExecuteNonQuery()

                    detailID -= 1
                    cmdSelectXXC2P200 = New iDB2Command(strSqlXXC2P200, conADL)
                    AddC2P200Parameters(cmdSelectXXC2P200, detailID, headerID, 607, "ActionComment4", _de.ActionComment04)
                    cmdSelectXXC2P200.ExecuteNonQuery()

            End Select

            ' Upload to CRM ..
            If decs.CreationType = "REGISTRATION" Or decs.CreationType = "UPDATE" Or decs.CreationType = "CONTACTUS" Then
                Dim parmInput1, parmOutput As iDB2Parameter

                cmdUpload = New iDB2Command(strSqlUpload, conADL)

                parmInput1 = cmdUpload.Parameters.Add("@PARAM1", iDB2DbType.iDB2Char, 1024)
                parmInput1.Value = String.Empty
                parmInput1.Direction = ParameterDirection.Input


                parmOutput = cmdUpload.Parameters.Add("@PARAM2", iDB2DbType.iDB2Char, 1024)
                parmOutput.Value = String.Empty
                parmOutput.Direction = ParameterDirection.InputOutput

                cmdUpload.ExecuteNonQuery()

            End If


        Catch ex As Exception
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBCC-03"
                .HasError = True
            End With
        End Try

        Return err
    End Function

    Private Sub AddC2P200Parameters(ByRef cmd As IBM.Data.DB2.iSeries.iDB2Command, _
                                    ByVal c3c3id As Decimal, ByVal c3hdid As Decimal, _
                                    ByVal c3fdid As Decimal, ByVal c3titl As String, _
                                    ByVal c3valu As String)
        With cmd.Parameters
            .Add("@C3USER", iDB2DbType.iDB2VarChar, 10).Value = "CALLCENT"
            .Add("@C3PGMD", iDB2DbType.iDB2VarChar, 10).Value = "ADE_TALN_R"
            Dim s As String = "1" & Format(Date.Now, "yyMMdd")
            .Add("@C3UPDT", iDB2DbType.iDB2Decimal).Value = "1" & Format(Date.Now, "yyMMdd")
            .Add("@C3UPTM", iDB2DbType.iDB2Decimal).Value = Format(Date.Now, "HHmmss")
            .Add("@C3ACTR", iDB2DbType.iDB2VarChar, 1).Value = "A"
            .Add("@C3C3ID", iDB2DbType.iDB2Decimal).Value = c3c3id
            .Add("@C3HDID", iDB2DbType.iDB2Decimal).Value = c3hdid
            .Add("@C3FDID", iDB2DbType.iDB2Decimal).Value = c3fdid
            .Add("@C3TITL", iDB2DbType.iDB2VarChar, 50).Value = c3titl
            .Add("@C3VALU", iDB2DbType.iDB2VarChar, 256).Value = c3valu
            .Add("@C3VAL1", iDB2DbType.iDB2VarChar, 256).Value = ""
            .Add("@C3VAL2", iDB2DbType.iDB2VarChar, 256).Value = ""
            .Add("@C3FLG1", iDB2DbType.iDB2VarChar, 1).Value = ""
            .Add("@C3FLG2", iDB2DbType.iDB2VarChar, 1).Value = ""
            .Add("@C3CHR1", iDB2DbType.iDB2VarChar, 30).Value = ""
            .Add("@C3CHR2", iDB2DbType.iDB2VarChar, 30).Value = ""
            .Add("@C3NUM1", iDB2DbType.iDB2Decimal).Value = 0
            .Add("@C3NUM2", iDB2DbType.iDB2Decimal).Value = 0
            .Add("@C3ULN", iDB2DbType.iDB2VarChar, 3).Value = "ENG"
        End With
    End Sub

    Private Sub FormatTelephone(ByVal telephone As String, _
                                ByRef formattedTel As String, _
                                ByRef formattedStd As String)

        Dim spacePos As Integer

        'Has a telephone number been entered
        If telephone.Trim <> "" Then
            'Is there a gap between the std and the tel
            spacePos = telephone.IndexOf(" ")
            If spacePos = -1 Then
                'No blank so we must split the telephone string after position 5
                If telephone.Length >= 5 Then
                    formattedStd = telephone.Substring(0, 5)
                    formattedTel = telephone.Substring(5)
                Else
                    formattedTel = telephone
                End If

            Else
                'Extract the telephone information
                formattedStd = telephone.Substring(0, spacePos)
                formattedTel = telephone.Substring(spacePos + 1)
            End If
        End If

    End Sub

    Protected Function AccessDataBaseTALENTCRM_SetCustomer() As ErrorObj

        Dim err As New ErrorObj
        '----------------------------------------------------------------------------------
        '   Create command object
        '
        Dim cmdSELECT As iDB2Command = Nothing

        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                    "/CMIMPORT(@PARAM1, @PARAM2, @PARAM3, @PARAM4, @PARAM5)"
        Dim parmInput1, parmInput2, parmInput3, parmInput4, parmOutput As iDB2Parameter
        Dim PARMOUT As String = String.Empty

        If Not err.HasError Then
            Try

                cmdSELECT = New iDB2Command(strHEADER, conTALENTCRM)

                'parmInput1 = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 1024)
                parmInput1 = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 2048)
                parmInput1.Value = CrmCustomerManagerInput1()
                parmInput1.Direction = ParameterDirection.Input

                parmInput2 = cmdSELECT.Parameters.Add(Param2, iDB2DbType.iDB2Char, 1024)
                parmInput2.Value = CrmCustomerManagerInput2()
                parmInput2.Direction = ParameterDirection.Input

                parmInput3 = cmdSELECT.Parameters.Add(Param3, iDB2DbType.iDB2Char, 1024)
                parmInput3.Value = CrmCustomerManagerInput3()
                parmInput3.Direction = ParameterDirection.Input

                parmInput4 = cmdSELECT.Parameters.Add(Param4, iDB2DbType.iDB2Char, 2048)
                parmInput4.Value = CrmCustomerManagerInput4()
                parmInput4.Direction = ParameterDirection.Input

                parmOutput = cmdSELECT.Parameters.Add(Param5, iDB2DbType.iDB2Char, 1024)
                parmOutput.Value = String.Empty
                parmOutput.Direction = ParameterDirection.InputOutput

                TalentCommonLog("AccessDataBaseTALENTCRM", _de.CustomerNumber, "Backend Request: strHEADER=" & strHEADER & ", parm1=" & parmInput1.Value & ", parm2=" & parmInput2.Value & ", parm3=" & parmInput3.Value & ", parm4=" & parmInput4.Value)

                cmdSELECT.ExecuteNonQuery()
                PARMOUT = cmdSELECT.Parameters(Param5).Value.ToString

                TalentCommonLog("AccessDataBaseTALENTCRM", _de.CustomerNumber, "Backend Response: PARMOUT=" & PARMOUT)

            Catch ex As Exception
                Const strError As String = "Error during database access"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TACDBCC-01"
                    .HasError = True
                End With
            End Try
        End If
        Return err
    End Function

    Private Function CrmCustomerManagerInput1() As String

        Dim myString As String
        Dim processFlags As String = String.Empty

        myString = Utilities.FixStringLength(Settings.Company, 10) & _
                   Utilities.FixStringLength(_de.CustomerID, 13) & _
                   Utilities.FixStringLength(_de.ContactID, 13) & _
                   Utilities.FixStringLength(_de.CustomerNumber, 12) & _
                   Utilities.FixStringLength(_de.BranchCode, 3) & _
                   Utilities.FixStringLength(_de.Action, 1) & _
                   Utilities.FixStringLength(_de.ManualDeDupe, 1) & _
                   Utilities.FixStringLength(_de.ManualDeDupeBranch, 3) & _
                   Utilities.FixStringLength(_de.ThirdPartyContactRef, 60) & _
                   Utilities.FixStringLength(_de.DateFormat, 1) & _
                   Utilities.FixStringLength(_de.ThirdPartyCompanyRef1, 29) & _
                   Utilities.FixStringLength(_de.ThirdPartyCompanyRef2, 29) & _
                   Utilities.FixStringLength(_de.AddressLine1, 40) & _
                   Utilities.FixStringLength(_de.AddressLine2, 40) & _
                   Utilities.FixStringLength(_de.AddressLine3, 40) & _
                   Utilities.FixStringLength(_de.AddressLine4, 40) & _
                   Utilities.FixStringLength(_de.AddressLine5, 20) & _
                   Utilities.FixStringLength(_de.PostCode, 10) & _
                   Utilities.FixStringLength(_de.CustomerDesc1, 10) & _
                   Utilities.FixStringLength(_de.CustomerDesc2, 10) & _
                   Utilities.FixStringLength(_de.CustomerDesc3, 10) & _
                   Utilities.FixStringLength(_de.CustomerDesc4, 10) & _
                   Utilities.FixStringLength(_de.CustomerDesc5, 10) & _
                   Utilities.FixStringLength(_de.CustomerDesc6, 10) & _
                   Utilities.FixStringLength(_de.CustomerDesc7, 10) & _
                   Utilities.FixStringLength(_de.CustomerDesc8, 10) & _
                   Utilities.FixStringLength(_de.CustomerDesc9, 10) & _
                   Utilities.FixStringLength(_de.OwningAgent, 10) & _
                   Utilities.FixStringLength(_de.WebAddress, 60) & _
                   Utilities.FixStringLength(_de.CompanyName, 40) & _
                   Utilities.FixStringLength(_de.ContactTitle, 4) & _
                   Utilities.FixStringLength(_de.ContactInitials, 3) & _
                   Utilities.FixStringLength(_de.ContactSurname, 30) & _
                   Utilities.FixStringLength(_de.ContactForename, 15) & _
                   Utilities.FixStringLength(_de.HomeTelephoneNumber, 30) & _
                   Utilities.FixStringLength(_de.ContactViaHomeTelephone, 1) & _
                   Utilities.FixStringLength(_de.WorkTelephoneNumber, 30) & _
                   Utilities.FixStringLength(_de.ContactViaWorkTelephone, 1) & _
                   Utilities.FixStringLength(_de.MobileNumber, 30) & _
                   Utilities.FixStringLength(_de.ContactViaMobile, 1) & _
                   Utilities.FixStringLength(_de.Telephone4, 30) & _
                   Utilities.FixStringLength(_de.ContactViaTelephone4, 1) & _
                   Utilities.FixStringLength(_de.Telephone5, 30) & _
                   Utilities.FixStringLength(_de.ContactViaTelephone5, 1) & _
                   Utilities.FixStringLength(_de.ContactCaptionA, 10) & _
                   Utilities.FixStringLength(_de.ContactCaptionB, 10) & _
                   Utilities.FixStringLength(_de.ContactCaptionC, 10) & _
                   Utilities.FixStringLength(_de.ContactCaptionD, 10) & _
                   Utilities.FixStringLength(_de.ContactCaptionE, 10) & _
                   Utilities.FixStringLength(_de.ContactCaptionF, 10) & _
                   Utilities.FixStringLength(_de.ContactCaptionG, 10) & _
                   Utilities.FixStringLength(_de.ContactCaptionH, 10) & _
                   Utilities.FixStringLength(_de.ContactCaptionI, 10) & _
                   Utilities.FixStringLength(_de.ContactCaptionJ, 10) & _
                   Utilities.FixStringLength(_de.ContactCaptionK, 10) & _
                   Utilities.FixStringLength(_de.ContactCaptionL, 10) & _
                   Utilities.FixStringLength(_de.ContactViaMail, 1) & _
                   Utilities.FixStringLength(_de.EmailAddress, 60) & _
                   Utilities.FixStringLength(_de.ContactViaMail, 1) & _
                   Utilities.FixStringLength(_de.DateBirth, 8) & _
                   Utilities.FixStringLength(_de.Gender, 1) & _
                   Utilities.FixStringLength(_de.VatCode, 12) & _
                   Utilities.FixStringLength(_de.CompanySLNumber1, 15) & _
                   Utilities.FixStringLength(_de.CompanySLNumber2, 15) & _
                   Utilities.FixStringLength(_de.CampaignCode, 12) & _
                   Utilities.FixStringLength(_de.CampaignResult, 10) & _
                   Utilities.FixStringLength(_de.CampaignEventCode, 12) & _
                   Utilities.FixStringLength(_de.PositionInCompany, 30)

        processFlags = Utilities.FixStringLength(_de.ProcessAddressLine1, 1) & _
                   Utilities.FixStringLength(_de.ProcessAddressLine2, 1) & _
                   Utilities.FixStringLength(_de.ProcessAddressLine3, 1) & _
                   Utilities.FixStringLength(_de.ProcessAddressLine4, 1) & _
                   Utilities.FixStringLength(_de.ProcessAddressLine5, 1) & _
                   Utilities.FixStringLength(_de.ProcessPostCode, 1) & _
                   Utilities.FixStringLength(_de.ProcessCustomerDesc1, 1) & _
                   Utilities.FixStringLength(_de.ProcessCustomerDesc2, 1) & _
                   Utilities.FixStringLength(_de.ProcessCustomerDesc3, 1) & _
                   Utilities.FixStringLength(_de.ProcessCustomerDesc4, 1) & _
                   Utilities.FixStringLength(_de.ProcessCustomerDesc5, 1) & _
                   Utilities.FixStringLength(_de.ProcessCustomerDesc6, 1) & _
                   Utilities.FixStringLength(_de.ProcessCustomerDesc7, 1) & _
                   Utilities.FixStringLength(_de.ProcessCustomerDesc8, 1) & _
                   Utilities.FixStringLength(_de.ProcessCustomerDesc9, 1) & _
                   Utilities.FixStringLength(_de.ProcessOwningAgent, 1) & _
                   Utilities.FixStringLength(_de.ProcessWebAddress, 1) & _
                   Utilities.FixStringLength(_de.ProcessCompanyName, 1) & _
                   Utilities.FixStringLength(_de.ProcessContactTitle, 1) & _
                   Utilities.FixStringLength(_de.ProcessContactInitials, 1) & _
                   Utilities.FixStringLength(_de.ProcessContactSurname, 1) & _
                   Utilities.FixStringLength(_de.ProcessContactForename, 1) & _
                   Utilities.FixStringLength(_de.ProcessHomeTelephoneNumber, 1) & _
                   Utilities.FixStringLength(_de.ProcessContactViaHomeTelephone, 1) & _
                   Utilities.FixStringLength(_de.ProcessWorkTelephoneNumber, 1) & _
                   Utilities.FixStringLength(_de.ProcessContactViaWorkTelephone, 1) & _
                   Utilities.FixStringLength(_de.ProcessMobileNumber, 1) & _
                   Utilities.FixStringLength(_de.ProcessContactViaMobile, 1) & _
                   Utilities.FixStringLength(_de.ProcessTelephone4, 1) & _
                   Utilities.FixStringLength(_de.ProcessContactViaTelephone4, 1) & _
                   Utilities.FixStringLength(_de.ProcessTelephone5, 1) & _
                   Utilities.FixStringLength(_de.ProcessContactViaTelephone5, 1) & _
                   Utilities.FixStringLength(_de.ProcessContactCaptionA, 1) & _
                   Utilities.FixStringLength(_de.ProcessContactCaptionB, 1) & _
                   Utilities.FixStringLength(_de.ProcessContactCaptionC, 1) & _
                   Utilities.FixStringLength(_de.ProcessContactCaptionD, 1) & _
                   Utilities.FixStringLength(_de.ProcessContactCaptionE, 1) & _
                   Utilities.FixStringLength(_de.ProcessContactCaptionF, 1) & _
                   Utilities.FixStringLength(_de.ProcessContactCaptionG, 1) & _
                   Utilities.FixStringLength(_de.ProcessContactCaptionH, 1) & _
                   Utilities.FixStringLength(_de.ProcessContactCaptionI, 1) & _
                   Utilities.FixStringLength(_de.ProcessContactCaptionJ, 1) & _
                   Utilities.FixStringLength(_de.ProcessContactCaptionK, 1) & _
                   Utilities.FixStringLength(_de.ProcessContactCaptionL, 1) & _
                   Utilities.FixStringLength(_de.ProcessContactViaMail, 1) & _
                   Utilities.FixStringLength(_de.ProcessEmailAddress, 1) & _
                   Utilities.FixStringLength(_de.ProcessContactViaMail, 1) & _
                   Utilities.FixStringLength(_de.ProcessDateBirth, 1) & _
                   Utilities.FixStringLength(_de.ProcessGender, 1) & _
                   Utilities.FixStringLength(_de.ProcessVatCode, 1) & _
                   Utilities.FixStringLength(_de.ProcessCompanySLNumber1, 1) & _
                   Utilities.FixStringLength(_de.ProcessCompanySLNumber2, 1) & _
                   Utilities.FixStringLength(_de.ProcessCampaignCode, 1) & _
                   Utilities.FixStringLength(_de.ProcessCampaignResult, 1) & _
                   Utilities.FixStringLength(_de.ProcessCampaignEventCode, 1) & _
                   Utilities.FixStringLength(_de.ProcessPositionInCompany, 1)

        '-----------------------------------------
        ' Insert process flags at end of parameter
        '-----------------------------------------
        myString = Utilities.FixStringLength(myString, 1948) & processFlags
        '---------------------------------------
        ' Remove carriage returns and line feeds
        '---------------------------------------
        myString = myString.Replace(Chr(10), " ")
        myString = myString.Replace(Chr(13), " ")
        Return myString
    End Function

    Private Function CrmCustomerManagerInput2() As String

        Dim myString As String

        myString = Utilities.FixStringLength(_de.Salutation, 20) & _
                   Utilities.FixStringLength(_de.SLNumber1, 15) & _
                   Utilities.FixStringLength(_de.SLNumber2, 15) & _
                   Utilities.FixStringLength(_de.ProcessSalutation, 1) & _
                   Utilities.FixStringLength(_de.ProcessSLNumber1, 1) & _
                   Utilities.FixStringLength(_de.ProcessSLNumber2, 1)

        Return myString
    End Function

    Private Function CrmCustomerManagerInput3() As String

        Dim myString As String

        myString = Utilities.FixStringLength(_de.Attribute01, 12) & _
                   Utilities.FixStringLength(_de.Attribute01Action, 1) & _
                   Utilities.FixStringLength(_de.Attribute02, 12) & _
                   Utilities.FixStringLength(_de.Attribute02Action, 1) & _
                   Utilities.FixStringLength(_de.Attribute03, 12) & _
                   Utilities.FixStringLength(_de.Attribute03Action, 1) & _
                   Utilities.FixStringLength(_de.Attribute04, 12) & _
                   Utilities.FixStringLength(_de.Attribute04Action, 1) & _
                   Utilities.FixStringLength(_de.Attribute05, 12) & _
                   Utilities.FixStringLength(_de.Attribute05Action, 1) & _
                   Utilities.FixStringLength(_de.Attribute06, 12) & _
                   Utilities.FixStringLength(_de.Attribute06Action, 1) & _
                   Utilities.FixStringLength(_de.Attribute07, 12) & _
                   Utilities.FixStringLength(_de.Attribute07Action, 1) & _
                   Utilities.FixStringLength(_de.Attribute08, 12) & _
                   Utilities.FixStringLength(_de.Attribute08Action, 1) & _
                   Utilities.FixStringLength(_de.Attribute09, 12) & _
                   Utilities.FixStringLength(_de.Attribute09Action, 1) & _
                   Utilities.FixStringLength(_de.Attribute10, 12) & _
                   Utilities.FixStringLength(_de.Attribute10Action, 1) & _
                   Utilities.FixStringLength(_de.Attribute11, 12) & _
                   Utilities.FixStringLength(_de.Attribute11Action, 1) & _
                   Utilities.FixStringLength(_de.Attribute12, 12) & _
                   Utilities.FixStringLength(_de.Attribute12Action, 1) & _
                   Utilities.FixStringLength(_de.Attribute13, 12) & _
                   Utilities.FixStringLength(_de.Attribute13Action, 1) & _
                   Utilities.FixStringLength(_de.Attribute14, 12) & _
                   Utilities.FixStringLength(_de.Attribute14Action, 1) & _
                   Utilities.FixStringLength(_de.Attribute15, 12) & _
                   Utilities.FixStringLength(_de.Attribute15Action, 1) & _
                   Utilities.FixStringLength(_de.Attribute16, 12) & _
                   Utilities.FixStringLength(_de.Attribute16Action, 1) & _
                   Utilities.FixStringLength(_de.Attribute17, 12) & _
                   Utilities.FixStringLength(_de.Attribute17Action, 1) & _
                   Utilities.FixStringLength(_de.Attribute18, 12) & _
                   Utilities.FixStringLength(_de.Attribute18Action, 1) & _
                   Utilities.FixStringLength(_de.Attribute19, 12) & _
                   Utilities.FixStringLength(_de.Attribute19Action, 1) & _
                   Utilities.FixStringLength(_de.Attribute20, 12) & _
                   Utilities.FixStringLength(_de.Attribute20Action, 1) & _
                   Utilities.FixStringLength(_de.Attribute21, 12) & _
                   Utilities.FixStringLength(_de.Attribute21Action, 1) & _
                   Utilities.FixStringLength(_de.Attribute22, 12) & _
                   Utilities.FixStringLength(_de.Attribute22Action, 1) & _
                   Utilities.FixStringLength(_de.Attribute23, 12) & _
                   Utilities.FixStringLength(_de.Attribute23Action, 1) & _
                   Utilities.FixStringLength(_de.Attribute24, 12) & _
                   Utilities.FixStringLength(_de.Attribute24Action, 1) & _
                   Utilities.FixStringLength(_de.Attribute25, 12) & _
                   Utilities.FixStringLength(_de.Attribute25Action, 1) & _
                   Utilities.FixStringLength(_de.Attribute26, 12) & _
                   Utilities.FixStringLength(_de.Attribute26Action, 1) & _
                   Utilities.FixStringLength(_de.Attribute27, 12) & _
                   Utilities.FixStringLength(_de.Attribute27Action, 1) & _
                   Utilities.FixStringLength(_de.Attribute28, 12) & _
                   Utilities.FixStringLength(_de.Attribute28Action, 1) & _
                   Utilities.FixStringLength(_de.Attribute29, 12) & _
                   Utilities.FixStringLength(_de.Attribute29Action, 1) & _
                   Utilities.FixStringLength(_de.Attribute30, 12) & _
                   Utilities.FixStringLength(_de.Attribute30Action, 1) & _
                   Utilities.FixStringLength(_de.Attribute31, 12) & _
                   Utilities.FixStringLength(_de.Attribute31Action, 1) & _
                   Utilities.FixStringLength(_de.Attribute32, 12) & _
                   Utilities.FixStringLength(_de.Attribute32Action, 1) & _
                   Utilities.FixStringLength(_de.Attribute33, 12) & _
                   Utilities.FixStringLength(_de.Attribute33Action, 1) & _
                   Utilities.FixStringLength(_de.Attribute34, 12) & _
                   Utilities.FixStringLength(_de.Attribute34Action, 1) & _
                   Utilities.FixStringLength(_de.Attribute35, 12) & _
                   Utilities.FixStringLength(_de.Attribute35Action, 1) & _
                   Utilities.FixStringLength(_de.Attribute36, 12) & _
                   Utilities.FixStringLength(_de.Attribute36Action, 1) & _
                   Utilities.FixStringLength(_de.Attribute37, 12) & _
                   Utilities.FixStringLength(_de.Attribute37Action, 1) & _
                   Utilities.FixStringLength(_de.Attribute38, 12) & _
                   Utilities.FixStringLength(_de.Attribute38Action, 1) & _
                   Utilities.FixStringLength(_de.Attribute39, 12) & _
                   Utilities.FixStringLength(_de.Attribute39Action, 1) & _
                   Utilities.FixStringLength(_de.Attribute40, 12) & _
                   Utilities.FixStringLength(_de.Attribute40Action, 1) & _
                   Utilities.FixStringLength(_de.Attribute41, 12) & _
                   Utilities.FixStringLength(_de.Attribute41Action, 1) & _
                   Utilities.FixStringLength(_de.Attribute42, 12) & _
                   Utilities.FixStringLength(_de.Attribute42Action, 1) & _
                   Utilities.FixStringLength(_de.Attribute43, 12) & _
                   Utilities.FixStringLength(_de.Attribute43Action, 1) & _
                   Utilities.FixStringLength(_de.Attribute44, 12) & _
                   Utilities.FixStringLength(_de.Attribute44Action, 1) & _
                   Utilities.FixStringLength(_de.Attribute45, 12) & _
                   Utilities.FixStringLength(_de.Attribute45Action, 1) & _
                   Utilities.FixStringLength(_de.Attribute46, 12) & _
                   Utilities.FixStringLength(_de.Attribute46Action, 1) & _
                   Utilities.FixStringLength(_de.Attribute47, 12) & _
                   Utilities.FixStringLength(_de.Attribute47Action, 1) & _
                   Utilities.FixStringLength(_de.Attribute48, 12) & _
                   Utilities.FixStringLength(_de.Attribute48Action, 1) & _
                   Utilities.FixStringLength(_de.Attribute49, 12) & _
                   Utilities.FixStringLength(_de.Attribute49Action, 1) & _
                   Utilities.FixStringLength(_de.Attribute50, 12) & _
                   Utilities.FixStringLength(_de.Attribute50Action, 1) & _
                   Utilities.FixStringLength(_de.Attribute51, 12) & _
                   Utilities.FixStringLength(_de.Attribute51Action, 1) & _
                   Utilities.FixStringLength(_de.Attribute52, 12) & _
                   Utilities.FixStringLength(_de.Attribute52Action, 1) & _
                   Utilities.FixStringLength(_de.Attribute53, 12) & _
                   Utilities.FixStringLength(_de.Attribute53Action, 1) & _
                   Utilities.FixStringLength(_de.Attribute54, 12) & _
                   Utilities.FixStringLength(_de.Attribute54Action, 1) & _
                   Utilities.FixStringLength(_de.Attribute55, 12) & _
                   Utilities.FixStringLength(_de.Attribute55Action, 1) & _
                   Utilities.FixStringLength(_de.Attribute56, 12) & _
                   Utilities.FixStringLength(_de.Attribute56Action, 1) & _
                   Utilities.FixStringLength(_de.Attribute57, 12) & _
                   Utilities.FixStringLength(_de.Attribute57Action, 1) & _
                   Utilities.FixStringLength(_de.Attribute58, 12) & _
                   Utilities.FixStringLength(_de.Attribute58Action, 1) & _
                   Utilities.FixStringLength(_de.Attribute59, 12) & _
                   Utilities.FixStringLength(_de.Attribute59Action, 1) & _
                   Utilities.FixStringLength(_de.Attribute60, 12) & _
                   Utilities.FixStringLength(_de.Attribute60Action, 1)

        myString = myString.Replace(Chr(10), " ")
        myString = myString.Replace(Chr(13), " ")
        Return myString

    End Function

    Private Function CrmCustomerManagerInput4() As String

        Dim myString As String

        myString = Utilities.FixStringLength(_de.ActionCode, 12) & _
                   Utilities.FixStringLength(_de.ActionStatus, 1) & _
                   Utilities.FixStringLength(_de.ActionPty, 1) & _
                   Utilities.FixStringLength(_de.ActionAgent, 10) & _
                   Utilities.FixStringLength(_de.ActionComment01, 60) & _
                   Utilities.FixStringLength(_de.ActionComment02, 60) & _
                   Utilities.FixStringLength(_de.ActionComment03, 60) & _
                   Utilities.FixStringLength(_de.ActionComment04, 60) & _
                   Utilities.FixStringLength(_de.ActionComment05, 60) & _
                   Utilities.FixStringLength(_de.ActionComment06, 60) & _
                   Utilities.FixStringLength(_de.ActionComment07, 60) & _
                   Utilities.FixStringLength(_de.ActionComment08, 60) & _
                   Utilities.FixStringLength(_de.ActionComment09, 60) & _
                   Utilities.FixStringLength(_de.ActionComment10, 60) & _
                   Utilities.FixStringLength(_de.ActionComment11, 60) & _
                   Utilities.FixStringLength(_de.ActionComment12, 60) & _
                   Utilities.FixStringLength(_de.ActionComment13, 60) & _
                   Utilities.FixStringLength(_de.ActionComment14, 60) & _
                   Utilities.FixStringLength(_de.ActionComment15, 60) & _
                   Utilities.FixStringLength(_de.ActionComment16, 60) & _
                   Utilities.FixStringLength(_de.ActionDate, 8) & _
                   Utilities.FixStringLength(_de.ActionID, 13) & _
                   Utilities.FixStringLength(_de.ActionSubject, 15) & _
                   Utilities.FixStringLength(_de.ActionMemo, 15) & _
                   Utilities.FixStringLength(_de.ActionDepartmentID, 13) & _
                   Utilities.FixStringLength(_de.ActionProductID, 13) & _
                   Utilities.FixStringLength(_de.ActionProjectID, 13) & _
                   Utilities.FixStringLength(_de.ActionCampaignID, 13) & _
                   Utilities.FixStringLength(_de.ActionField1, 10) & _
                   Utilities.FixStringLength(_de.ActionField2, 10) & _
                   Utilities.FixStringLength(_de.ActionField3, 10) & _
                   Utilities.FixStringLength(_de.ActionField4, 10) & _
                   Utilities.FixStringLength(_de.ActionField5, 10)
        myString = myString.Replace(Chr(10), " ")
        myString = myString.Replace(Chr(13), " ")

        Return myString

    End Function

    Private Function CrmCustomerManagerInput5() As String

        Dim myString As String

        myString = Utilities.FixStringLength(_de.NoteCode, 12) & _
                   Utilities.FixStringLength(_de.NoteComment01, 60) & _
                   Utilities.FixStringLength(_de.NoteComment02, 60) & _
                   Utilities.FixStringLength(_de.NoteComment03, 60) & _
                   Utilities.FixStringLength(_de.NoteComment04, 60) & _
                   Utilities.FixStringLength(_de.NoteComment05, 60) & _
                   Utilities.FixStringLength(_de.NoteComment06, 60) & _
                   Utilities.FixStringLength(_de.NoteComment07, 60) & _
                   Utilities.FixStringLength(_de.NoteComment08, 60) & _
                   Utilities.FixStringLength(_de.NoteComment09, 60) & _
                   Utilities.FixStringLength(_de.NoteComment10, 60) & _
                   Utilities.FixStringLength(_de.NoteComment11, 60) & _
                   Utilities.FixStringLength(_de.NoteComment12, 60) & _
                   Utilities.FixStringLength(_de.NoteComment13, 60) & _
                   Utilities.FixStringLength(_de.NoteComment14, 60) & _
                   Utilities.FixStringLength(_de.NoteComment15, 60) & _
                   Utilities.FixStringLength(_de.NoteComment16, 60)
        myString = myString.Replace(Chr(10), " ")
        myString = myString.Replace(Chr(13), " ")
        Return myString

    End Function

    Private Function AccessDatabaseWS120R() As ErrorObj

        Dim err As New ErrorObj
        ResultDataSet = New DataSet
        Dim dRow As DataRow = Nothing
        Dim PARAMOUT As String = String.Empty
        Dim returnCode As String = String.Empty
        Dim newPassword As String = String.Empty

        ' If generate password then generate a new password first.
        Dim DtGeneratePasswordResults As New DataTable("GeneratePasswordResults")
        If Settings.ModuleName = GeneratePassword Then
            Dim passwordLength As Integer = 10
            If _de.PasswordLength <> 0 Then
                passwordLength = _de.PasswordLength
            End If
            newPassword = Utilities.RandomString(passwordLength)

            _de.Password = newPassword

            'Create the new password table
            ResultDataSet.Tables.Add(DtGeneratePasswordResults)
            AddPasswordResultsColumns(DtGeneratePasswordResults)

        End If

        ' For verify or generate, check if password is to be hashed
        If Settings.ModuleName = VerifyPassword OrElse Settings.ModuleName = GeneratePassword Then

            If Settings.EcommerceModuleDefaultsValues IsNot Nothing AndAlso Settings.EcommerceModuleDefaultsValues.UseEncryptedPassword = True Then
                If Not _de.Password Is String.Empty AndAlso _de.HashedPassword Is String.Empty Then
                    Dim passHash As New Talent.Common.PasswordHash
                    _de.HashedPassword = passHash.HashSalt(_de.Password, Settings.EcommerceModuleDefaultsValues.SaltString)
                End If
            End If
        End If

        'Create the Status data table
        Dim DtStatusResults As New DataTable("StatusResults")
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        Dim DtPasswordResults As New DataTable("RetrievePasswordResults")
        If Me.Settings.ModuleName = RetrievePassword Then
            'Create the new password table
            ResultDataSet.Tables.Add(DtPasswordResults)
            AddPasswordResultsColumns(DtPasswordResults)
        End If

        Dim DtVerifyPasswordResults As New DataTable("VerifyPasswordResults")
        If Me.Settings.ModuleName = VerifyPassword Then
            'Create the new password table
            ResultDataSet.Tables.Add(DtVerifyPasswordResults)
            With DtVerifyPasswordResults.Columns
                .Add("UserName", GetType(String))
                .Add("Success", GetType(Boolean))
                .Add("EncryptedPassword", GetType(String))
            End With
        End If

        Try

            'Call WS120R
            PARAMOUT = CallWS120R()

            'Extract the return code
            returnCode = PARAMOUT.Substring(1021, 2)

            'Set the response data
            dRow = Nothing
            dRow = DtStatusResults.NewRow
            If returnCode.Trim = "" Then
                dRow("ErrorOccurred") = ""
                dRow("ReturnCode") = ""
                If Me.Settings.ModuleName = RetrievePassword Then
                    'Assuming a hashed password is returned
                    If Trim(PARAMOUT.Substring(196, 150)).Length > 0 Then
                        newPassword = Trim(PARAMOUT.Substring(196, 150))
                    Else
                        newPassword = PARAMOUT.Substring(13, 60)
                    End If
                    Dim pRow As DataRow = DtPasswordResults.NewRow
                    If _de.UserName <> String.Empty Then
                        pRow("UserName") = _de.UserName
                    Else
                        pRow("UserName") = PARAMOUT.Substring(1, 12)
                    End If
                    pRow("Password") = newPassword
                    DtPasswordResults.Rows.Add(pRow)
                End If
                If Me.Settings.ModuleName = VerifyPassword Then
                    Dim pRow As DataRow = DtVerifyPasswordResults.NewRow
                    If _de.UserName <> String.Empty Then
                        pRow("UserName") = _de.UserName
                    Else
                        pRow("UserName") = PARAMOUT.Substring(1, 12)
                    End If
                    pRow("Success") = True
                    pRow("EncryptedPassword") = PARAMOUT.Substring(196, 150)
                    DtVerifyPasswordResults.Rows.Add(pRow)
                End If
                If Me.Settings.ModuleName = GeneratePassword Then
                    Dim pRow As DataRow = DtGeneratePasswordResults.NewRow
                    If _de.UserName <> String.Empty Then
                        pRow("UserName") = _de.UserName
                    Else
                        pRow("UserName") = PARAMOUT.Substring(1, 12)
                    End If
                    If _de.UseEncryptedPassword AndAlso _de.EncryptionType = "1" Then
                        ' pRow("Password") = _de.HashedPassword
                        pRow("Password") = newPassword
                    Else
                        pRow("Password") = newPassword
                    End If

                    DtGeneratePasswordResults.Rows.Add(pRow)
                End If

            Else
                dRow("ErrorOccurred") = "E"
                dRow("ReturnCode") = returnCode
                If Me.Settings.ModuleName = VerifyPassword Then
                    Dim pRow As DataRow = DtVerifyPasswordResults.NewRow
                    If _de.UserName <> String.Empty Then
                        pRow("UserName") = _de.UserName
                    Else
                        pRow("UserName") = PARAMOUT.Substring(1, 12)
                    End If
                    pRow("Success") = False
                    DtVerifyPasswordResults.Rows.Add(pRow)
                End If
                If Me.Settings.ModuleName = RetrievePassword Then
                    Dim pRow As DataRow = DtPasswordResults.NewRow
                    pRow("UserName") = _de.UserName
                    pRow("Password") = String.Empty
                    DtPasswordResults.Rows.Add(pRow)
                End If
                If Me.Settings.ModuleName = GeneratePassword Then
                    Dim pRow As DataRow = DtGeneratePasswordResults.NewRow
                    pRow("UserName") = _de.UserName
                    pRow("Password") = String.Empty
                    DtGeneratePasswordResults.Rows.Add(pRow)
                End If
            End If
            DtStatusResults.Rows.Add(dRow)

        Catch ex As Exception
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBCC-WS120R-01"
                .HasError = True
            End With
        End Try

        Return err

    End Function

    Private Function CallWS120R() As String

        'Create command object
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strProgram As String = "WS120R"
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                    "/" & strProgram & "(@PARAM1)"
        Dim parmIO As iDB2Parameter
        Dim PARAMOUT As String = String.Empty

        'Set the connection string
        cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)

        'Populate the parameter
        parmIO = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 1024)
        parmIO.Value = WS120RParm()
        parmIO.Direction = ParameterDirection.InputOutput

        'Execute
        TalentCommonLog("CallWS120R", _de.CustomerNumber, "Backend Request: strHEADER=" & strHEADER & ", parmIO.Value=" & parmIO.Value)

        cmdSELECT.ExecuteNonQuery()

        PARAMOUT = cmdSELECT.Parameters(Param1).Value.ToString

        TalentCommonLog("CallWS120R", _de.CustomerNumber, "Backend Response: PARAMOUT=" & PARAMOUT)

        Return PARAMOUT

    End Function

    Private Function WS120RParm() As String

        Dim myString As String
        Dim mode As String = ""

        'Set the mode parameters
        Select Case Me.Settings.ModuleName
            Case Is = VerifyPassword
                mode = "V"
            Case Is = UpdatePassword, GeneratePassword
                mode = "U"
            Case Is = ResetPassword
                mode = "R"
            Case Is = CheckCustomer
                mode = "H"
            Case Else
                mode = "V"
        End Select

        'Construct the parameter
        myString = Utilities.PadLeadingZeros(mode, 1) & _
                 Utilities.PadLeadingZeros(_de.UserName, 12) & _
                 Utilities.FixStringLength(_de.Password, 60) & _
                 Utilities.FixStringLength(_de.NewPassword, 60) & _
                 Utilities.FixStringLength(_de.EmailAddress, 60) & _
                 Utilities.FixStringLength(" ", 3) & _
                 Utilities.FixStringLength(_de.HashedPassword, 150) & _
                 Utilities.FixStringLength(_de.NewHashedPassword, 150) & _
                 Utilities.FixStringLength("", 1) & _
                 Utilities.FixStringLength(_de.PassportNumber, 20) & _
                 Utilities.FixStringLength(_de.GreenCardNumber, 20) & _
                 Utilities.FixStringLength(_de.PIN_Number, 20) & _
                 Utilities.FixStringLength(_de.SaltString, 150) & _
                 Utilities.FixStringLength("", 313) & _
                 Utilities.FixStringLength(_de.Source, 1) & _
                 Utilities.FixStringLength("", 3)

        Return myString

    End Function

    Private Function AccessDatabaseWS009C() As ErrorObj

        Dim err As New ErrorObj
        ResultDataSet = New DataSet
        Dim dRow As DataRow = Nothing
        Dim PARAMOUT As String = String.Empty
        Dim returnCode As String = String.Empty

        'Create the Status data table
        Dim DtStatusResults As New DataTable
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        Try

            'Call WS009C
            PARAMOUT = CallWS009C()

            'Extract the return code
            returnCode = PARAMOUT.Substring(0, 1) + PARAMOUT.Substring(7, 3)

            'Set the response data
            dRow = Nothing
            dRow = DtStatusResults.NewRow
            If returnCode = "0000" Then
                dRow("ErrorOccurred") = ""
                dRow("ReturnCode") = ""
            Else
                dRow("ErrorOccurred") = "E"
                dRow("ReturnCode") = returnCode
            End If
            DtStatusResults.Rows.Add(dRow)

        Catch ex As Exception
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBCC-04"
                .HasError = True
            End With
        End Try

        Return err

    End Function

    Private Function CallWS009C() As String

        'Create command object
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strProgram As String = "WS009C"
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                    "/" & strProgram & "(@PARAM1, @PARAM2, @PARAM3, @PARAM4, @PARAM5, @PARAM6, @PARAM7)"
        Dim parmInput1, parmInput2, parmInput3, parmInput4, parmInput5, parmInput6, parmIO As iDB2Parameter
        Dim PARMOUT As String = String.Empty
        Dim PARAMOUT As String = String.Empty

        'Set the connection string
        cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)

        'Populate the action parameter
        parmInput1 = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 10)
        parmInput1.Value = _de.Source + "*VERIFY"
        parmInput1.Direction = ParameterDirection.Input

        'Populate the user parameter
        parmInput2 = cmdSELECT.Parameters.Add(Param2, iDB2DbType.iDB2Char, 30)
        parmInput2.Value = _de.UserName.TrimStart("0")
        parmInput2.Direction = ParameterDirection.Input

        'Populate the user length parameter
        parmInput3 = cmdSELECT.Parameters.Add(Param3, iDB2DbType.iDB2Char, 10)
        parmInput3.Value = _de.UserName.Length.ToString.Trim
        parmInput3.Direction = ParameterDirection.Input

        'Populate the user length parameter
        parmInput4 = cmdSELECT.Parameters.Add(Param4, iDB2DbType.iDB2Char, 30)
        parmInput4.Value = _de.Password
        parmInput4.Direction = ParameterDirection.Input

        'Leave the validation list blank.  This will be sorted out later
        parmInput5 = cmdSELECT.Parameters.Add(Param5, iDB2DbType.iDB2Char, 20)
        parmInput5.Value = ""
        parmInput5.Direction = ParameterDirection.Input

        'We never use the description field
        parmInput6 = cmdSELECT.Parameters.Add(Param6, iDB2DbType.iDB2Char, 300)
        parmInput6.Value = ""
        parmInput6.Direction = ParameterDirection.Input

        'Populate the parameter
        parmIO = cmdSELECT.Parameters.Add(Param7, iDB2DbType.iDB2Char, 10)
        parmIO.Value = "0000000000"
        parmIO.Direction = ParameterDirection.InputOutput

        'Execute
        TalentCommonLog("CallWS009C", _de.CustomerNumber, "Backend Request: strHEADER=" & strHEADER & ", parmInput1.Value=" & parmInput1.Value & ", parmInput2.Value=" & parmInput2.Value & ", parmInput3.Value=" & parmInput3.Value & ", parmInput4.Value=" & parmInput4.Value & ", parmInput5.Value=" & parmInput5.Value & ", parmInput6.Value=" & parmInput6.Value)

        cmdSELECT.ExecuteNonQuery()
        PARAMOUT = cmdSELECT.Parameters(Param7).Value.ToString

        TalentCommonLog("CallWS009C", _de.CustomerNumber, "Backend Response: PARAMOUT=" & PARAMOUT)

        Return PARAMOUT


    End Function

    Private Function AccessDatabaseWS027R() As ErrorObj

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

            'Call WS007R
            PARAMOUT = CallWS027R()

            'Set the response data
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

        Catch ex As Exception
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBCC-WS027R"
                .HasError = True
            End With
        End Try

        Return err

    End Function

    Private Function CallWS027R() As String

        'Create command object
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strProgram As String = "WS027R"
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                    "/" & strProgram & "(@PARAM1)"
        Dim parmIO As iDB2Parameter
        Dim PARAMOUT As String = String.Empty

        'Set the connection string
        cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)

        'Populate the parameter
        parmIO = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 1024)
        parmIO.Value = WS027Parm()
        parmIO.Direction = ParameterDirection.InputOutput

        'Execute
        TalentCommonLog("CallWS027R", _de.CustomerNumber, "Backend Request: strHEADER=" & strHEADER & ", parmIO.Value=" & parmIO.Value)

        cmdSELECT.ExecuteNonQuery()
        PARAMOUT = cmdSELECT.Parameters(Param1).Value.ToString

        TalentCommonLog("CallWS027R", _de.CustomerNumber, "Backend Response: PARAMOUT=" & PARAMOUT)

        Return PARAMOUT

    End Function

    Private Function WS027Parm() As String

        Dim myString As String
        Dim mode As String = ""
        Dim toTheirs As String = ""

        'Set the mode parameters
        Select Case _de.FriendsAndFamilyMode
            Case Is = "V1"
                mode = "V"
            Case Is = "V2"
                mode = "V"
                toTheirs = "Y"
            Case Is = "D"
                mode = "D"
            Case Else
                mode = _de.FriendsAndFamilyMode
        End Select

        'Construct the parameter
        myString = Utilities.PadLeadingZeros(_de.FriendsAndFamilyId, 12) & _
                 Utilities.FixStringLength(mode, 1) & _
                 Utilities.FixStringLength(_de.ContactSurname, 30) & _
                 Utilities.FixStringLength(_de.PostCode, 8) & _
                 Utilities.FixStringLength(toTheirs, 1) & _
                 Utilities.FixStringLength("", 950) & _
                 Utilities.PadLeadingZeros(_de.CustomerNumber, 12) & _
                 Utilities.FixStringLength("", 6) & _
                 Utilities.FixStringLength(_de.Source, 1) & _
                 Utilities.FixStringLength("", 3)

        Return myString

    End Function

    Private Function AccessDatabaseWS026R(Optional ByVal parmWS026R As String = "") As ErrorObj

        Dim err As New ErrorObj
        If parmWS026R.Trim = "" Then
            ResultDataSet = New DataSet
        End If
        Dim sLastRecord As String = "000"
        Dim sRecordTotal As String = "000"
        Dim bMoreRecords As Boolean = True
        Dim dRow As DataRow = Nothing
        Dim PARAMOUT As String = String.Empty

        'Create the Status data table
        Dim DtStatusResults As New DataTable("StatusResults")
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With


        Try
            If _settings.ModuleName = CanMoreCustomerAssociationsCanBeAdded Then
                PARAMOUT = CallWS026R(sRecordTotal, sLastRecord)
                Dim strCanAssociationsBeAdded As String = String.Empty
                strCanAssociationsBeAdded = PARAMOUT.Substring(3047, 1).Trim

                Dim DtCanMoreAssociationsBeAdded As New DataTable
                With DtCanMoreAssociationsBeAdded.Columns
                    .Add("MoreAssociationsAdded", GetType(Boolean))
                End With

                dRow = Nothing
                dRow = DtCanMoreAssociationsBeAdded.NewRow
                If strCanAssociationsBeAdded = "N" Then
                    dRow("MoreAssociationsAdded") = False
                Else
                    dRow("MoreAssociationsAdded") = True
                End If
                DtCanMoreAssociationsBeAdded.Rows.Add(dRow)

                ResultDataSet.Tables.Add(DtCanMoreAssociationsBeAdded)

            Else

                'Create the product list data table
                Dim DtFriendsAndFamilyResults As New DataTable("FriendsAndFamily")
                With DtFriendsAndFamilyResults.Columns
                    .Add("CustomerNumber", GetType(String))
                    .Add("AssociatedCustomerNumber", GetType(String))
                    .Add("Forename", GetType(String))
                    .Add("Surname", GetType(String))
                    .Add("AddressLine1", GetType(String))
                    .Add("PostCode", GetType(String))
                    .Add("PriceBand", GetType(String))
                    .Add("ActiveFlag", GetType(String))
                    .Add("LoyalityPoints", GetType(String))
                End With

                'Loop until no more products available
                Do While bMoreRecords = True

                    'Call WS026R
                    If parmWS026R.Trim <> "" And sLastRecord = "000" Then
                        PARAMOUT = parmWS026R
                    Else

                        'Call WS016R
                        PARAMOUT = CallWS026R(sRecordTotal, sLastRecord)

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

                            'Add the status table
                            ResultDataSet.Tables.Add(DtStatusResults)

                        End If

                    End If

                    'Add the friends and family table
                    If sLastRecord = "000" Then
                        ResultDataSet.Tables.Add(DtFriendsAndFamilyResults)
                    End If

                    'No errors 
                    If PARAMOUT.Substring(3071, 1) <> "E" And PARAMOUT.Substring(3069, 2).Trim = "" Then

                        'Extract the data from the parameter
                        Dim iPosition As Integer = 0
                        Dim iCounter As Integer = 1
                        Do While iCounter <= 20

                            ' Has a product been returned
                            If PARAMOUT.Substring(iPosition, 12).Trim = "" Then
                                Exit Do
                            Else

                                'Create a new row
                                dRow = Nothing
                                dRow = DtFriendsAndFamilyResults.NewRow
                                dRow("CustomerNumber") = _de.CustomerNumber
                                dRow("AssociatedCustomerNumber") = PARAMOUT.Substring(iPosition, 12).Trim()
                                dRow("Forename") = PARAMOUT.Substring(iPosition + 12, 20).Trim()
                                dRow("Surname") = PARAMOUT.Substring(iPosition + 32, 30).Trim()
                                dRow("AddressLine1") = PARAMOUT.Substring(iPosition + 62, 30).Trim()
                                dRow("PostCode") = PARAMOUT.Substring(iPosition + 92, 8).Trim()
                                dRow("PriceBand") = PARAMOUT.Substring(iPosition + 100, 1).Trim()
                                dRow("ActiveFlag") = PARAMOUT.Substring(iPosition + 101, 1).Trim()
                                dRow("LoyalityPoints") = PARAMOUT.Substring(iPosition + 102, 5).Trim()
                                DtFriendsAndFamilyResults.Rows.Add(dRow)

                                'Increment
                                iPosition = iPosition + 150
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
            End If

        Catch ex As Exception
            ResultDataSet = Nothing
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBCC-WS026R"
                .HasError = True
            End With
        End Try

        Return err

    End Function

    Private Function CallWS026R(ByVal sRecordTotal As String, ByVal sLastRecord As String) As String

        'Create command object
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strProgram As String = "WS026R"
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                    "/" & strProgram & "(@PARAM1)"
        Dim parmIO As iDB2Parameter
        Dim PARAMOUT As String = String.Empty

        'Set the connection string
        cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)

        'Populate the parameter
        parmIO = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 3072)
        parmIO.Value = WS026Parm(sRecordTotal, sLastRecord)
        parmIO.Direction = ParameterDirection.InputOutput

        'Execute
        TalentCommonLog("CallWS026R", _de.CustomerNumber, "Backend Request: strHEADER=" & strHEADER & "parmIO.Value=" & parmIO.Value)
        cmdSELECT.ExecuteNonQuery()
        PARAMOUT = cmdSELECT.Parameters(Param1).Value.ToString
        TalentCommonLog("CallWS026R", _de.CustomerNumber, "Backend Response: PARAMOUT=" & PARAMOUT)

        Return PARAMOUT

    End Function

    Private Function WS026Parm(ByVal sRecordTotal As String, ByVal sLastRecord As String) As String

        Dim myString As String

        'Construct the parameter
        myString = Utilities.FixStringLength("", 3048) & _
                 Utilities.PadLeadingZeros(ConvertToYN(_de.IncludeBoxOfficeLinks), 1) & _
                 Utilities.FixStringLength("", 1) & _
                 Utilities.PadLeadingZeros(_de.CustomerNumber, 12) & _
                 Utilities.FixStringLength(sRecordTotal, 3) & _
                 Utilities.FixStringLength(sLastRecord, 3) & _
                 Utilities.FixStringLength(_de.Source, 1) & _
                 Utilities.FixStringLength("", 3)

        Return myString

    End Function

    Private Function AccessDatabaseWS607R() As ErrorObj

        Dim err As New ErrorObj
        ResultDataSet = New DataSet
        Dim sLastRecord As String = "000"
        Dim sRecordTotal As String = "000"
        Dim bMoreRecords As Boolean = True
        Dim dRow As DataRow = Nothing
        Dim parmWS026R As String = String.Empty
        Dim parmWS009R As String = String.Empty
        Dim parmWS121R As String = String.Empty
        Dim parmWS009R2 As String = String.Empty
        Dim parmWS051R As String = String.Empty
        Dim parmWS052R As String = String.Empty
        Dim WS051RLastRecord As Integer = 0

        'Create the Status data table
        Dim DtStatusResults As New DataTable("StatusResults")
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        Try

            'Call WS607R
            CallWS607R(parmWS009R, parmWS026R, parmWS121R, parmWS009R2, parmWS051R, parmWS052R)

            'Set the response data on the first call to WS016R
            dRow = Nothing
            dRow = DtStatusResults.NewRow
            If parmWS009R.Substring(1023, 1) = "E" Or parmWS009R.Substring(1021, 2).Trim <> "" Then
                dRow("ErrorOccurred") = "E"
                dRow("ReturnCode") = parmWS009R.Substring(1021, 2)
                bMoreRecords = False
            Else
                dRow("ErrorOccurred") = ""
                dRow("ReturnCode") = ""
            End If
            DtStatusResults.Rows.Add(dRow)

            'No errors 
            If parmWS009R.Substring(1023, 1) <> "E" And parmWS009R.Substring(1021, 2).Trim = "" Then

                'Process the customer details
                err = AccessDatabaseWS009R(parmWS009R, parmWS009R2)
                If _de.CustomerNumber.Trim().Length = 0 Then _de.CustomerNumber = parmWS009R.Substring(1002, 12)
                If _de.PaymentReference.Trim().Length = 0 Then _de.PaymentReference = (parmWS009R.Substring(625, 15))

                'Process the friends and families
                If Not err.HasError Then err = AccessDatabaseWS026R(parmWS026R)

                'Process the additional customer details
                If Not err.HasError Then err = AdditionalCustomerDetails(parmWS121R)

                'Process the user attributes ans special attributes
                If RefreshUserAttributesOnLogin Then
                    If Not err.HasError Then err = CustomerAttributes(parmWS051R, True)
                    If Not err.HasError Then err = AccessDatabaseWS052R(parmWS052R)
                End If

            End If

        Catch ex As Exception
            ResultDataSet = Nothing
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBCC-WS607R"
                .HasError = True
            End With
        End Try

        Return err

    End Function

    Private Sub CallWS607R(ByRef parmWS009R As String, ByRef parmWS026R As String, ByRef parmWS121R As String, ByRef parmWS009R2 As String, ByRef parmWS051R As String, ByRef parmWS052R As String)

        'Create command object
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strProgram As String = "WS607R"
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                   "/" & strProgram & "(@PARAM1, @PARAM2, @PARAM3, @PARAM4, @PARAM5, @PARAM6)"
        'Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
        '                           "/" & strProgram & "(@PARAM1, @PARAM2, @PARAM3, @PARAM4)"
        Dim parmIOWS009R As iDB2Parameter
        Dim parmIOWS026R As iDB2Parameter
        Dim parmIOWS121R As iDB2Parameter
        Dim parmIOWS009R2 As iDB2Parameter
        Dim parmIOWS051R As iDB2Parameter
        Dim parmIOWS052R As iDB2Parameter

        'Set the connection string
        cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)

        'Populate the WS009R parameter
        parmIOWS009R = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 1024)
        parmIOWS009R.Value = WS009Parm()
        parmIOWS009R.Direction = ParameterDirection.InputOutput

        'Populate the WS026R parameter
        parmIOWS026R = cmdSELECT.Parameters.Add(Param2, iDB2DbType.iDB2Char, 3072)
        parmIOWS026R.Value = WS026Parm("000", "000")
        parmIOWS026R.Direction = ParameterDirection.InputOutput

        'Populate the WS121R parameter
        parmIOWS121R = cmdSELECT.Parameters.Add(Param3, iDB2DbType.iDB2Char, 1024)
        parmIOWS121R.Value = WS121Parm()
        parmIOWS121R.Direction = ParameterDirection.InputOutput

        'Populate the WS009Parm2 parameter
        parmIOWS009R2 = cmdSELECT.Parameters.Add(Param4, iDB2DbType.iDB2Char, 1024)
        parmIOWS009R2.Value = WS009Parm2()
        parmIOWS009R2.Direction = ParameterDirection.InputOutput

        'Populate the WS051R parameter
        parmIOWS051R = cmdSELECT.Parameters.Add(Param5, iDB2DbType.iDB2Char, 5120)
        If RefreshUserAttributesOnLogin Then
            parmIOWS051R.Value = WS051Parm("00000", "00000")
        Else
            parmIOWS051R.Value = " "
        End If
        parmIOWS051R.Direction = ParameterDirection.InputOutput

        'Populate the WS052R parameter
        parmIOWS052R = cmdSELECT.Parameters.Add(Param6, iDB2DbType.iDB2Char, 32765)
        If RefreshUserAttributesOnLogin Then
            parmIOWS052R.Value = WS052Parm()
        Else
            parmIOWS052R.Value = " "
        End If
        parmIOWS052R.Direction = ParameterDirection.InputOutput

        'Execute
        TalentCommonLog("CallWS607R", _de.CustomerNo, "Backend Request: strHEADER=" & strHEADER & "parmIOWS009R.Value=" & parmIOWS009R.Value & _
                        ";parmIOWS026R.Value=" & parmIOWS026R.Value & ";parmIOWS121R.Value=" & parmIOWS121R.Value & ";parmIOWS009R2.Value=" & _
                        parmIOWS009R2.Value & ";parmIOWS051R.Value=" & parmIOWS051R.Value & ";parmIOWS052R.Value=" & parmIOWS052R.Value)
        cmdSELECT.ExecuteNonQuery()
        parmWS009R = cmdSELECT.Parameters(Param1).Value.ToString
        parmWS026R = cmdSELECT.Parameters(Param2).Value.ToString
        parmWS121R = cmdSELECT.Parameters(Param3).Value.ToString
        parmWS009R2 = cmdSELECT.Parameters(Param4).Value.ToString
        parmWS051R = cmdSELECT.Parameters(Param5).Value.ToString
        parmWS052R = cmdSELECT.Parameters(Param6).Value.ToString
        TalentCommonLog("CallWS607R", _de.CustomerNo, "Backend Response: parmWS009R=" & parmWS009R & ";parmWS026R=" & parmWS026R & ";parmWS121R=" & _
                        parmWS121R & ";parmWS051R=" & parmWS051R & ";parmWS052R=" & parmWS052R)

    End Sub

    Private Function FormatUserIDs() As String
        Dim myString As New StringBuilder
        myString.Append(Utilities.FixStringLength(_de.User_ID_4, 20))
        myString.Append(Utilities.FixStringLength(_de.User_ID_5, 20))
        myString.Append(Utilities.FixStringLength(_de.User_ID_6, 20))
        myString.Append(Utilities.FixStringLength(_de.User_ID_7, 20))
        myString.Append(Utilities.FixStringLength(_de.User_ID_8, 20))
        myString.Append(Utilities.FixStringLength(_de.User_ID_9, 20)) '120

        myString.Append(Utilities.FixStringLength(String.Empty, 333)) '453
        myString.Append(Utilities.FixStringLength(_de.ID, 20)) ' 473
        myString.Append(Utilities.FixStringLength(_de.IDType, 1)) '474


        Return myString.ToString()
    End Function

    Private Function AccessDatabaseWS616R() As ErrorObj
        Dim err As New ErrorObj
        ResultDataSet = New DataSet
        Dim parmWS616R As String = String.Empty
        Dim memberArray As String = String.Empty
        Dim totalCustomerCount As Integer = 0
        Dim currentCustomerIndex As Integer = 0
        Dim dRow As DataRow = Nothing
        Dim moreCustomer As Boolean = True
        Dim customerNumberLength As Integer = 12
        totalCustomerCount = DeV11.DECustomersV1.Count

        'Create the Status data table
        Dim dtStatusResults As New DataTable
        ResultDataSet.Tables.Add(dtStatusResults)
        With dtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With
        Dim dtParticipants As New DataTable
        With dtParticipants.Columns
            .Add("CustomerNumber", GetType(String))
        End With
        ResultDataSet.Tables.Add(dtParticipants)
        Try
            Do While moreCustomer
                CallWS616R(parmWS616R, totalCustomerCount, currentCustomerIndex, memberArray)
                If currentCustomerIndex < totalCustomerCount Then
                    moreCustomer = True
                Else
                    moreCustomer = False
                End If
                dRow = Nothing
                dRow = dtStatusResults.NewRow
                If parmWS616R.Substring(9999, 1) = "E" AndAlso parmWS616R.Substring(9997, 2).Trim <> "" Then
                    dRow("ErrorOccurred") = "E"
                    dRow("ReturnCode") = parmWS616R.Substring(9997, 2)
                    dtStatusResults.Rows.Add(dRow)
                    moreCustomer = False
                Else
                    If (currentCustomerIndex = totalCustomerCount) Then
                        dRow("ErrorOccurred") = ""
                        dRow("ReturnCode") = ""
                        dtStatusResults.Rows.Add(dRow)
                    End If
                    memberArray = parmWS616R.Substring(9000, 600)
                End If
            Loop
            memberArray = memberArray.Trim()
            Do While memberArray.Length > 0
                dRow = Nothing
                dRow = dtParticipants.NewRow
                dRow("CustomerNumber") = memberArray.Substring(0, customerNumberLength)
                dtParticipants.Rows.Add(dRow)
                If memberArray.Length > 12 Then
                    memberArray = memberArray.Substring(customerNumberLength, (memberArray.Length - customerNumberLength))
                Else
                    memberArray = ""
                End If
            Loop
        Catch ex As Exception
            ResultDataSet = Nothing
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBCC-WS616R"
                .HasError = True
            End With
        End Try
        Return err
    End Function

    Private Sub CallWS616R(ByRef parmWS616R As String, ByRef totalCustomerCount As Integer, ByRef currentCustomerIndex As Integer, ByVal memberArray As String)
        'Create command object
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strProgram As String = "WS616R"
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                    "/" & strProgram & "(@PARAM1,@PARAM2)"
        Dim paramInput1, paramInput2 As iDB2Parameter
        Dim paramValue1 As String = String.Empty
        Dim paramValue2 As String = String.Empty

        'build paramValue1 and paramValue2 by passing it as reference
        WS616Parm(totalCustomerCount, currentCustomerIndex, memberArray, paramValue1, paramValue2)

        'Set the connection string
        cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)

        'Populate the WS616R parameter
        paramInput1 = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 10000)
        paramInput1.Value = paramValue1
        paramInput1.Direction = ParameterDirection.InputOutput

        paramInput2 = cmdSELECT.Parameters.Add(Param2, iDB2DbType.iDB2Char, 10000)
        paramInput2.Value = paramValue2
        paramInput2.Direction = ParameterDirection.InputOutput

        'Execute
        TalentCommonLog("CallWS616R", _deAddTicketingItems.CustomerNumber, "Backend Request: strHEADER=" & strHEADER & "parmIOWS616R.Value=" & paramInput1.Value)
        cmdSELECT.ExecuteNonQuery()
        parmWS616R = cmdSELECT.Parameters(Param1).Value.ToString

        TalentCommonLog("CallWS616R", _deAddTicketingItems.CustomerNumber, "Backend Response: parmWS616R=" & parmWS616R)

    End Sub

    Private Sub WS616Parm(ByRef totalCustomerCount As Integer, ByRef currentCustomerIndex As Integer, ByVal memberArray As String, ByRef paramValue1 As String, ByRef paramValue2 As String)
        Dim deCust As DECustomer = Nothing
        Dim sbParameter As New StringBuilder
        Dim sbParamCustomers As New StringBuilder
        Dim sbParameter2 As New StringBuilder
        Dim customerLoopCount As Integer = 0
        'Construct the parameter
        For customerLoopCount = currentCustomerIndex To (currentCustomerIndex + 9)
            If customerLoopCount < totalCustomerCount Then
                deCust = DeV11.DECustomersV1(customerLoopCount)
                With sbParamCustomers
                    .Append(Utilities.FixStringLength(deCust.ContactForename, 20))
                    .Append(Utilities.FixStringLength(deCust.ContactSurname, 30))
                    If CLng(deCust.CustomerNumber) > 0 Then
                        .Append(Utilities.PadLeadingZeros(deCust.CustomerNumber, 12))
                    Else
                        .Append(Utilities.FixStringLength("", 12))
                    End If
                    .Append(Utilities.FixStringLength(deCust.DateBirth, 8))
                    .Append(Utilities.FixStringLength(deCust.Gender, 1))
                    .Append(Utilities.FixStringLength(deCust.EmailAddress, 60))
                    .Append(Utilities.FixStringLength(deCust.EmergencyContactName, 30))
                    .Append(Utilities.FixStringLength(deCust.EmergencyContactNumber, 30))
                    .Append(Utilities.FixStringLength(deCust.FanFlag, 1))
                    .Append(Utilities.FixStringLength("", 256))
                    .Append(Utilities.FixStringLength(deCust.AddressLine1, 30))
                    .Append(Utilities.FixStringLength(deCust.AddressLine2, 30))
                    .Append(Utilities.FixStringLength(deCust.AddressLine3, 25))
                    .Append(Utilities.FixStringLength(deCust.AddressLine4, 25))
                    .Append(Utilities.FixStringLength(deCust.AddressLine5, 20))
                    .Append(Utilities.FixStringLength(deCust.PostCode, 8))
                    .Append(Utilities.FixStringLength("", 314))
                End With
                'second paramValue Medical information
                With sbParameter2
                    .Append(Utilities.FixStringLength(deCust.MedicalInformation, 750))
                End With
            Else
                Exit For
            End If
        Next
        sbParameter.Append(Utilities.FixStringLength(sbParamCustomers.ToString(), 9000))
        sbParamCustomers = Nothing
        With sbParameter
            .Append(Utilities.FixStringLength(memberArray, 600))
            .Append(Utilities.FixStringLength(_deAddTicketingItems.SessionId, 36))
            .Append(Utilities.FixStringLength(_deAddTicketingItems.ProductCode, 6))
            .Append(Utilities.FixStringLength(_deAddTicketingItems.StandCode, 3))
            .Append(Utilities.FixStringLength(_deAddTicketingItems.AreaCode, 4))
            .Append(Utilities.FixStringLength(_deAddTicketingItems.SignedInCustomer, 12))
            .Append(Utilities.FixStringLength("", 332))
            .Append(Utilities.FixStringLength(_deAddTicketingItems.ProductType, 1))
            .Append(Utilities.FixStringLength(_deAddTicketingItems.UpdateMode, 1))
            .Append(Utilities.FixStringLength(_deAddTicketingItems.Source, 1))
            If customerLoopCount < totalCustomerCount Then
                .Append(Utilities.FixStringLength("Y", 1))
            Else
                .Append(Utilities.FixStringLength("N", 1))
            End If
            .Append(Utilities.FixStringLength("", 2))
            .Append(Utilities.FixStringLength("", 1))
        End With
        currentCustomerIndex = customerLoopCount

        paramValue1 = sbParameter.ToString

        'for 10 participants
        paramValue2 = Utilities.FixStringLength(sbParameter2.ToString(), 7500)
        'remaining space
        paramValue2 = Utilities.FixStringLength(paramValue2, 10000)

    End Sub

    Private Function AccessDatabaseWS003R() As ErrorObj
        Dim err As New ErrorObj
        ResultDataSet = New DataSet
        Dim PARAMOUT As String = String.Empty
        Dim dRow As DataRow = Nothing
        'Create the Status data table
        Dim dtStatusResults As New DataTable
        ResultDataSet.Tables.Add(dtStatusResults)
        With dtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With
        Try
            PARAMOUT = CallWS003RSingleMode()
            'Set the response data
            dRow = Nothing
            dRow = dtStatusResults.NewRow
            If PARAMOUT.Substring(1023, 1) = "E" Or PARAMOUT.Substring(1021, 2).Trim <> "" Then
                dRow("ErrorOccurred") = "E"
                dRow("ReturnCode") = PARAMOUT.Substring(1021, 2)
            Else
                dRow("ErrorOccurred") = ""
                dRow("ReturnCode") = ""
            End If
            dtStatusResults.Rows.Add(dRow)
        Catch ex As Exception
            ResultDataSet = Nothing
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBCC-WS003R"
                .HasError = True
            End With
        End Try
        Return err
    End Function

    Private Function CallWS003RSingleMode() As String
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strProgram As String = "WS003R"
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                    "/" & strProgram & "(@PARAM1,@PARAM2,@PARAM3)"
        Dim paramInput1, paramInput2, paramInput3 As iDB2Parameter
        Dim paramValue1 As String = String.Empty
        Dim paramValue2 As String = String.Empty
        Dim paramValue3 As String = String.Empty
        Dim paramOut As String = String.Empty

        'Set the connection string
        cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)

        'Populate the WS616R parameter
        paramInput1 = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 1024)
        paramInput1.Value = WS003ParamSingleMode()
        paramInput1.Direction = ParameterDirection.InputOutput

        paramInput2 = cmdSELECT.Parameters.Add(Param2, iDB2DbType.iDB2Char, 1024)
        paramInput2.Value = Utilities.FixStringLength("", 1024)
        paramInput2.Direction = ParameterDirection.InputOutput

        paramInput3 = cmdSELECT.Parameters.Add(Param3, iDB2DbType.iDB2Char, 6000)
        paramInput3.Value = Utilities.FixStringLength("", 6000)
        paramInput3.Direction = ParameterDirection.InputOutput

        'Execute
        TalentCommonLog("CallWS003RSingleMode", _de.CustomerNumber, "Backend Request: strHEADER=" & strHEADER & " WS003RValuesSingleMode=" & paramInput1.Value)
        cmdSELECT.ExecuteNonQuery()
        paramOut = cmdSELECT.Parameters(Param1).Value.ToString
        TalentCommonLog("CallWS003RSingleMode", _de.CustomerNumber, "Backend Response: paramWS003R=" & paramInput1.Value)
        Return paramOut
    End Function

    Private Function WS003ParamSingleMode() As String
        Dim paramString As String = String.Empty

        Select Case _de.SingleFieldMode

            Case SingleModeFieldsEnum.PHOTO
                paramString = paramString & Utilities.FixStringLength("", 511)
                paramString = paramString & Utilities.FixStringLength("PHOTO", 6)
                paramString = paramString & Utilities.FixStringLength("", 485)
                paramString = paramString & Utilities.PadLeadingZeros(_de.CustomerNumber, 12)
                paramString = paramString & Utilities.FixStringLength("", 10)

            Case SingleModeFieldsEnum.FSEAT
                paramString = paramString & Utilities.FixStringLength("", 473)
                paramString = paramString & Utilities.FixStringLength(_de.FavouriteStand, 3)
                paramString = paramString & Utilities.FixStringLength(_de.FavouriteArea, 4)
                paramString = paramString & Utilities.FixStringLength("", 31)
                paramString = paramString & Utilities.FixStringLength("FSEAT", 5) '516 (.net) 517 (RPG)
                paramString = paramString & Utilities.FixStringLength("", 461)
                paramString = paramString & Utilities.FixStringLength(_de.FavouriteRow, 4)
                paramString = paramString & Utilities.FixStringLength(_de.FavouriteSeat, 5)
                paramString = paramString & Utilities.FixStringLength("", 16)
                paramString = paramString & Utilities.PadLeadingZeros(_de.CustomerNumber, 12)
                paramString = paramString & Utilities.FixStringLength("", 10)

            Case SingleModeFieldsEnum.HOLD20
                paramString = paramString & Utilities.FixStringLength("", 511)
                paramString = paramString & Utilities.FixStringLength("HOLD20", 6)
                paramString = paramString & Utilities.FixStringLength("", 482)
                paramString = paramString & Utilities.PadLeadingZeros(_de.StopCode, 2)
                paramString = paramString & Utilities.FixStringLength("", 23)

            Case Else

        End Select
        Return paramString
    End Function

    Private Function AccessDatabaseTALENTCRM_VerifyAndRetrieveCustomerDetails()
        Dim err As New ErrorObj
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strProgram As String = "WS042R"
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & "/" & strProgram & "(@PARAM1,@PARAM2)"
        Dim inputParameter, outputParameter As iDB2Parameter
        Dim PARAMOUT1, PARAMOUT2 As String
        ResultDataSet = New DataSet

        If Not err.HasError Then
            Try
                cmdSELECT = New iDB2Command(strHEADER, conTALENTCRM)
                inputParameter = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 1024)
                inputParameter.Value = CrmRetreieveCustomerDetailsParameter()
                inputParameter.Direction = ParameterDirection.InputOutput
                outputParameter = cmdSELECT.Parameters.Add(Param2, iDB2DbType.iDB2Char, 1024)
                outputParameter.Value = Utilities.FixStringLength("", 1024)
                outputParameter.Direction = ParameterDirection.InputOutput

                TalentCommonLog("AccessDataBaseTALENTCRM", _de.CustomerNumber, "Backend Request: strHEADER=" & strHEADER & ", parm1=" & inputParameter.Value)
                cmdSELECT.ExecuteNonQuery()
                PARAMOUT1 = cmdSELECT.Parameters(Param1).Value.ToString
                PARAMOUT2 = cmdSELECT.Parameters(Param2).Value.ToString
                TalentCommonLog("AccessDataBaseTALENTCRM", _de.CustomerNumber, "Backend Response: PARMOUT1=" & PARAMOUT1 & ", PARMOUT2=" & PARAMOUT2)

                Dim DtStatusResults As New DataTable
                ResultDataSet.Tables.Add(DtStatusResults)
                With DtStatusResults.Columns
                    .Add("ErrorOccurred", GetType(String))
                    .Add("ReturnCode", GetType(String))
                End With

                Dim dRow As DataRow
                dRow = DtStatusResults.NewRow
                If (PARAMOUT1.Substring(1023, 1) = "E" Or PARAMOUT1.Substring(1021, 2).Trim <> "") Then
                    dRow("ErrorOccurred") = "E"
                    dRow("ReturnCode") = PARAMOUT1.Substring(1021, 2)
                    _de.ErrorFlag = "E"
                    _de.ErrorCode = PARAMOUT1.Substring(1021, 2)
                    AssignAddressOnlyFoundFlag(PARAMOUT2)
                Else
                    dRow("ErrorOccurred") = ""
                    dRow("ReturnCode") = ""
                End If
                DtStatusResults.Rows.Add(dRow)

                If PARAMOUT1.Substring(1023, 1) <> "E" OrElse _foundAddressOnlyFromCRM Then
                    Dim DtCRMCustomerResults As New DataTable
                    ResultDataSet.Tables.Add(DtCRMCustomerResults)
                    buildTALENTCRMCustomerTable(DtCRMCustomerResults)
                    dRow = Nothing
                    dRow = DtCRMCustomerResults.NewRow
                    buildTALENTCRMCustomerRecord(_de, PARAMOUT2, dRow)
                    DtCRMCustomerResults.Rows.Add(dRow)
                End If
            Catch ex As Exception
                Const strError As String = "Error during database access"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TACDBCC-06"
                    .HasError = True
                End With
            End Try
        End If
        Return err
    End Function

    Private Function CrmRetreieveCustomerDetailsParameter()
        Dim parameter As String = String.Empty

        parameter = Utilities.FixStringLength(_de.CompanySLNumber1, 15) & _
                    Utilities.FixStringLength(_de.CompanySLNumber2, 15) & _
                    Utilities.FixStringLength(_de.ContactForename, 30) & _
                    Utilities.FixStringLength(_de.ContactSurname, 30) & _
                    Utilities.FixStringLength(_de.PostCode, 10)
        If _de.RetreiveOnlyMode Then
            parameter += Utilities.FixStringLength("Y", 1)
        Else
            parameter += Utilities.FixStringLength("N", 1)
        End If
        parameter += Utilities.FixStringLength("", 1020) & _
                    Utilities.FixStringLength(_de.Source, 1)

        Return parameter
    End Function

    Public Sub buildTALENTCRMCustomerTable(ByRef dtCRMCustomerRetrieval As Data.DataTable)
        With dtCRMCustomerRetrieval.Columns
            .Add("EmailAddress", GetType(String))
            .Add("ContactTitle", GetType(String))
            .Add("ContactInitials", GetType(String))
            .Add("ContactForename", GetType(String))
            .Add("ContactSurname", GetType(String))
            .Add("Salutation", GetType(String))
            .Add("CompanyName", GetType(String))
            .Add("PositionInCompany", GetType(String))
            .Add("AddressLine1", GetType(String))
            .Add("AddressLine2", GetType(String))
            .Add("AddressLine3", GetType(String))
            .Add("AddressLine4", GetType(String))
            .Add("AddressLine5", GetType(String))
            .Add("PostCode", GetType(String))
            .Add("HomeTelephoneNumber", GetType(String))
            .Add("WorkTelephoneNumber", GetType(String))
            .Add("MobileNumber", GetType(String))
            .Add("FaxNumber", GetType(String))
            .Add("OtherNumber", GetType(String))
            .Add("DateBirth", GetType(String))
            .Add("ContactViaMail1", GetType(Boolean))
            .Add("ContactViaMail2", GetType(Boolean))
            .Add("ContactViaMail3", GetType(Boolean))
            .Add("ContactViaMail4", GetType(Boolean))
            .Add("ContactViaMail5", GetType(Boolean))
            .Add("CRMBranch", GetType(String))
            .Add("FoundAddressOnly", GetType(Boolean))
            .Add("RestrictedPaymentTypes", GetType(String))
        End With
    End Sub

    Public Sub buildTALENTCRMCustomerRecord(ByVal parmDE As DECustomer, ByVal PARAMOUT As String, ByRef dRow As DataRow)
        dRow("EmailAddress") = PARAMOUT.Substring(0, 60).Trim
        dRow("ContactTitle") = PARAMOUT.Substring(60, 4).Trim
        dRow("ContactForename") = PARAMOUT.Substring(64, 15).Trim
        dRow("ContactSurname") = PARAMOUT.Substring(79, 30).Trim
        dRow("AddressLine1") = PARAMOUT.Substring(109, 40).Trim
        dRow("AddressLine2") = PARAMOUT.Substring(149, 40).Trim
        dRow("AddressLine3") = PARAMOUT.Substring(189, 40).Trim
        dRow("AddressLine4") = PARAMOUT.Substring(229, 40).Trim
        dRow("AddressLine5") = PARAMOUT.Substring(269, 20).Trim
        dRow("PostCode") = PARAMOUT.Substring(289, 10).Trim
        dRow("WorkTelephoneNumber") = PARAMOUT.Substring(299, 30).Trim
        dRow("HomeTelephoneNumber") = PARAMOUT.Substring(329, 30).Trim
        dRow("MobileNumber") = PARAMOUT.Substring(359, 30).Trim
        dRow("FaxNumber") = PARAMOUT.Substring(389, 30).Trim
        dRow("OtherNumber") = PARAMOUT.Substring(419, 30).Trim
        dRow("ContactViaMail1") = convertToBool(PARAMOUT.Substring(449, 1).Trim.ToUpper)
        dRow("ContactViaMail2") = convertToBool(PARAMOUT.Substring(450, 1).Trim.ToUpper)
        dRow("ContactViaMail3") = convertToBool(PARAMOUT.Substring(451, 1).Trim.ToUpper)
        dRow("ContactViaMail4") = convertToBool(PARAMOUT.Substring(452, 1).Trim.ToUpper)
        dRow("ContactViaMail5") = convertToBool(PARAMOUT.Substring(453, 1).Trim.ToUpper)
        dRow("CompanyName") = PARAMOUT.Substring(454, 40).Trim
        dRow("CRMBranch") = PARAMOUT.Substring(494, 3).Trim
        dRow("DateBirth") = PARAMOUT.Substring(497, 6).Trim
        dRow("Salutation") = PARAMOUT.Substring(503, 20).Trim
        dRow("ContactInitials") = PARAMOUT.Substring(523, 2).Trim
        dRow("PositionInCompany") = PARAMOUT.Substring(556, 2).Trim
        dRow("FoundAddressOnly") = _foundAddressOnlyFromCRM
        dRow("RestrictedPaymentTypes") = PARAMOUT.Substring(586, 3).Trim
    End Sub

    Private Sub AssignAddressOnlyFoundFlag(ByVal PARAMOUT As String)
        'postcode orelse addressline1 orelse addressline2 orelse addressline3 if anyone found then assign flag=true
        If PARAMOUT.Substring(289, 10).Trim.Length > 0 _
            OrElse PARAMOUT.Substring(109, 40).Trim.Length > 0 _
            OrElse PARAMOUT.Substring(149, 40).Trim.Length > 0 _
            OrElse PARAMOUT.Substring(189, 40).Trim.Length > 0 Then
            _foundAddressOnlyFromCRM = True
        End If
    End Sub

    Private Function WS052Parm() As String
        Dim myString As New StringBuilder

        'Construct the parameter
        myString.Append(Utilities.PadLeadingZeros(_de.CustomerNo, 12))                          ' 1   - 12
        myString.Append(Utilities.FixStringLength(ConvertToYN(_de.BirthdayAlertEnabled), 1))        ' 13  - 13
        myString.Append(Utilities.FixStringLength(ConvertToYN(_de.FAndFBirthdayAlertEnabled), 1))   ' 14  - 14
        myString.Append(Utilities.FixStringLength(ConvertToYN(_de.CardExpiryPPSAlertEnabled), 1))   ' 15  - 15
        myString.Append(Utilities.FixStringLength(ConvertToYN(_de.CardExpirySAVAlertEnabled), 1))   ' 16  - 16
        myString.Append(Utilities.FixStringLength("", 96))                                          ' 17  - 112
        myString.Append(Utilities.FixStringLength(_de.Source, 1))                                   ' 113 - 113
        myString.Append(Utilities.FixStringLength("", 3))                                           ' 114 - 116
        myString.Append(Utilities.PadLeadingZeros(_de.CardExpiryPPSWarnPeriodDays, 3))              ' 117 - 119
        myString.Append(Utilities.PadLeadingZeros(_de.CardExpirySAVWarnPeriodDays, 3))              ' 120 - 122

        Return myString.ToString
    End Function

    Private Function AccessDatabaseWS059R() As ErrorObj
        Dim err As New ErrorObj
        ResultDataSet = New DataSet
        Dim lastId As String = ""
        Dim bMoreRecords As Boolean = True
        Dim dRow As DataRow = Nothing
        Dim PARAMOUT As String = String.Empty
        Dim PARAMOUT2 As String = String.Empty

        Dim DtStatusResults As New DataTable
        DtStatusResults.TableName = "ResultStatus"
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        Dim DtAttributeDefinition As New DataTable
        DtAttributeDefinition.TableName = "AttributeDefinition"
        ResultDataSet.Tables.Add(DtAttributeDefinition)
        With DtAttributeDefinition.Columns
            .Add("NAME", GetType(String))
            .Add("DESCRIPTION", GetType(String))
            .Add("CATEGORY", GetType(String))
            .Add("TYPE", GetType(String)) 'Eg.(customer, contact, product)
            .Add("FOREIGN_KEY", GetType(String))
        End With

        Try
            Do While bMoreRecords
                PARAMOUT2 = String.Empty
                PARAMOUT = CallWS059R(lastId, PARAMOUT2)
                Dim hasError As Boolean = True
                dRow = Nothing
                dRow = DtStatusResults.NewRow
                If PARAMOUT.Substring(509, 1) = "E" Or PARAMOUT.Substring(510, 2).Trim <> "" Then
                    dRow("ErrorOccurred") = "E"
                    dRow("ReturnCode") = PARAMOUT.Substring(510, 2)
                    bMoreRecords = False
                    hasError = True
                Else
                    dRow("ErrorOccurred") = ""
                    dRow("ReturnCode") = ""
                    bMoreRecords = convertToBool(PARAMOUT.Substring(494, 1))
                    lastId = PARAMOUT.Substring(495, 13)
                    hasError = False
                End If
                DtStatusResults.Rows.Add(dRow)

                If Not hasError Then
                    Dim iPosition As Integer = 0
                    Dim iCounter As Integer = 1
                    Do While iCounter <= 200
                        If PARAMOUT2.Substring(iPosition, 13).Trim = "" Then
                            Exit Do
                        Else
                            dRow = Nothing
                            dRow = DtAttributeDefinition.NewRow
                            'lastId = PARAMOUT2.Substring(iPosition, 13)
                            dRow("FOREIGN_KEY") = (PARAMOUT2.Substring(iPosition, 13)).Trim()
                            dRow("NAME") = (PARAMOUT2.Substring(iPosition + 13, 12)).Trim()
                            dRow("DESCRIPTION") = (PARAMOUT2.Substring(iPosition + 25, 30)).Trim()
                            dRow("CATEGORY") = (PARAMOUT2.Substring(iPosition + 55, 3)).Trim()
                            dRow("TYPE") = (PARAMOUT2.Substring(iPosition + 58, 1)).Trim()
                            DtAttributeDefinition.Rows.Add(dRow)
                            iPosition = iPosition + 100
                            iCounter = iCounter + 1
                        End If
                    Loop
                End If
            Loop
        Catch ex As Exception
            ResultDataSet = Nothing
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBCC-WS059R"
                .HasError = True
            End With
        End Try
        Return err
    End Function

    Private Function CallWS059R(ByVal lastId As String, ByRef PARAMOUT2 As String) As String
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strProgram As String = "WS059R"
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & "/" & strProgram & "(@PARAM1, @PARAM2)"
        Dim parmIO1, parmIO2 As iDB2Parameter
        Dim PARAMOUT As String = String.Empty

        cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)
        parmIO1 = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 512)
        parmIO1.Value = WS059RParm(lastId)
        parmIO1.Direction = ParameterDirection.InputOutput
        parmIO2 = cmdSELECT.Parameters.Add(Param2, iDB2DbType.iDB2Char, 20000)
        parmIO2.Value = Utilities.FixStringLength("", 20000)
        parmIO2.Direction = ParameterDirection.InputOutput

        TalentCommonLog("CallWS059R", _de.CustomerNumber, "Backend Request: strHEADER=" & strHEADER & ", parmIO1.Value=" & parmIO1.Value & ", parmIO2.Value=''")
        cmdSELECT.ExecuteNonQuery()
        PARAMOUT = cmdSELECT.Parameters(Param1).Value.ToString
        PARAMOUT2 = cmdSELECT.Parameters(Param2).Value.ToString
        TalentCommonLog("CallWS059R", _de.CustomerNumber, "Backend Response: (parm2) PARAMOUT2=" & PARAMOUT)

        Return PARAMOUT
    End Function

    Private Function WS059RParm(ByVal lastId As String) As String
        Dim myString As New StringBuilder

        If _de.Category.Length > 0 Then
            myString.Append("AC")
            myString.Append(Utilities.FixStringLength(_de.Category, 3))
        Else
            myString.Append("AT")
            myString.Append(Utilities.FixStringLength("", 3))
        End If
        myString.Append("2") 'Attribute Type
        myString.Append(Utilities.FixStringLength("", 489))
        myString.Append(Utilities.FixStringLength(lastId, 13))
        myString.Append(Utilities.FixStringLength(_de.Source, 1))
        myString.Append(Utilities.FixStringLength("", 3))

        Return myString.ToString
    End Function

    Private Function AccessDatabaseWS052R(ByVal PARAMOUT As String) As ErrorObj
        Dim err As New ErrorObj
        Dim dRow As DataRow = Nothing
        Dim tDataObjects As New Talent.Common.TalentDataObjects
        Dim DtSpecialAttributes As New DataTable("SpecialAttributes")
        ResultDataSet.Tables.Add(DtSpecialAttributes)
        With DtSpecialAttributes.Columns
            .Add("Attribute", GetType(String))
            .Add("AttributeId", GetType(String))
            .Add("AttributeName", GetType(String))
            .Add("AttributeData", GetType(String))
            .Add("LoginId", GetType(String))
        End With
        Try
            Const birthdayAlert As String = "BirthdayAlert"
            Const ffBirthdayAlert As String = "FFBirthdayAlert"
            Const ccExpiryAlertPPS As String = "CCExpiryAlertPPS"
            Const ccExpiryAlertSAV As String = "CCExpiryAlertSAV"
            Const CustomerReservationsAlert As String = "ReservationAlert"
            Dim specialAttribute() As String = PARAMOUT.Trim.Split("|".ToCharArray, StringSplitOptions.RemoveEmptyEntries)
            For Each attribute As String In specialAttribute
                tDataObjects.Settings = _settings

                Select Case True
                    Case attribute.StartsWith(birthdayAlert)
                        dRow = Nothing
                        dRow = DtSpecialAttributes.NewRow
                        dRow("AttributeId") = tDataObjects.AlertSettings.TblAttributeDefinition.GetSpecialAlertAttributeForeignKey(_settings.BusinessUnit, _settings.Partner, birthdayAlert)
                        dRow("AttributeName") = birthdayAlert
                        dRow("AttributeData") = attribute.Replace(birthdayAlert, String.Empty)
                        dRow("LoginId") = Utilities.PadLeadingZeros(_de.CustomerNumber, 12)
                        DtSpecialAttributes.Rows.Add(dRow)
                    Case attribute.StartsWith(ffBirthdayAlert)
                        dRow = Nothing
                        dRow = DtSpecialAttributes.NewRow
                        dRow("AttributeId") = tDataObjects.AlertSettings.TblAttributeDefinition.GetSpecialAlertAttributeForeignKey(_settings.BusinessUnit, _settings.Partner, ffBirthdayAlert)
                        dRow("AttributeName") = ffBirthdayAlert
                        dRow("AttributeData") = attribute.Replace(ffBirthdayAlert, String.Empty)
                        dRow("LoginId") = Utilities.PadLeadingZeros(_de.CustomerNumber, 12)
                        DtSpecialAttributes.Rows.Add(dRow)
                    Case attribute.StartsWith(ccExpiryAlertPPS)
                        dRow = Nothing
                        dRow = DtSpecialAttributes.NewRow
                        dRow("AttributeId") = tDataObjects.AlertSettings.TblAttributeDefinition.GetSpecialAlertAttributeForeignKey(_settings.BusinessUnit, _settings.Partner, ccExpiryAlertPPS)
                        dRow("AttributeName") = ccExpiryAlertPPS
                        dRow("AttributeData") = attribute.Replace(ccExpiryAlertPPS, String.Empty)
                        dRow("LoginId") = Utilities.PadLeadingZeros(_de.CustomerNumber, 12)
                        DtSpecialAttributes.Rows.Add(dRow)
                    Case attribute.StartsWith(ccExpiryAlertSAV)
                        dRow = Nothing
                        dRow = DtSpecialAttributes.NewRow
                        dRow("AttributeId") = tDataObjects.AlertSettings.TblAttributeDefinition.GetSpecialAlertAttributeForeignKey(_settings.BusinessUnit, _settings.Partner, ccExpiryAlertSAV)
                        dRow("AttributeName") = ccExpiryAlertSAV
                        dRow("AttributeData") = attribute.Replace(ccExpiryAlertSAV, String.Empty)
                        dRow("LoginId") = Utilities.PadLeadingZeros(_de.CustomerNumber, 12)
                        DtSpecialAttributes.Rows.Add(dRow)
                    Case attribute.StartsWith(CustomerReservationsAlert)
                        dRow = Nothing
                        dRow = DtSpecialAttributes.NewRow
                        dRow("AttributeId") = tDataObjects.AlertSettings.TblAttributeDefinition.GetSpecialAlertAttributeForeignKey(_settings.BusinessUnit, _settings.Partner, CustomerReservationsAlert)
                        dRow("AttributeName") = CustomerReservationsAlert
                        dRow("AttributeData") = attribute.Replace(CustomerReservationsAlert, String.Empty)
                        dRow("LoginId") = Utilities.PadLeadingZeros(_de.CustomerNumber, 12)
                        DtSpecialAttributes.Rows.Add(dRow)
                End Select

            Next
        Catch ex As Exception
            ResultDataSet = Nothing
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBCC-WS052R"
                .HasError = True
            End With
        End Try
        Return err
    End Function

    ''' <summary>
    ''' Build the call to WS133R
    ''' and populate the data entity and return an error object
    ''' </summary>
    ''' <returns>An error object</returns>
    ''' <remarks></remarks>

    Private Function AccessDatabaseWS133R() As ErrorObj
        Dim err As New ErrorObj
        ResultDataSet = New DataSet
        Dim sLastRecord As String = "0000"
        Dim PARAMOUT As String = String.Empty
        Dim bMoreRecords As Boolean = True
        Dim dRow As DataRow = Nothing

        'Create the Status data table
        Dim DtStatusResults As New DataTable
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        Dim DtReservedAndSoldSeats As New DataTable("ReservedAndSoldSeats")
        ResultDataSet.Tables.Add(DtReservedAndSoldSeats)
        With DtReservedAndSoldSeats.Columns
            .Add("ProductCode", GetType(String))
            .Add("Stand", GetType(String))
            .Add("Area", GetType(String))
            .Add("Row", GetType(String))
            .Add("Seat", GetType(String))
            .Add("Alpha", GetType(String))
            .Add("ReservationCode", GetType(String))
            .Add("CustomerNumber", GetType(String))
            .Add("Sold", GetType(String))
            .Add("ReservedDate", GetType(Date))
            .Add("ReservedTime", GetType(String))
        End With

        Try
            While bMoreRecords

                PARAMOUT = CallWS133R(sLastRecord)
                If sLastRecord = "0000" Then
                    'Set the status data table
                    dRow = Nothing
                    dRow = DtStatusResults.NewRow
                    If PARAMOUT.Substring(10239, 1) = "E" Or PARAMOUT.Substring(10238, 2).Trim <> "" Then
                        dRow("ErrorOccurred") = "E"
                        dRow("ReturnCode") = PARAMOUT.Substring(10238, 2)
                    Else
                        dRow("ErrorOccurred") = ""
                        dRow("ReturnCode") = ""
                    End If
                    DtStatusResults.Rows.Add(dRow)
                End If

                If Not (PARAMOUT.Substring(10239, 1) = "E" Or PARAMOUT.Substring(10238, 2).Trim <> "") Then
                    Dim iPosition As Integer = 0
                    Dim iCounter As Integer = 1
                    Do While iCounter <= 150
                        If PARAMOUT.Substring(iPosition, 65).Trim = "" Then
                            Exit Do
                        Else
                            dRow = Nothing
                            dRow = DtReservedAndSoldSeats.NewRow
                            dRow("ProductCode") = PARAMOUT.Substring(iPosition, 6).Trim
                            dRow("Stand") = PARAMOUT.Substring(iPosition + 6, 3).Trim
                            dRow("Area") = PARAMOUT.Substring(iPosition + 9, 4).Trim
                            dRow("Row") = PARAMOUT.Substring(iPosition + 13, 4).Trim
                            dRow("Seat") = PARAMOUT.Substring(iPosition + 17, 4).Trim
                            dRow("Alpha") = PARAMOUT.Substring(iPosition + 21, 1).Trim
                            dRow("ReservationCode") = PARAMOUT.Substring(iPosition + 22, 2).Trim
                            dRow("CustomerNumber") = PARAMOUT.Substring(iPosition + 24, 13).Trim
                            dRow("Sold") = PARAMOUT.Substring(iPosition + 37, 1).Trim
                            dRow("ReservedDate") = Utilities.ISeriesDate(PARAMOUT.Substring(iPosition + 38, 7).Trim)
                            dRow("ReservedTime") = Utilities.ISeriesTime(PARAMOUT.Substring(iPosition + 45, 7).Trim)
                            DtReservedAndSoldSeats.Rows.Add(dRow)
                            iPosition = iPosition + 65
                            iCounter = iCounter + 1
                        End If
                    Loop
                End If

                Dim moreRecordsToProcess As String = PARAMOUT.Substring(10230, 2)
                If moreRecordsToProcess <> "Y" Then
                    bMoreRecords = False
                Else
                    sLastRecord = PARAMOUT.Substring(10232, 4)
                End If
            End While
        Catch ex As Exception
            ResultDataSet = Nothing
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "WS133R"
                .HasError = True
            End With
        End Try

        Return err
    End Function

    ''' <summary>
    ''' Make the call to the WS API and return the outbound parameter as a string
    ''' </summary>
    ''' <returns>The outbound parameter as it is returned from the iSeries</returns>
    ''' <remarks></remarks>
    Private Function CallWS133R(ByVal sLastRecord As String) As String

        'Create command object
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strProgram As String = "WS133R"
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                    "/" & strProgram & "(@PARAM1)"
        Dim parmIO As iDB2Parameter
        Dim PARAMOUT As String = String.Empty

        'Set the connection string
        cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)

        'Populate the parameter
        parmIO = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 10240)
        parmIO.Value = WS133RParm(sLastRecord)
        parmIO.Direction = ParameterDirection.InputOutput

        'Execute
        Utilities.TalentCommonLog("CallWS133R", _deAddTicketingItems.CustomerNumber, "Backend Request: strHEADER=" & strHEADER & ", parmIO.Value=" & parmIO.Value)
        cmdSELECT.ExecuteNonQuery()
        PARAMOUT = cmdSELECT.Parameters(Param1).Value.ToString
        Utilities.TalentCommonLog("CallWS133R", _deAddTicketingItems.CustomerNumber, "Backend Response: PARAMOUT=" & PARAMOUT)

        Return PARAMOUT

    End Function

    ''' <summary>
    ''' Format WS133R inbound parameter value
    ''' </summary>
    ''' <returns>The formatted parameter as a string</returns>
    ''' <remarks></remarks>
    Private Function WS133RParm(ByVal sLastRecord As String) As String

        Dim myString As String = ""

        'Construct the parameter

        myString = Utilities.FixStringLength(String.Empty, 10212) & _
                   Utilities.FixStringLength(_deAddTicketingItems.ProductCode, 6) & _
                   Utilities.FixStringLength(_deAddTicketingItems.CustomerNumber, 12) & _
                   Utilities.FixStringLength(String.Empty, 2) & _
                   Utilities.FixStringLength(sLastRecord, 4) & _
                   Utilities.FixStringLength(_deAddTicketingItems.Source, 1) & _
                   Utilities.FixStringLength(String.Empty, 2) & _
                   Utilities.FixStringLength(String.Empty, 1)
        Return myString

    End Function

    ''' <summary>
    ''' Build the call to WS140R
    ''' and populate the data entity and return an error object
    ''' </summary>
    ''' <returns>An error object</returns>
    ''' <remarks></remarks>
    Private Function AccessDatabaseWS140R() As ErrorObj
        Dim err As New ErrorObj
        ResultDataSet = New DataSet
        Dim PARAMOUT As String = String.Empty
        Dim bMoreRecords As Boolean = True
        Dim dRow As DataRow = Nothing
        Dim lastRRN As String = ""
        Dim lastRRN2 As String = ""
        Dim CustomerNo As String = ""


        Dim DtStatusResults As New DataTable
        DtStatusResults.TableName = "ResultStatus"
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        Dim DtAddresses As New DataTable
        DtAddresses.TableName = "DtAddresses"
        ResultDataSet.Tables.Add(DtAddresses)
        With DtAddresses.Columns
            .Add("ADDRESS1", GetType(String))
            .Add("ADDRESS2", GetType(String))
            .Add("TOWN", GetType(String))
            .Add("COUNTY", GetType(String))
            .Add("COUNTRY", GetType(String))
            .Add("POSTCODE1", GetType(String))
            .Add("POSTCODE2", GetType(String))
            .Add("MONIKER", GetType(String))
        End With


        Try
            Do While bMoreRecords

                PARAMOUT = CallWS140R(lastRRN, lastRRN2)

                ' Address Loop.......
                Dim hasError As Boolean = True
                dRow = Nothing
                dRow = DtStatusResults.NewRow
                If PARAMOUT.Substring(32764, 1) = "E" Or PARAMOUT.Substring(32762, 2).Trim <> "" Then
                    dRow("ErrorOccurred") = "E"
                    dRow("ReturnCode") = PARAMOUT.Substring(32762, 2)
                    bMoreRecords = False
                    hasError = True
                Else
                    dRow("ErrorOccurred") = ""
                    dRow("ReturnCode") = ""
                    bMoreRecords = Utilities.convertToBool(PARAMOUT.Substring(32735, 1))
                    lastRRN = PARAMOUT.Substring(32736, 15)
                    lastRRN2 = PARAMOUT.Substring(32708, 15)
                    hasError = False
                End If
                DtStatusResults.Rows.Add(dRow)

                If Not hasError Then
                    Dim iPosition As Integer = 0
                    Dim iCounter As Integer = 1
                    Do While iCounter <= 100
                        If PARAMOUT.Substring(iPosition, 250).Trim = "" Then
                            Exit Do
                        Else
                            dRow = Nothing
                            dRow = DtAddresses.NewRow
                            dRow("ADDRESS1") = (PARAMOUT.Substring(iPosition, 30)).Trim()
                            dRow("ADDRESS2") = (PARAMOUT.Substring(iPosition + 30, 30)).Trim()
                            dRow("TOWN") = (PARAMOUT.Substring(iPosition + 60, 25)).Trim()
                            dRow("COUNTY") = (PARAMOUT.Substring(iPosition + 85, 25)).Trim()
                            dRow("COUNTRY") = (PARAMOUT.Substring(iPosition + 110, 20)).Trim()
                            dRow("POSTCODE1") = (PARAMOUT.Substring(iPosition + 130, 4)).Trim()
                            dRow("POSTCODE2") = (PARAMOUT.Substring(iPosition + 134, 4)).Trim()
                            dRow("MONIKER") = (PARAMOUT.Substring(iPosition + 138, 30)).Trim()
                            DtAddresses.Rows.Add(dRow)
                            iPosition = iPosition + 250
                            iCounter = iCounter + 1
                        End If
                    Loop
                End If

                ' Printer Loop
                dRow = Nothing
                dRow = DtStatusResults.NewRow
                If PARAMOUT.Substring(32764, 1) = "E" Or PARAMOUT.Substring(32762, 2).Trim <> "" Then
                    dRow("ErrorOccurred") = "E"
                    dRow("ReturnCode") = PARAMOUT.Substring(32762, 2)
                    bMoreRecords = False
                    hasError = True
                Else
                    dRow("ErrorOccurred") = ""
                    dRow("ReturnCode") = ""
                    bMoreRecords = Utilities.convertToBool(PARAMOUT.Substring(32735, 1))
                    lastRRN = PARAMOUT.Substring(32736, 15)
                    lastRRN2 = PARAMOUT.Substring(32708, 15)
                    hasError = False
                End If
                DtStatusResults.Rows.Add(dRow)


            Loop

        Catch ex As Exception
            ResultDataSet = Nothing
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "WS140R"
                .HasError = True
            End With
        End Try
        Return err
    End Function

    ''' <summary>
    ''' Make the call to the WS API and return the outbound parameter as a string
    ''' </summary>
    ''' <returns>The outbound parameter as it is returned from the iSeries</returns>
    ''' <remarks></remarks>
    Private Function CallWS140R(ByVal lastRRN As String, lastRRN2 As String) As String
        Dim cmdSELECT As iDB2Command = Nothing
        Dim parmIO As iDB2Parameter
        Dim PARAMOUT As String = String.Empty
        Dim strHEADER As New StringBuilder

        strHEADER.Append("CALL ")
        strHEADER.Append(Settings.StoredProcedureGroup.Trim)
        strHEADER.Append("/WS140R(@PARAM1)")

        cmdSELECT = New iDB2Command(strHEADER.ToString(), conTALENTTKT)
        parmIO = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 32765)
        parmIO.Value = WS140Parm(lastRRN, lastRRN2)
        parmIO.Direction = ParameterDirection.InputOutput
        cmdSELECT.ExecuteNonQuery()
        PARAMOUT = cmdSELECT.Parameters(Param1).Value.ToString
        Return PARAMOUT
    End Function

    ''' <summary>
    ''' Format WS140R inbound parameter value
    ''' </summary>
    ''' <returns>The formatted parameter as a string</returns>
    ''' <remarks></remarks>
    Private Function WS140Parm(ByVal lastRRN As String, lastRRN2 As String) As String
        Dim myString As New StringBuilder
        myString.Append(Utilities.FixStringLength(String.Empty, 32708))
        myString.Append(Utilities.PadLeadingZeros(lastRRN2, 15))
        myString.Append(Utilities.PadLeadingZeros(_de.CustomerNumber, 12))
        myString.Append(Utilities.FixStringLength("N", 1))
        myString.Append(Utilities.PadLeadingZeros(lastRRN, 15))
        myString.Append(Utilities.FixStringLength(AgentDataEntity.AgentUsername, 10))
        myString.Append(Utilities.FixStringLength(String.Empty, 4))
        Return myString.ToString()
    End Function

    Private Function WS165Parm() As String
        Dim myString As New StringBuilder
        myString.Append(Utilities.PadLeadingZeros(_de.CustomerNo, 12))
        myString.Append(_de.NewAddress)
        myString.Append(_de.CustomerList)
        myString.Append(Utilities.FixStringLength(String.Empty, 5)) ' Returned Customer count 
        myString.Append(Utilities.FixStringLength(String.Empty, 7841)) ' Spare
        myString.Append(Utilities.PadLeadingZeros(GlobalConstants.SOURCE, 1))
        myString.Append(Utilities.FixStringLength(String.Empty, 3))
        Return myString.ToString()
    End Function

    Private Function AccessDatabase_RetrieveCourseDetails() As ErrorObj
        Dim err As New ErrorObj
        Dim cmdSelect As iDB2Command = Nothing
        Dim dtr As iDB2DataReader = Nothing
        Try
            Dim sqlselect As New StringBuilder
            sqlselect.Append("SELECT FANF2U, EMNM2U, EMCN2U, MEDL2U FROM CD02U ")
            sqlselect.Append("WHERE CONO2U = @COMPANY AND MEMB2U = @CUSTOMER AND ACTR2U = 'A'")
            cmdSelect = New iDB2Command(sqlselect.ToString(), conTALENTTKT)
            cmdSelect.Parameters.Add("@COMPANY", iDB2DbType.iDB2VarChar, 3).Value = Settings.Company
            cmdSelect.Parameters.Add("@CUSTOMER", iDB2DbType.iDB2VarChar, 12).Value = _de.CustomerNumber

            dtr = cmdSelect.ExecuteReader
            While dtr.Read
                CourseDetailsFanFlag = Utilities.convertToBool(dtr.Item("FANF2U").ToString().Trim())
                CourseDetailsContactName = dtr.Item("EMNM2U").ToString().Trim()
                CourseDetailsContactNumber = dtr.Item("EMCN2U").ToString().Trim()
                CourseDetailsMedicalInfo = dtr.Item("MEDL2U").ToString().Trim()
            End While
        Catch ex As Exception
            Const strError As String = "Error Retrieving Course Details"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBPD-AccessDatabase_RetrieveCourseDetails"
                .HasError = True
            End With
        End Try
        Return err
    End Function

    Private Function AccessDatabase_UpdateCourseDetails() As ErrorObj
        Dim err As New ErrorObj
        Try
            Dim sqlselect As New StringBuilder
            sqlselect.Append("SELECT * FROM CD02U ")
            sqlselect.Append("WHERE CONO2U = @COMPANY AND MEMB2U = @CUSTOMER AND ACTR2U = 'A'")
            Dim cmdSelect As New iDB2Command(sqlselect.ToString(), conTALENTTKT)
            cmdSelect.Parameters.Add("@COMPANY", iDB2DbType.iDB2VarChar, 3).Value = Settings.Company
            cmdSelect.Parameters.Add("@CUSTOMER", iDB2DbType.iDB2VarChar, 12).Value = _de.CustomerNumber

            Dim dataReader As iDB2DataReader
            dataReader = cmdSelect.ExecuteReader
            If dataReader.HasRows Then
                Dim sqlUpdate As New StringBuilder
                sqlUpdate.Append("UPDATE CD02U SET FANF2U = @FAN_FLAG, EMNM2U = @CONTACT_NAME, EMCN2U = @CONTACT_NUMBER, MEDL2U = @MEDICAL_INFO, ")
                sqlUpdate.Append("USER2U = @AGENT, PGMD2U = 'WEBSQL',  UPDT2U = @DATE ")
                sqlUpdate.Append("WHERE CONO2U = @COMPANY AND MEMB2U = @CUSTOMER AND ACTR2U = 'A'")
                Dim cmdUpdate As New iDB2Command(sqlUpdate.ToString(), conTALENTTKT)
                cmdUpdate.Parameters.Add("@FAN_FLAG", iDB2DbType.iDB2VarChar, 1).Value = Utilities.ConvertToYN(CourseDetailsFanFlag)
                cmdUpdate.Parameters.Add("@CONTACT_NAME", iDB2DbType.iDB2VarChar, 30).Value = CourseDetailsContactName
                cmdUpdate.Parameters.Add("@CONTACT_NUMBER", iDB2DbType.iDB2VarChar, 30).Value = CourseDetailsContactNumber
                cmdUpdate.Parameters.Add("@MEDICAL_INFO", iDB2DbType.iDB2VarChar, 750).Value = CourseDetailsMedicalInfo
                cmdUpdate.Parameters.Add("@AGENT", iDB2DbType.iDB2VarChar, 10).Value = Settings.AgentEntity.AgentUsername
                cmdUpdate.Parameters.Add("@DATE", iDB2DbType.iDB2Date, 7).Value = Utilities.DateToIseriesFormat(Now)
                cmdUpdate.Parameters.Add("@COMPANY", iDB2DbType.iDB2VarChar, 3).Value = Settings.Company
                cmdUpdate.Parameters.Add("@CUSTOMER", iDB2DbType.iDB2VarChar, 12).Value = _de.CustomerNumber
                cmdUpdate.ExecuteNonQuery()
            Else
                Dim sqlInsert As New StringBuilder
                sqlInsert.Append("INSERT INTO CD02U ")
                sqlInsert.Append("VALUES(@AGENT, 'WEBSQL', 0, @DATE, 'A', @COMPANY, @CUSTOMER, @FAN_FLAG, @CONTACT_NAME, @CONTACT_NUMBER, @MEDICAL_INFO)")
                Dim cmdInsert As New iDB2Command(sqlInsert.ToString(), conTALENTTKT)
                cmdInsert.Parameters.Add("@AGENT", iDB2DbType.iDB2VarChar, 10).Value = Settings.AgentEntity.AgentUsername
                cmdInsert.Parameters.Add("@DATE", iDB2DbType.iDB2Date, 7).Value = Utilities.DateToIseriesFormat(Now)
                cmdInsert.Parameters.Add("@COMPANY", iDB2DbType.iDB2VarChar, 3).Value = Settings.Company
                cmdInsert.Parameters.Add("@CUSTOMER", iDB2DbType.iDB2VarChar, 12).Value = _de.CustomerNumber
                cmdInsert.Parameters.Add("@FAN_FLAG", iDB2DbType.iDB2VarChar, 1).Value = Utilities.ConvertToYN(CourseDetailsFanFlag)
                cmdInsert.Parameters.Add("@CONTACT_NAME", iDB2DbType.iDB2VarChar, 30).Value = CourseDetailsContactName
                cmdInsert.Parameters.Add("@CONTACT_NUMBER", iDB2DbType.iDB2VarChar, 30).Value = CourseDetailsContactNumber
                cmdInsert.Parameters.Add("@MEDICAL_INFO", iDB2DbType.iDB2VarChar, 750).Value = CourseDetailsMedicalInfo
                cmdInsert.ExecuteNonQuery()
            End If
        Catch ex As Exception
            Const strError As String = "Error Updating Course Details"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBPD-AccessDatabase_UpdateCourseDetails"
                .HasError = True
            End With
        End Try
        Return err
    End Function

    Private Function AccessDatabaseWS272R() As ErrorObj
        Dim err As New ErrorObj
        ResultDataSet = New DataSet
        Dim PARAMOUT As String = String.Empty
        Dim bMoreRecords As Boolean = True
        Dim dRow As DataRow = Nothing
        Dim totalRRN As String = "0"
        Dim lastRRN As String = "0"
        Dim lastSeatID As String = ""
        Dim CustomerNo As String = ""


        Dim DtStatusResults As New DataTable("StatusResults")
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        Dim dtCustomerMemberships As New DataTable("CustomerMemberships")

        ResultDataSet.Tables.Add(dtCustomerMemberships)
        With dtCustomerMemberships.Columns
            .Add("MembershipDesc", GetType(String))
            .Add("MembershipNumber", GetType(String))
            .Add("PurchaseDate", GetType(String))
            .Add("ExpiryDate", GetType(String))
            .Add("Loyalty", GetType(String))
            .Add("MembershipSinceDate", GetType(String))
            .Add("PriceCode", GetType(String))
            .Add("PriceCodeDescription", GetType(String))
            .Add("PriceBand", GetType(String))
            .Add("PriceBandDescription", GetType(String))
        End With


        Try
            Do While bMoreRecords

                PARAMOUT = CallWS272R(totalRRN, lastRRN, lastSeatID)

                Dim hasError As Boolean = True
                dRow = Nothing
                dRow = DtStatusResults.NewRow
                If PARAMOUT.Substring(10239, 1) = "E" Or PARAMOUT.Substring(10237, 2).Trim <> "" Then
                    dRow("ErrorOccurred") = "E"
                    dRow("ReturnCode") = PARAMOUT.Substring(10237, 2)
                    bMoreRecords = False
                    hasError = True
                Else
                    dRow("ErrorOccurred") = ""
                    dRow("ReturnCode") = ""
                    totalRRN = PARAMOUT.Substring(10188, 5)
                    lastRRN = PARAMOUT.Substring(10193, 5)
                    bMoreRecords = PARAMOUT.Substring(10198, 1).ConvertFromISeriesYesNoToBoolean
                    lastSeatID = PARAMOUT.Substring(10199, 37)
                    hasError = False

                End If
                DtStatusResults.Rows.Add(dRow)

                If Not hasError Then
                    Dim iPosition As Integer = 0
                    Dim iCounter As Integer = 1
                    Do While iCounter <= 50
                        If PARAMOUT.Substring(iPosition, 200).Trim = "" Then
                            Exit Do
                        Else
                            dRow = Nothing
                            dRow = dtCustomerMemberships.NewRow
                            dRow("MembershipDesc") = (PARAMOUT.Substring(iPosition, 40)).Trim()
                            dRow("MembershipNumber") = (PARAMOUT.Substring(iPosition + 40, 19)).Trim()
                            dRow("PurchaseDate") = (PARAMOUT.Substring(iPosition + 59, 7)).Trim()
                            dRow("ExpiryDate") = (PARAMOUT.Substring(iPosition + 66, 7)).Trim()
                            dRow("Loyalty") = (PARAMOUT.Substring(iPosition + 73, 5)).Trim()
                            dRow("MembershipSinceDate") = (PARAMOUT.Substring(iPosition + 78, 7)).Trim()
                            dRow("PriceCode") = (PARAMOUT.Substring(iPosition + 85, 2)).Trim()
                            dRow("PriceCodeDescription") = (PARAMOUT.Substring(iPosition + 87, 40)).Trim()
                            dRow("PriceBand") = (PARAMOUT.Substring(iPosition + 127, 1)).Trim()
                            dRow("PriceBandDescription") = (PARAMOUT.Substring(iPosition + 128, 30)).Trim()
                            dtCustomerMemberships.Rows.Add(dRow)
                            iPosition = iPosition + 200
                            iCounter = iCounter + 1
                        End If
                    Loop
                End If

            Loop

        Catch ex As Exception
            ResultDataSet = Nothing
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "WS272R"
                .HasError = True
            End With
        End Try
        Return err
    End Function

    Private Function CallWS272R(ByVal totalRRN As String, ByVal lastRRN As String, ByVal lastSeatID As String) As String
        Dim cmdSELECT As iDB2Command = Nothing
        Dim parmIO As iDB2Parameter
        Dim PARAMOUT As String = String.Empty
        Dim strHEADER As New StringBuilder

        strHEADER.Append("CALL ")
        strHEADER.Append(Settings.StoredProcedureGroup.Trim)
        strHEADER.Append("/WS272R(@PARAM1)")

        cmdSELECT = New iDB2Command(strHEADER.ToString(), conTALENTTKT)
        parmIO = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 10240)
        parmIO.Value = WS272RParm(totalRRN, lastRRN, lastSeatID)
        parmIO.Direction = ParameterDirection.InputOutput
        cmdSELECT.ExecuteNonQuery()
        PARAMOUT = cmdSELECT.Parameters(Param1).Value.ToString
        Return PARAMOUT
    End Function

    Private Function WS272RParm(ByVal totalRRN As String, ByVal lastRRN As String, ByVal lastSeatID As String) As String
        Dim myString As New StringBuilder
        ' myString.Append(Utilities.FixStringLength(String.Empty, 10172))
        ' myString.Append(Utilities.FixStringLength(_de.MembershipsProductSubType, 4))

        myString.Append(Utilities.FixStringLength(String.Empty, 10076))
        myString.Append(Utilities.FixStringLength(_de.MembershipsProductSubType, 100))
        myString.Append(Utilities.PadLeadingZeros(_de.CustomerNumber, 12))
        myString.Append(Utilities.PadLeadingZeros(totalRRN, 5))
        myString.Append(Utilities.PadLeadingZeros(lastRRN, 5))
        myString.Append(Utilities.FixStringLength("", 1))
        myString.Append(Utilities.FixStringLength(lastSeatID, 37))
        myString.Append(Utilities.FixStringLength(Settings.OriginatingSourceCode, 1))
        myString.Append(Utilities.FixStringLength("", 3))
        Return myString.ToString()
    End Function

    Private Function AccessDatabaseCD020S() As ErrorObj
        Dim err As New ErrorObj
        ResultDataSet = New DataSet

        'Create the Status data table
        Dim dtStatusResults As New DataTable("StatusResults")
        ResultDataSet.Tables.Add(dtStatusResults)
        With dtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        'Create the Customer Activities Details data table
        Dim dtCustomersAtAddress As New DataTable("CustomersAtAddress")
        'With dtCustomersAtAddress.Columns
        '    .Add("CustomerNumber", GetType(String))
        '    .Add("Forname", GetType(Integer))
        '    .Add("Surname", GetType(String))
        'End With
        ResultDataSet.Tables.Add(dtCustomersAtAddress)

        Try
            CallCD020S()
        Catch ex As Exception
            Const strError8 As String = "Error during database Access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError8
                .ErrorNumber = "TACDBCustsAtAddr-CD020S"
                .HasError = True
            End With
        End Try
        Return err
    End Function

    Private Sub CallCD020S()
        Dim _cmd As iDB2Command = Nothing
        Dim _cmdAdapter As iDB2DataAdapter = Nothing
        _cmd = conTALENTTKT.CreateCommand()
        _cmd.CommandText = "CALL CD020S( @ERRORCODE, @SOURCE, @PARAM3, @PARAM4, @PARAM5, @PARAM6, @PARAM7, @PARAM8)"
        _cmd.CommandType = CommandType.Text

        Dim pAddressLine2 As iDB2Parameter
        Dim pAddressLine3 As iDB2Parameter
        Dim pAddressLine4 As iDB2Parameter
        Dim pAddressLine5 As iDB2Parameter
        Dim pCountry As iDB2Parameter
        Dim pPostCode As iDB2Parameter
        Dim pSource As iDB2Parameter
        Dim pErrorCode As iDB2Parameter

        pErrorCode = _cmd.Parameters.Add(ErrorCode, iDB2DbType.iDB2Char, 10)
        pSource = _cmd.Parameters.Add(Source, iDB2DbType.iDB2Char, 1)
        pAddressLine2 = _cmd.Parameters.Add(Param3, iDB2DbType.iDB2Char, 30)
        pAddressLine3 = _cmd.Parameters.Add(Param4, iDB2DbType.iDB2Char, 30)
        pAddressLine4 = _cmd.Parameters.Add(Param5, iDB2DbType.iDB2Char, 25)
        pAddressLine5 = _cmd.Parameters.Add(Param6, iDB2DbType.iDB2Char, 25)
        pCountry = _cmd.Parameters.Add(Param7, iDB2DbType.iDB2Char, 20)
        pPostCode = _cmd.Parameters.Add(Param8, iDB2DbType.iDB2Char, 8)

        pErrorCode.Direction = ParameterDirection.InputOutput

        pAddressLine2.Value = _de.AddressLine2
        pAddressLine3.Value = _de.AddressLine3
        pAddressLine4.Value = _de.AddressLine4
        pAddressLine5.Value = _de.AddressLine5
        pCountry.Value = _de.Country
        pPostCode.Value = _de.PostCode.Replace(" ", "")
        pSource.Value = GlobalConstants.SOURCE
        pErrorCode.Value = String.Empty

        _cmdAdapter = New IBM.Data.DB2.iSeries.iDB2DataAdapter
        _cmdAdapter.SelectCommand = _cmd
        _cmdAdapter.Fill(ResultDataSet, "CustomersAtAddress")
        ConvertISeriesTables(ResultDataSet)

        Dim drStatus As DataRow = ResultDataSet.Tables("StatusResults").NewRow
        If CStr(_cmd.Parameters(ErrorCode).Value).Trim.Length > 0 Then
            drStatus("ErrorOccurred") = GlobalConstants.ERRORFLAG
            drStatus("ReturnCode") = CStr(_cmd.Parameters(Param2).Value).Trim
        Else
            drStatus("ErrorOccurred") = String.Empty
            drStatus("ReturnCode") = String.Empty
        End If
        ResultDataSet.Tables("StatusResults").Rows.Add(drStatus)
    End Sub


    Private Function AccessDatabaseWS165R() As ErrorObj
        Dim err As New ErrorObj
        ResultDataSet = New DataSet

        'Create the Status data table
        Dim dtStatusResults As New DataTable("StatusResults")
        ResultDataSet.Tables.Add(dtStatusResults)
        With dtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        Try
            CallWS165R()
        Catch ex As Exception
            Const strError8 As String = "Error during database Access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError8
                .ErrorNumber = "TACDBUpdCustAddr-WS165R"
                .HasError = True
            End With
        End Try
        Return err
    End Function

    Private Function CallWS165R()


        Dim cmdSELECT As iDB2Command = Nothing
        Dim parmIO As iDB2Parameter
        Dim PARAMOUT As String = String.Empty
        Dim strHEADER As New StringBuilder

        strHEADER.Append("CALL ")
        strHEADER.Append(Settings.StoredProcedureGroup.Trim)
        strHEADER.Append("/WS165R(@PARAM1)")

        cmdSELECT = New iDB2Command(strHEADER.ToString(), conTALENTTKT)
        parmIO = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 32765)
        parmIO.Value = WS165Parm()
        parmIO.Direction = ParameterDirection.InputOutput
        cmdSELECT.ExecuteNonQuery()
        PARAMOUT = cmdSELECT.Parameters(Param1).Value.ToString

        Dim drStatus As DataRow = ResultDataSet.Tables("StatusResults").NewRow
        If PARAMOUT.Substring(19999, 1) = "E" Or PARAMOUT.Substring(19997, 2).Trim <> "" Then
            drStatus("ErrorOccurred") = GlobalConstants.ERRORFLAG
            drStatus("ReturnCode") = PARAMOUT.Substring(19997, 2)
        Else
            drStatus("ErrorOccurred") = String.Empty
            drStatus("ReturnCode") = String.Empty
        End If
        ResultDataSet.Tables("StatusResults").Rows.Add(drStatus)

        Return PARAMOUT
    End Function


    Private Function RetrieveActivities() As ErrorObj
        Dim err As ErrorObj = New ErrorObj
        Me.ResultDataSet = New DataSet
        With Me.ResultDataSet.Tables
            .Add("StatusResults")
            .Add("CustomerActivities")
        End With

        Dim resultStatusColumns As Data.DataColumnCollection = Me.ResultDataSet.Tables("StatusResults").Columns
        resultStatusColumns.Add(New DataColumn("ErrorOccurred"))
        resultStatusColumns.Add(New DataColumn("ReturnCode"))
        resultStatusColumns.Add(New DataColumn("ProductCode"))
        resultStatusColumns.Add(New DataColumn("ProductDescription"))

        Dim attributeColumns As Data.DataColumnCollection = Me.ResultDataSet.Tables("CustomerActivities").Columns
        attributeColumns.Add(New DataColumn("Id"))
        attributeColumns.Add(New DataColumn("Name"))
        attributeColumns.Add(New DataColumn("Date", GetType(Date)))
        attributeColumns.Add(New DataColumn("Subject"))
        attributeColumns.Add(New DataColumn("UserName"))
        attributeColumns.Add(New DataColumn("UserCode"))
        attributeColumns.Add(New DataColumn("TemplateId"))

        With Me.ResultDataSet.Tables("StatusResults").Rows.Add
            .Item("ErrorOccurred") = ""
            .Item("ReturnCode") = ""
            .Item("ProductCode") = ""
            .Item("ProductDescription") = ""
        End With

        With Me.ResultDataSet.Tables("CustomerActivities").Rows.Add
            .Item("Id") = "101"
            .Item("Name") = "Passport Details"
            .Item("Date") = Utilities.ISeriesDate("10/10/2013")
            .Item("Subject") = "My Passport"
            .Item("UserName") = "User1"
            .Item("UserCode") = "TKT472TF"
            .Item("TemplateId") = "62"
        End With

        With Me.ResultDataSet.Tables("CustomerActivities").Rows.Add
            .Item("Id") = "102"
            .Item("Name") = "Next of Kin Information"
            .Item("Date") = Utilities.ISeriesDate("11/11/2013")
            .Item("Subject") = "Next Of Kin"
            .Item("UserName") = "User1"
            .Item("UserCode") = "TKT472TF"
            .Item("TemplateId") = "63"
        End With

        With Me.ResultDataSet.Tables("CustomerActivities").Rows.Add
            .Item("Id") = "103"
            .Item("Name") = "Car Details"
            .Item("Date") = Utilities.ISeriesDate("12/12/2013")
            .Item("Subject") = "Ferrari"
            .Item("UserName") = "User1"
            .Item("UserCode") = "TKT472TF"
            .Item("TemplateId") = "64"
        End With

        With Me.ResultDataSet.Tables("CustomerActivities").Rows.Add
            .Item("Id") = "104"
            .Item("Name") = "Passport Details"
            .Item("Date") = Utilities.ISeriesDate("10/10/2013")
            .Item("Subject") = "My Passport"
            .Item("UserName") = "User2"
            .Item("UserCode") = "TKT473TF"
            .Item("TemplateId") = "62"
        End With

        With Me.ResultDataSet.Tables("CustomerActivities").Rows.Add
            .Item("Id") = "105"
            .Item("Name") = "Next of Kin Information"
            .Item("Date") = Utilities.ISeriesDate("11/11/2013")
            .Item("Subject") = "Next Of Kin"
            .Item("UserName") = "User2"
            .Item("UserCode") = "TKT473TF"
            .Item("TemplateId") = "63"
        End With

        With Me.ResultDataSet.Tables("CustomerActivities").Rows.Add
            .Item("Id") = "106"
            .Item("Name") = "Car Details"
            .Item("Date") = Utilities.ISeriesDate("12/12/2013")
            .Item("Subject") = "Ferrari"
            .Item("UserName") = "User2"
            .Item("UserCode") = "TKT473TF"
            .Item("TemplateId") = "64"
        End With

        With Me.ResultDataSet.Tables("CustomerActivities").Rows.Add
            .Item("Id") = "107"
            .Item("Name") = "Passport Details"
            .Item("Date") = Utilities.ISeriesDate("10/10/2013")
            .Item("Subject") = "My Passport"
            .Item("UserName") = "User3"
            .Item("UserCode") = "TKT474TF"
            .Item("TemplateId") = "62"
        End With

        With Me.ResultDataSet.Tables("CustomerActivities").Rows.Add
            .Item("Id") = "108"
            .Item("Name") = "Next of Kin Information"
            .Item("Date") = Utilities.ISeriesDate("11/11/2013")
            .Item("Subject") = "Next Of Kin"
            .Item("UserName") = "User3"
            .Item("UserCode") = "TKT474TF"
            .Item("TemplateId") = "63"
        End With

        With Me.ResultDataSet.Tables("CustomerActivities").Rows.Add
            .Item("Id") = "109"
            .Item("Name") = "Car Details"
            .Item("Date") = Utilities.ISeriesDate("12/12/2013")
            .Item("Subject") = "Ferrari"
            .Item("UserName") = "User3"
            .Item("UserCode") = "TKT474TF"
            .Item("TemplateId") = "64"
        End With
        Return err
    End Function

End Class