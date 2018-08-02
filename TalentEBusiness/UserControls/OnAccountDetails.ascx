<%@ Control Language="VB" AutoEventWireup="false" CodeFile="OnAccountDetails.ascx.vb" Inherits="UserControls_OnAccountDetails" %>

<asp:PlaceHolder id="plhOnAccountDetails" runat="server" Visible="false">
    <div class="panel ebiz-on-account-details-wrap">
        <asp:PlaceHolder ID="plhHeader" runat="server">
            <h2><asp:Literal ID="ltlTitle" runat="server" /></h2>
        </asp:PlaceHolder>
        
        <asp:BulletedList ID="ErrorList" runat="server" CssClass="alert-box alert" />
        <asp:ValidationSummary ID="OrderSummary" runat="server" ValidationGroup="Checkout" CssClass="alert-box alert" />
        
        <asp:Repeater ID="rptOnAccountDetails" runat="server">
            <HeaderTemplate>
                <table class="stack ebiz-on-account-details-table">
                    <thead>
                        <tr>
                            <th class="ebiz-date"><%= SetText("ltlDate") %></th>
                            <th class="ebiz-activity"><%= SetText("ltlActivity") %></th>
                            <th class="ebiz-refund-from"><%= SetText("ltlRefundFrom")%></th>
                            <th class="ebiz-amount"><%= SetText("ltlAmount")%></th>
                            <th class="ebiz-description"><%= SetText("ltlProductDescription")%></th>
                            <th class="ebiz-seat"><%= SetText("ltlSeat")%></th>
                            <th class="ebiz-paymnentreference"><%= SetText("ltlPaymentReference")%></th>
                            <th class="ebiz-running-balance"><%= SetText("ltlRunningBalance")%></th>
                        </tr>
                    </thead>
                    <tbody>
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td data-title="<%#SetText("ltlDate") %>" class="ebiz-date"><asp:Label ID="lblDate" runat="server" Text='<%# Talent.Common.Utilities.CheckForDBNull_String(DataBinder.Eval(Container.DataItem, "Date"))%>'></asp:Label></td> 
                    <td data-title="<%#SetText("ltlActivity") %>" class="ebiz-activity"><asp:Label ID="lblActivityType" runat="server" Text=' <%# Talent.Common.Utilities.CheckForDBNull_String(DataBinder.Eval(Container.DataItem, "ActivtyType"))%>'></asp:Label></td> 
                    <td data-title="<%#SetText("ltlRefundFrom") %>" class="ebiz-refund-from"><asp:Label ID="lblRefundFrom" runat="server" Text=' <%# Talent.Common.Utilities.CheckForDBNull_String(DataBinder.Eval(Container.DataItem, "RefundFrom"))%>'></asp:Label></td> 
                    <td data-title="<%#SetText("ltlAmount") %>" class="ebiz-amount"><asp:Label ID="lblAmount" runat="server" Text=' <%# Talent.Common.Utilities.CheckForDBNull_String(DataBinder.Eval(Container.DataItem, "Amount"))%>'></asp:Label></td>  
                    <td data-title="<%#SetText("ltlProductDescription") %>" class="description"> <%# Talent.Common.Utilities.CheckForDBNull_String(DataBinder.Eval(Container.DataItem, "ProductDescription"))%></td>
                    <td data-title="<%#SetText("ltlSeat") %>" class="ebiz-seat"><%# Talent.Common.Utilities.CheckForDBNull_String(DataBinder.Eval(Container.DataItem, "Seat"))%></td>
                    <td data-title="<%#SetText("ltlPaymentReference") %>" class="ebiz-paymnentreference"><%# Talent.Common.Utilities.CheckForDBNull_String(DataBinder.Eval(Container.DataItem, "PaymentReference"))%></td>
                    <td data-title="<%#SetText("ltlRunningBalance") %>" class="ebiz-running-balance"><asp:Label ID="lblRunningBalance" runat="server" Text=' <%# Talent.Common.Utilities.CheckForDBNull_String(DataBinder.Eval(Container.DataItem, "RunningBalance"))%>'></asp:Label></td> 
                </tr>
            </ItemTemplate>
            <FooterTemplate>
                </tbody>
                </table>
            </FooterTemplate>
        </asp:Repeater>                       
    </div>    
     <div class="ebiz-submit">
      <asp:Button ID="btnManualAdjustment" runat="server" CssClass="button" OnClick="btnManualAdjustment_Click" />
    </div>
      <div class="ebiz-submit">
      <asp:Button ID="btnPWSRefundToCard" runat="server" CssClass="button" OnClick="btnPWSRefundToCard_Click" />
    </div>
</asp:PlaceHolder>

<asp:HiddenField ID="hdfOnAccountDetailsTotal" runat="server" />