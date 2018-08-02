Imports System.Collections.Generic

''' <summary>
''' Class used to control sensitive pages
''' </summary>
''' <remarks></remarks>
Public Class SensitivePages

    ''' <summary>
    ''' Returns true if the given page name is classed as sensitive otherwise returns false
    ''' </summary>
    ''' <param name="fullPageUrl">The given page url required to be checked</param>
    ''' <returns>True if the page is sensitive</returns>
    ''' <remarks></remarks>
    Public Shared Function IsSensitivePage(ByVal fullPageUrl As System.Uri) As Boolean
        Dim pageIsSensitive As Boolean = False
        If fullPageUrl.AbsoluteUri.Length > 0 AndAlso Not String.IsNullOrWhiteSpace(fullPageUrl.AbsoluteUri) Then
            Dim pageName As String = fullPageUrl.Segments(fullPageUrl.Segments.Length - 1)
            Select Case pageName.ToUpper()
                Case Is = "CHECKOUT.ASPX" : pageIsSensitive = True
                Case Is = "APPLYCASHBACK.ASPX" : pageIsSensitive = True
                Case Is = "CHECKOUTDELIVERYDETAILS.ASPX" : pageIsSensitive = True
                Case Is = "FRIENDSANDFAMILY.ASPX" : pageIsSensitive = True
                Case Is = "INVOICEENQUIRY.ASPX" : pageIsSensitive = True
                Case Is = "INVOICEENQUIRYDETAIL.ASPX" : pageIsSensitive = True
                Case Is = "CATCONFIRM.ASPX" : pageIsSensitive = True
                Case Is = "ORDERDETAILS.ASPX" : pageIsSensitive = True
                Case Is = "ORDERENQUIRY.ASPX" : pageIsSensitive = True
                Case Is = "TICKETEXCHANGESELECTION.ASPX" : pageIsSensitive = True
                Case Is = "TICKETEXCHANGEPRODUCTS.ASPX" : pageIsSensitive = True
                Case Is = "ORDERRETURN.ASPX" : pageIsSensitive = True
                Case Is = "ORDERRETURNCONFIRMATION.ASPX" : pageIsSensitive = True
                Case Is = "CHANGEPASSWORD.ASPX" : pageIsSensitive = True
                Case Is = "MYACCOUNT.ASPX" : pageIsSensitive = True
                Case Is = "PROFILEPHOTO.ASPX" : pageIsSensitive = True
                Case Is = "ONACCOUNT.ASPX" : pageIsSensitive = True
                Case Is = "SAVEMYCARD.ASPX" : pageIsSensitive = True
                Case Is = "UPDATEPROFILE.ASPX" : pageIsSensitive = True
                Case Is = "ACTIVATESMARTCARD.ASPX" : pageIsSensitive = True
                Case Is = "AMENDPPSENROLMENT.ASPX" : pageIsSensitive = True
                Case Is = "AMENDPPSPAYMENTS.ASPX" : pageIsSensitive = True
                Case Is = "EPURSE.ASPX" : pageIsSensitive = True
                Case Is = "PURCHASEHISTORY.ASPX" : pageIsSensitive = True
                Case Is = "PURCHASEDETAILS.ASPX" : pageIsSensitive = True
                Case Is = "VOUCHERS.ASPX" : pageIsSensitive = True
                Case Is = "PROFILEMEMBERSHIP.ASPX" : pageIsSensitive = True
                Case Is = "PROFILEATTRIBUTES.ASPX" : pageIsSensitive = True
                Case Is = "PROFILEACTIVITIES.ASPX" : pageIsSensitive = True
                Case Is = "EDITPROFILEACTIVITY.ASPX" : pageIsSensitive = True
                Case Is = "ONACCOUNTADJUSTMENT.ASPX" : pageIsSensitive = True
                Case Is = "CUSTOMERTEXT.ASPX" : pageIsSensitive = True
                Case Is = "COMPANYUPDATE.ASPX" : pageIsSensitive = True
                Case Is = "COMPANYCONTACTS.ASPX" : pageIsSensitive = True
                Case Else : pageIsSensitive = False
            End Select
            If Not pageIsSensitive Then
                If fullPageUrl.AbsoluteUri.ToUpper.Contains("ADDSEASONTICKETRENEWALSTOBASKET") Then pageIsSensitive = True
            End If
        End If
        Return pageIsSensitive
    End Function

End Class