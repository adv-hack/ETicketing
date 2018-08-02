Imports System.IO
Imports System.Collections.Generic
Imports System.Linq
Partial Class UserControls_ProgressBar
    Inherits ControlBase

    Public Property HTMLInclude As String

    Public Function ChooseStaticHTMLInclude() As String

        If ModuleDefaults.ShowProgressBar() And
            (Request.Url.ToString.ToUpper.Contains("COMPONENTSELECTION.ASPX") Or
             Request.Url.ToString.ToUpper.Contains("BASKET.ASPX") Or
             Request.Url.ToString.ToUpper.Contains("CHECKOUT.ASPX") Or
             Request.Url.ToString.ToUpper.Contains("CHECKOUTORDERCONFIRMATION.ASPX")) Then
            Return TicketingPackageProgressBar()
        End If

        If HTMLInclude IsNot Nothing Then
            Return GetStaticHTMLInclude(HTMLInclude)
        End If

        Return String.Empty
    End Function

    Private Function TicketingPackageProgressBar() As String
        Dim returnHTML As String = ""
        Dim ProgressTypesList As New List(Of String)
        Dim ProgressTypesArray As String()

        'Get the list of product Subtypes required
        ProgressTypesArray = ModuleDefaults.ProgressBarSubtypes.Split(",")
        ProgressTypesList = ProgressTypesArray.ToList()

        'Check if there is any of those product subtypes in the basket
        For Each item In Profile.Basket.BasketItems
            If ProgressTypesList.Contains(item.PRODUCT_SUB_TYPE) Then
                plhCustomerProgressBar.Visible = True
                'Check which page we're on then decide which include to use
                Select Case True
                    Case Request.Url.ToString.ToUpper.Contains("COMPONENTSELECTION.ASPX")
                        If Request.QueryString("Stage") IsNot Nothing Then
                            If Request.QueryString("Stage") = "1" Then
                                returnHTML = GetStaticHTMLInclude("componentProgress.html")
                            Else
                                returnHTML = GetStaticHTMLInclude("componentProgress2.html")
                            End If
                        End If
                    Case Request.Url.ToString.ToUpper.Contains("BASKET.ASPX")
                        returnHTML = GetStaticHTMLInclude("basketProgress.html")
                    Case Request.Url.ToString.ToUpper.Contains("CHECKOUT.ASPX")
                        returnHTML = GetStaticHTMLInclude("checkoutProgress.html")
                    Case Request.Url.ToString.ToUpper.Contains("CHECKOUTORDERCONFIRMATION.ASPX")
                        returnHTML = GetStaticHTMLInclude("successProgress.html")
                    Case Else
                        returnHTML = ""
                End Select
                'Can leave for loop if this is true for any product in basket
                Exit For
            End If
        Next

        Return returnHTML
    End Function

End Class
