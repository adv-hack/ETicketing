Partial Class PagesPublic_Basket_Comments
    Inherits TalentBase01

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' Get package ID and customer from URL query string
        If Not Page.IsPostBack Then
            Dim myPackageID As String = Request("packageID")
            Dim myCustomer As String = Request("Customer")
            Dim myProduct = Request("Product")
            Dim mySeat = Request("Seat")
            Dim myTempBasketID = Request("TempBasketID")

            'If comments already entered for package get them 
            If Session("comments_" & myPackageID & myProduct & mySeat + myTempBasketID) IsNot Nothing Then
                txtComments.Text = Session("comments_" & myPackageID + myProduct + mySeat + myTempBasketID)
            End If

            Dim wfr As New Talent.Common.WebFormResource
            With wfr
                .BusinessUnit = TalentCache.GetBusinessUnit()
                .PageCode = ProfileHelper.GetPageName
                .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile, .BusinessUnit)
                .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
                .KeyCode = "Comments.aspx"
            End With

            Dim _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
            rgxComments.Text = wfr.Content("MaxCommentLengthErrorText", _languageCode, True)
            btApplyComments.Text = wfr.Content("ApplyButtonText", _languageCode, True)
            btApplyCommentsNoScript.Text = btApplyComments.Text

            ' Set maximum comment length to 100 characters for Corporate and 50 for ticketing    
            If myPackageID Is Nothing Then
                rgxComments.ValidationExpression = "^[\s\S]{0,50}$"
                rgxComments.Text = rgxComments.Text & " 50"
            Else
                rgxComments.ValidationExpression = "^[\s\S]{0,100}$"
                rgxComments.Text = rgxComments.Text & " 100"
            End If
        End If
    End Sub

    Protected Sub btnComments_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btApplyComments.Click
        updateComments()
        Response.Redirect("~/PagesPublic/Basket/Basket.aspx")
    End Sub

    Protected Sub btnApplyCommentsNoScript_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btApplyCommentsNoScript.Click
        updateComments()
        Response.Redirect("~/PagesPublic/Basket/Basket.aspx")
    End Sub


    ''' <summary>
    ''' Update the added comments from what the user has entered
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub updateComments()
        Dim myCommentDataEntity As New Talent.Common.DEComments
        Dim myPackageID As String = String.Empty
        Dim myCustomer As String = String.Empty
        Dim myProduct As String = String.Empty
        Dim mySeat As String = String.Empty
        Dim myTempBasketID As String = String.Empty
        If Not Request("packageID") Is Nothing Then myPackageID = Request("packageID")
        If Not Request("Customer") Is Nothing Then myCustomer = Request("Customer")
        If Not Request("Product") Is Nothing Then myProduct = Request("Product")
        If Not Request("Seat") Is Nothing Then mySeat = Request("Seat")
        If Not Request("TempBasketID") Is Nothing Then myTempBasketID = Request("TempBasketID")

        ' save comments for package/product 
        Session("comments_" & myPackageID & myProduct & mySeat & myTempBasketID) = txtComments.Text
        txtComments.Text = txtComments.Text.Replace(vbCr, "").Replace(vbLf, "")

        ' Create and Populate the data entity    
        With myCommentDataEntity
            .CommentText = txtComments.Text
            .SessionID = Profile.Basket.Basket_Header_ID
            .CorporatePackageNumericID = myPackageID
            .ProductCode = myProduct
            .CustomerID = myCustomer
            .Seat = mySeat
            .TempBasketID = myTempBasketID
        End With

        ' Create a TalentComments object and set its   
        Dim myTalentComments As New Talent.Common.TalentComments
        With myTalentComments
            .CommentsDataEntity = myCommentDataEntity
            .Settings = Talent.eCommerce.Utilities.GetSettingsObject()
        End With

        ' Finally call method to add/update comments to the back end 
        Dim err As New Talent.Common.ErrorObj
        err = myTalentComments.AddOrderComments
    End Sub

    Protected Sub Page_PreInit1(sender As Object, e As EventArgs) Handles Me.PreInit
        'Page.Theme = ""
    End Sub
End Class
