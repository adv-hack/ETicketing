Imports System.Data
Imports Talent.eCommerce
Imports Talent.Common

Partial Class UserControls_ChangeBU
    Inherits ControlBase

#Region "Class Level Fields"
    Private _ucr As New Talent.Common.UserControlResource
    Private _log As Talent.Common.TalentLogging = Talent.eCommerce.Utilities.TalentLogging
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
    Private _errormessage As String = String.Empty
    Private _currentRootDir As String = String.Empty
    Private _success As Boolean = False
#End Region

#Region "Protected Methods"

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        With _ucr
            .BusinessUnit = TalentCache.GetBusinessUnit
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
            .PageCode = Talent.eCommerce.Utilities.GetCurrentPageName()
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "ChangeBU.ascx"
        End With

        ' If this control is to be displayed then process....
        If ModuleDefaults.ChangeBusinessUnitAllowed Then
            _success = LoadRepeater()
        End If
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load



        Me.blErrList.Items.Clear()
        Me.plhErrlist.Visible = False

    End Sub


    Protected Sub rptBusinessUnits_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptBusinessUnits.ItemDataBound

        If e.Item.ItemType = ListItemType.AlternatingItem OrElse e.Item.ItemType = ListItemType.Item Then

            Dim itemData As DataRow = CType(e.Item.DataItem, DataRowView).Row
            Dim sChangeToBusinessUnit As String = Talent.Common.Utilities.CheckForDBNull_String(itemData("BUSINESS_UNIT"))
            Dim sChangeToURL As String = Talent.Common.Utilities.CheckForDBNull_String(itemData("URL"))
            Dim sChangeToImagePath As String = Talent.Common.Utilities.CheckForDBNull_String(itemData("CHANGETO_IMAGE_PATH"))
            Dim sChangeToRootDir As String = Talent.Common.Utilities.CheckForDBNull_String(itemData("CHANGETO_ROOT_DIR"))

            Dim btnBusinessUnit As ImageButton = CType(e.Item.FindControl("btnBusinessUnit"), ImageButton)
            If Not btnBusinessUnit Is Nothing Then
                btnBusinessUnit.ImageUrl = sChangeToImagePath
                btnBusinessUnit.CommandArgument = sChangeToBusinessUnit + ";" + sChangeToURL + ";" + sChangeToRootDir
                If itemData("BUSINESS_UNIT").ToString.Trim = TalentCache.GetBusinessUnit.ToString.Trim Then
                    btnBusinessUnit.Enabled = False
                End If
            End If
        End If

    End Sub

    Protected Sub rptBusinessUnits_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.RepeaterCommandEventArgs) Handles rptBusinessUnits.ItemCommand

        If e.Item.ItemType = ListItemType.AlternatingItem OrElse e.Item.ItemType = ListItemType.Item Then
            Dim sArg As String = CType(e.CommandArgument, String)
            _success = ChangeToBU(sArg)
        End If

    End Sub

    Protected Sub SetText()
        With _ucr
            _errormessage = .Content("errormessage", _languageCode, True)
        End With
    End Sub


#End Region

