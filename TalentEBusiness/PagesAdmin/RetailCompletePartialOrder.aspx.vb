
Imports Talent.eCommerce

Partial Class PagesAdmin_RetailCompletePartialOrder
    Inherits TalentBase01
#Region "Protected Functions"
    Protected Sub btnCompletePartialOrder_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCompletePartialOrder.Click
        If ModuleDefaults.CompletePartialOrder Then
            CompletePartialOrders()
        End If
    End Sub
#End Region

#Region "Private Function"
    Private Sub CompletePartialOrders()
        If Not String.IsNullOrEmpty(txtTempOrderID.Text) Then
            Profile.Basket.TempOrderID = txtTempOrderID.Text
            Dim orders As New TalentBasketDatasetTableAdapters.tbl_order_headerTableAdapter
            Dim dt As Data.DataTable = orders.Get_Header_By_Temp_Order_Id(txtTempOrderID.Text)
            If dt.Rows.Count > 0 Then
                Dim canProcess As Boolean = True
                If txtCanOveride.Text <> "TRUE" Then
                    If dt.Rows(0)("LOGINID") <> Profile.User.Details.LoginID Then
                        canProcess = False
                    End If
                End If
                If canProcess AndAlso dt.Rows(0)("STATUS") = Talent.Common.Utilities.GetOrderStatus("ORDER COMPLETE") Then
                    Dim order As New Order()
                    order.ProcessMerchandiseInBackend(False, False, String.Empty, False)
                    Response.Redirect("~/PagesLogin/Checkout/CheckoutOrderConfirmation.aspx")
                End If
            End If

        End If
    End Sub
#End Region


End Class
