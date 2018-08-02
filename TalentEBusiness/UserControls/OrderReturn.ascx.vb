Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.SqlClient
Imports Talent.Common
Imports Talent.eCommerce
Imports System.Xml
Imports System.Globalization
Imports System.Threading

'--------------------------------------------------------------------------------------------------
'       Project                     Trading E-Commerce
'
'       Function                    Order Return 
'
'       Date                        15th Dec '08
'
'       Author                      Craig Mcloughlin
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Partial Class UserControls_OrderReturn
    Inherits ControlBase
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
    Public ucr As New Talent.Common.UserControlResource
    Public errMsg As Talent.Common.TalentErrorMessages
    Dim myList As New Generic.List(Of String())(6)

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        With ucr
            .BusinessUnit = TalentCache.GetBusinessUnit()
            .PageCode = String.Empty
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "OrderReturn.ascx"
        End With

        ContinueButton.Text = ucr.Content("ContinueButtonText", _languageCode, True)
        BackButton.Text = ucr.Content("BackButton", _languageCode, True)


        errMsg = New Talent.Common.TalentErrorMessages(_languageCode, _
                                                        TalentCache.GetBusinessUnitGroup, _
                                                        TalentCache.GetPartner(Profile), _
                                                        ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString)

        If Not Page.IsPostBack Then

            If Not Session("errormsg") = "" Then
                Dim eli As New ListItem(getError(Session("errormsg").ToString))
                If Not errorlist Is Nothing Then
                    If Not errorlist.Items.Contains(eli) Then
                        errorlist.Items.Add(eli)
                    End If
                End If
            End If

        End If

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Visible And Not Page.IsPostBack Then
            Try
                showData()
            Catch ex As Exception
            End Try
        End If
    End Sub

    Protected Sub showData()

        Dim mode As String = "1"


        'Bind customer and order repeaters.
        Try
            myList = CType(Session.Item("SelectedOrderList"), Generic.List(Of String()))
            If Not myList Is Nothing AndAlso myList.Count > 0 Then

                'What mode are we in
                Select Case myList(0)(5).Trim.ToUpper
                    Case Is = UCase("Returned") : mode = "2"
                End Select


                Dim myCustomers As New Generic.List(Of String)

                For Each entry() As String In myList
                    If Not myCustomers.Contains(entry(0)) Then
                        myCustomers.Add(entry(0))
                    End If
                Next

                If myCustomers.Count > 0 Then
                    CustomerRepeater.DataSource = myCustomers
                    CustomerRepeater.DataBind()
                End If
            Else
                Response.Redirect("~/PagesLogin/Orders/OrderReturnEnquiry.aspx")
            End If

        Catch ex As Exception
            Response.Redirect("~/PagesLogin/Orders/OrderReturnEnquiry.aspx")
        End Try

        Dim dropDownStr As String = String.Empty

        'Set text and comment labels
        comment1Label.Text = GetText("comment1Label")
        comment2Label.Text = GetText("comment2Label")
        Select Case mode
            Case Is = "1"
                PageHeaderTextLabel.Text = GetText("returnHeaderText")
                dropDownStr = GetText("returnDropDownComments")
            Case Is = "2"
                PageHeaderTextLabel.Text = GetText("rebookHeaderText")
                dropDownStr = GetText("rebookDropDownComments")
        End Select

        'Populate drop-down comments list with values from the database
        Dim dropDownAry As String()
        Dim l1 As New ListItem
        dropDownAry = dropDownStr.Split(";")
        commentSelect.Items.Clear()
        With ucr
            For Each str As String In dropDownAry
                commentSelect.Items.Add(New ListItem(str))
            Next
        End With

    End Sub

    Protected Function getError(ByVal errCode As String) As String
        Return errMsg.GetErrorMessage(Talent.Common.Utilities.GetAllString, _
                            Talent.eCommerce.Utilities.GetCurrentPageName, _
                            errCode).ERROR_MESSAGE
    End Function

    Protected Function GetText(ByVal PValue As String) As String
        Dim str As String = ucr.Content(PValue, _languageCode, True)
        Return str
    End Function

    Protected Function showDate(ByVal Pvalue As DateTime) As String
        Dim str As String
        str = Pvalue.ToString("dd/MM/yyyy")
        Return str
    End Function

    Protected Sub CustomerRepeater_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles CustomerRepeater.ItemDataBound
        If e.Item.ItemIndex <> -1 Then
            Dim ordRpt As Repeater = CType(e.Item.FindControl("OrderRepeater"), Repeater)
            Dim ltlCustomerNumberLabel As Literal = CType(e.Item.FindControl("ltlCustomerNumberLabel"), Literal)
            Dim ltlCustomerNumberValue As Literal = CType(e.Item.FindControl("ltlCustomerNumberValue"), Literal)
            ltlCustomerNumberValue.Text = CStr(e.Item.DataItem)

            If Not ordRpt Is Nothing Then

                Dim myRows As New Generic.List(Of String())
                For Each entry() As String In myList
                    If ltlCustomerNumberValue.Text = entry(0) Then
                        myRows.Add(entry)
                    End If
                Next

                If myRows.Count Then
                    ordRpt.DataSource = myRows
                    ordRpt.DataBind()
                End If
            End If
            ltlCustomerNumberLabel.Text = ucr.Content("CustomerNumberLabel", _languageCode, True)
            ltlCustomerNumberValue.Text = ltlCustomerNumberValue.Text.TrimStart(GlobalConstants.LEADING_ZEROS)
        End If
    End Sub

    Protected Sub DoOrdersRepeaterItemDatabound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs)

        If e.Item.ItemIndex <> -1 Then

            Dim dateLabel As Literal = CType(e.Item.FindControl("dateLabel"), Literal)
            Dim matchLabel As Literal = CType(e.Item.FindControl("matchLabel"), Literal)
            Dim seatLabel As Literal = CType(e.Item.FindControl("seatLabel"), Literal)
            Dim statusLabel As Literal = CType(e.Item.FindControl("statusLabel"), Literal)

            Dim rowArray As String() = CType(e.Item.DataItem, String())

            dateLabel.Text = rowArray(1)
            matchLabel.Text = rowArray(2)
            seatLabel.Text = rowArray(4)
            statusLabel.Text = rowArray(5)

            ' translate if necessary
            Dim translateStatus As String = ucr.Content(rowArray(5).Trim.ToUpper, _languageCode, True)
            If translateStatus <> String.Empty Then
                statusLabel.Text = ucr.Content(rowArray(5).Trim.ToUpper, _languageCode, True)
            End If

        End If

    End Sub

    Protected Sub ContinueButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ContinueButton.Click

        If Page.IsPostBack Then

        End If

        errorlist.Items.Clear()
        'Comment Drop Down Box is manddatory
        If String.IsNullOrEmpty(commentSelect.SelectedItem.Text) Or commentSelect.SelectedItem.Text = "--" Then
            errorlist.Items.Add(ucr.Content("NoCommentErrorMessage", _languageCode, True))
        Else

            Session("errormsg") = ""
            Response.Redirect("~/Redirect/TicketingGateway.aspx?page=" & _
                                    Talent.eCommerce.Utilities.GetCurrentPageName & _
                                    "&function=orderReturn&comment1=" & commentSelect.SelectedItem.Text.Trim & _
                                    "&comment2=" & commentTxtBox.Text.Trim)
        End If
    End Sub


    Protected Sub BackButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles BackButton.Click
        Session.Remove("SelectedOrderList")
        Response.Redirect("~/PagesLogin/Orders/OrderReturnEnquiry.aspx")
    End Sub

    Protected Sub Page_PreRender(sender As Object, e As System.EventArgs) Handles Me.PreRender
        plhErrorList.Visible = (errorlist.Items.Count > 0)
        plhPageHeaderText.Visible = (PageHeaderTextLabel.Text.Length > 0)
    End Sub
End Class
