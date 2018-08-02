Imports Microsoft.VisualBasic
Imports Talent.eCommerce
'--------------------------------------------------------------------------------------------------
'       Project                     Trading E-Commerce
'
'       Function                    Pages Login Order Return Confirmation
'
'       Date                        Dec 2008
'
'       Author                      Craig Mcloughlin 
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base   
'                                    
'       User Controls
'           orderReturn 
'
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Partial Class PagesLogin_orderReturnConfirmation
    Inherits TalentBase01

    Private wfr As New Talent.Common.WebFormResource
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

        Try

            With wfr
                .BusinessUnit = TalentCache.GetBusinessUnit()
                .PageCode = ProfileHelper.GetPageName
                .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile, .BusinessUnit)
                .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
                .KeyCode = "orderReturnConfirmation.aspx"
            End With

            Dim def As New Talent.eCommerce.ECommerceModuleDefaults.DefaultValues
            Dim defaults As New Talent.eCommerce.ECommerceModuleDefaults
            def = defaults.GetDefaults

            ' Retrieve the order return reference passed in via the query string.
            ' Send an email to the customer with confirmation details relating to
            ' the order return.
            Dim sReturnRef As String = String.Empty
            If Not String.IsNullOrEmpty(Request.QueryString("orderReturnRef")) Then
                sReturnRef = Request.QueryString("orderReturnRef")
            End If

            'Create the a Data Table from the retrieved list of returned orders.
            Dim ds As New Data.DataSet()
            Dim DtOrderReturnItems As New Data.DataTable("OrderReturnItems")
            ds.Tables.Add(DtOrderReturnItems)
            With DtOrderReturnItems.Columns
                .Add("CustomerNo", GetType(String))
                .Add("ProductDesc", GetType(String))
                .Add("Date", GetType(String))
                .Add("SeatDetails", GetType(String))
                .Add("OrderReturnReference", GetType(String))
            End With

            'Retrieve the Returned order information from session.
            ' Add this information to the DataSet for the email send.
            Dim myList As New Generic.List(Of String())(6)
            myList = CType(Session.Item("SelectedOrderList"), Generic.List(Of String()))
            For Each entry() As String In myList
                Dim dRow As Data.DataRow = Nothing
                dRow = DtOrderReturnItems.NewRow
                dRow("CustomerNo") = entry(0)
                dRow("ProductDesc") = entry(2)
                dRow("Date") = entry(1)
                dRow("SeatDetails") = entry(4)
                dRow("OrderReturnReference") = sReturnRef
                DtOrderReturnItems.Rows.Add(dRow)
            Next

            'What mode are we in
            Dim mode As String = "1"
            Select Case myList(0)(5).Trim.ToUpper
                Case Is = UCase("Returned") : mode = "2"
            End Select

            'Send the email
            If def.ConfirmationEmail Then
                Dim Order_Email As New Order_Email
                Order_Email.SendOrderReturnConfirmationEmail(def.OrdersFromEmail, ds, mode)
            End If

            'Clear the session data.
            Session.Remove("SelectedOrderList")

            'Clear cache
            Dim order As New Talent.Common.TalentOrder
            Dim settings As New Talent.Common.DESettings
            Dim err As New Talent.Common.ErrorObj
            settings.CacheStringExtension = "OrderReturnEnquiry.ascxcustomer=" & Talent.Common.Utilities.PadLeadingZeros(Profile.User.Details.Account_No_1, 12)
            order.Settings() = settings
            err = order.OrderDetailsClearCache()

            'Set the screen header text
            Select Case mode
                Case Is = "1" : PageHeaderTextLabel.Text = wfr.Content("returnHeaderText", _languageCode, True)
                Case Is = "2" : PageHeaderTextLabel.Text = wfr.Content("rebookHeaderText", _languageCode, True)
            End Select

        Catch ex As Exception

        End Try

        

    End Sub
End Class