#Region "Private Functions"

    Private Function LoadRepeater() As Boolean

        _success = True

        Dim dtBU As New Data.DataTable

        Try

            ' Retrieve set of tbl_url_bu records
            Dim TalentDataObj = New Talent.Common.TalentDataObjects()
            Dim settings As Talent.Common.DESettings = New Talent.Common.DESettings()
            settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            settings.DestinationDatabase = GlobalConstants.SQL2005DESTINATIONDATABASE
            TalentDataObj.Settings = settings
            dtBU = TalentDataObj.AppVariableSettings.TblUrlBu.GetChangeToBUs(TalentCache.GetBusinessUnitDeviceType, TalentCache.GetBusinessUnitURLGroup)

            ' Bind to repeater
            If dtBU IsNot Nothing AndAlso dtBU.Rows.Count > 1 Then
                Me.plhChangeBU.Visible = True
                rptBusinessUnits.DataSource = dtBU
                rptBusinessUnits.DataBind()
            Else
                Me.plhChangeBU.Visible = False
            End If

            ' Determine and store various settings for current business unit
            Dim currentBU As String = TalentCache.GetBusinessUnit.ToString.Trim.ToLower
            For Each dr As Data.DataRow In dtBU.Rows
                If dr("BUSINESS_UNIT").ToString.Trim.ToLower = currentBU Then
                    _currentRootDir = dr("CHANGETO_ROOT_DIR")
                End If
            Next

            TDataObjects = Nothing

        Catch ex As Exception
            Me.blErrList.Items.Add(_errormessage + " (" + ex.Message.ToString + ")")
            Me.plhErrlist.Visible = True
            _success = False
        End Try

        Return _success

    End Function

    Private Function ChangeToBU(ByVal sArg As String) As Boolean

        Dim sArgs As String() = sArg.Split(New Char() {";"c})
        Dim sNewBusinessUnit As String = sArgs(0)
        Dim sNewURL As String = sArgs(1)
        Dim sChangeToRootDir As String = sArgs(2)

        _success = True

        ' 1. Update any tbl_basket_header record with new business unit
        _success = UpdateBasket(sNewBusinessUnit)
        If _success Then

            ' 2. Update tbl_active_noise_session with new business unit
            _success = UpdateNoise(sNewBusinessUnit)
            If _success Then

                ' 3. Clear session cache items for BU
                _success = UpdateCache()
                If _success Then

                    ' 4a.  Generate new URL for redirection
                    Dim redirectURL As String = GenerateRedirectURL(sChangeToRootDir)

                    ' 4b.  Redirect to new URL
                    If redirectURL = String.Empty Then
                        Me.blErrList.Items.Add(_errormessage)
                        Me.plhErrlist.Visible = True
                        _success = False
                    Else
                        Response.Redirect(redirectURL)
                    End If

                End If
            End If
        End If

        Return _success

    End Function

    Private Function UpdateBasket(ByVal sBusinessUnit As String) As Boolean

        _success = True

        ' Update tbl_basket_header record with new business_unit
        Dim basketHeader As New TalentBasketDatasetTableAdapters.tbl_basket_headerTableAdapter
        basketHeader.Update_Business_unit(sBusinessUnit, Profile.Basket.Basket_Header_ID)

        ' Remove any retail items from basket as these will be businessunit-specific price lists and prices etc.
        Dim basketDetails As New TalentBasketDatasetTableAdapters.tbl_basket_detailTableAdapter
        basketDetails.Delete_All_Retail_Items(Profile.Basket.Basket_Header_ID)

        Return _success

    End Function

    Private Function UpdateNoise(ByVal sBusinessUnit As String) As Boolean

        _success = True

        Try
            ' Update tbl_active_noise_sessions.BUSINESS_UNIT to reflect new BU
            Dim noiseSettings As DESettings = Talent.eCommerce.Utilities.GetSettingsObject
            Dim dummyDateTime As DateTime
            noiseSettings.BusinessUnitNew = sBusinessUnit
            Dim myNoise As New TalentNoise(noiseSettings, Session.SessionID, dummyDateTime, dummyDateTime)

            Dim errorObj As Talent.Common.ErrorObj = myNoise.UpdateNoiseSessionBU()

            If ModuleDefaults.NOISE_IN_USE Then
                ' If noise update was not successful then set success as false
                If Not (myNoise.SuccessfullCall AndAlso myNoise.RowsAffected > 0) Then
                    Me.blErrList.Items.Add(_errormessage)
                    Me.plhErrlist.Visible = True
                    _success = False
                End If
                If errorObj.HasError Then
                    Me.blErrList.Items.Add(_errormessage + " (" + errorObj.ErrorMessage + ")")
                    _success = False
                End If
            End If

        Catch ex As Exception
            Me.blErrList.Items.Add(_errormessage + " (" + ex.Message.ToString + ")")
            Me.plhErrlist.Visible = True
            _success = False
        End Try

        Return _success

    End Function

    Private Function UpdateCache() As Boolean

        _success = True

        Try
            ' Remove each cached url item
            Dim e As IDictionaryEnumerator = HttpContext.Current.Cache.GetEnumerator
            While e.MoveNext
                If e.Current.Key.ToString.StartsWith("BU") Then
                    HttpContext.Current.Cache.Remove(e.Current.key)
                End If
            End While
        Catch ex As Exception
            Me.blErrList.Items.Add(_errormessage + " (" + ex.Message.ToString + ")")
            Me.plhErrlist.Visible = True
            _success = False
        End Try

        Return _success

    End Function

    Private Function GenerateRedirectURL(ByVal sChangeToRootDir As String) As String

        ' Ensure that the format the root directory of current business unit is correctly formatted (i.e. start with a '/' and not ending a '/')
        '        If _currentRootDir.ToString.Trim.EndsWith("/") Then _currentRootDir = _currentRootDir.Substring(0, _currentRootDir.Length)
        If Not _currentRootDir.ToString.Trim.EndsWith("/") Then _currentRootDir = _currentRootDir + "/"
        If Not _currentRootDir.ToString.Trim.StartsWith("/") Then _currentRootDir = "/" + _currentRootDir

        ' Ensure that the format the root directory of current business unit is correctly formattyed (i.e. start and ends with a '/')
        If Not sChangeToRootDir.ToString.Trim.EndsWith("/") Then sChangeToRootDir = sChangeToRootDir + "/"
        If Not sChangeToRootDir.ToString.Trim.StartsWith("/") Then sChangeToRootDir = "/" + sChangeToRootDir


        ' Get the current path and query string
        Dim redirectURL As String = HttpContext.Current.Request.Url.PathAndQuery

        ' Strip Path and query string of current root directory
        redirectURL = redirectURL.Substring(_currentRootDir.Length)

        ' Redirect URL is new root path plus stipped path + query string
        redirectURL = sChangeToRootDir + redirectURL

        Return redirectURL

    End Function

#End Region

End Class
