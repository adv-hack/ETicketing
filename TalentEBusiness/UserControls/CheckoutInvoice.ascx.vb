Imports Talent.eCommerce

Partial Class UserControls_CheckoutInvoice
    Inherits ControlBase

    Protected Function FinaliseOrder() As Boolean
        Try
            Dim orders As New TalentBasketDatasetTableAdapters.tbl_order_headerTableAdapter 
            Dim webOrderID As String = GenNewOrderID()
            For i As Integer = 1 To 3
                Try
                    orders.Complete_Order(webOrderID, Talent.Common.Utilities.GetOrderStatus("ORDER COMPLETE"), Now, Profile.Basket.TempOrderID)
                    If i > 1 Then Logging.WriteLog(Profile.UserName, _
                                                    "UCCIFO-030", _
                                                    String.Format("Finalise order failed initially but was completed at attempt {0}", i.ToString), _
                                                    String.Format("Order successfully finalised at attempt {0} - TempOrderID: {1} - WebOrderID: {2}", i.ToString, Profile.Basket.TempOrderID, webOrderID), _
                                                    TalentCache.GetBusinessUnit, _
                                                    TalentCache.GetPartner(Profile), _
                                                    ProfileHelper.GetPageName, _
                                                    "CheckoutInvoice.ascx")
                    Exit For
                Catch ex As Exception
                    If i = 3 Then
                        Logging.WriteLog(Profile.UserName, "UCCIFO-020", ex.Message, String.Format("Error finalising order, attempt {0} - TempOrderID: {1}", i.ToString, Profile.Basket.TempOrderID), TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile), ProfileHelper.GetPageName, "OrderSummary.ascx")
                    Else
                        System.Threading.Thread.Sleep(50)
                    End If
                End Try
            Next
            Try
                Dim dt As Data.DataTable = orders.Get_Header_By_Temp_Order_Id(Profile.Basket.TempOrderID)
                Dim total As Decimal = Utilities.CheckForDBNull_Decimal(dt.Rows(0)("TOTAL_ORDER_VALUE"))
                Dim promo As Decimal = Utilities.CheckForDBNull_Decimal(dt.Rows(0)("PROMOTION_VALUE"))
                total = total - promo

                orders.Set_Total_Amount_Charged(Talent.Common.Utilities.GetOrderStatus("ORDER COMPLETE"), _
                                                Now, _
                                                Talent.eCommerce.Utilities.RoundToValue(total, 0.01, False), _
                                                Profile.Basket.TempOrderID, _
                                                TalentCache.GetBusinessUnit, _
                                                Profile.UserName)

                orders.SetPaymentType(Request("code"), Profile.Basket.TempOrderID)
            Catch ex As Exception
            End Try
            UpdateTempOrderIDOnBasket()
        Catch ex As Exception
            Logging.WriteLog(Profile.UserName, "UCCIFO-010", ex.Message, "Error marking order header as finalised - TempOrderID: " & Profile.Basket.TempOrderID, TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile), ProfileHelper.GetPageName, "OrderSummary.ascx")
            Return False
        End Try
        Return True
    End Function

    Protected Function GenNewOrderID() As String
        'Start MBSTEB-1304
        Dim prefix As String = String.Empty
        Dim moduleDefaults As Talent.eCommerce.ECommerceModuleDefaults = New Talent.eCommerce.ECommerceModuleDefaults
        Dim def As Talent.eCommerce.ECommerceModuleDefaults.DefaultValues = moduleDefaults.GetDefaults
        If def.WebOrderNumberPrefixOverride.ToString.Trim.Length > 0 Then
            prefix = def.WebOrderNumberPrefixOverride.ToString.Trim
        Else
            prefix = TalentCache.GetBusinessUnit
        End If
        If Utilities.IsBasketHomeDelivery(Profile) Then
            prefix = "H" + prefix
        End If
        Return prefix & Talent.Common.Utilities.GetNextOrderNumber(TalentCache.GetBusinessUnit, _
                                TalentCache.GetPartner(Profile), _
                                ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString)
        '        Return TalentCache.GetBusinessUnit & Talent.Common.Utilities.GetNextOrderNumber(TalentCache.GetBusinessUnit, _
        '                               TalentCache.GetPartner(Profile), _
        '                               ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString)
        'End   MBSTEB-1304
    End Function

    Protected Function UpdateTempOrderIDOnBasket() As Boolean
        Try
            Dim basket As New TalentBasketDatasetTableAdapters.tbl_basket_headerTableAdapter
            For i As Integer = 1 To 3
                Try
                    basket.Update_Temp_order_id(Profile.Basket.TempOrderID, Profile.Basket.Basket_Header_ID)
                    Exit For
                Catch ex As Exception
                    If i = 3 Then
                        Logging.WriteLog(Profile.UserName, "UCCIUB-010", ex.Message, String.Format("Error finalising basket, attempt {0} - TempOrderID: {1}", i.ToString, Profile.Basket.TempOrderID), TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile), ProfileHelper.GetPageName, "CheckoutInvoice.ascx")

                    Else
                        System.Threading.Thread.Sleep(50)
                    End If
                End Try
            Next
        Catch ex As Exception
        End Try
        Return True
    End Function

    Protected Sub Insert_Order_Status_Flow(ByVal StatusName As String, Optional ByVal comment As String = "")
        Try
            Dim status As New TalentBasketDatasetTableAdapters.tbl_order_statusTableAdapter
            status.Insert_Order_Status_Flow(TalentCache.GetBusinessUnit, _
                                                Profile.Basket.TempOrderID, _
                                                Talent.Common.Utilities.GetOrderStatus(StatusName), _
                                                Now, _
                                                comment)
        Catch ex As Exception
        End Try
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Profile.Basket.IsEmpty Then
        Else
            Session("HSBCRequest") = Nothing
            Session("InvoiceRequest") = "TRUE"
            Response.Redirect("~/PagesLogin/Checkout/CheckoutOrderConfirmation.aspx?PaymentRequest=InvoiceRequest")
            'If FinaliseOrder() Then
            '    Session("HSBCRequest") = Nothing
            '    Insert_Order_Status_Flow("ORDER COMPLETE")
            '    Response.Redirect("~/PagesLogin/Checkout/CheckoutOrderConfirmation.aspx")
            'Else
            '    Insert_Order_Status_Flow("ORDER FAILED", "Order was not properly finalised")
            '    Response.Redirect("~/PagesLogin/Checkout/CheckoutOrderConfirmation.aspx")
            'End If
        End If
    End Sub
End Class
