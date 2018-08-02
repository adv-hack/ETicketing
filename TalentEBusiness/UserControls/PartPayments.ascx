<%@ Control Language="VB" AutoEventWireup="false" CodeFile="PartPayments.ascx.vb" Inherits="UserControls_PartPayments" %>
<asp:PlaceHolder ID="plhErrorList" runat="server">
    <div class="alert-box alert">
        <asp:BulletedList ID="ErrorList" runat="server" />
    </div>
</asp:PlaceHolder>

<asp:PlaceHolder ID="plhOnAccount" runat="server" Visible="false">
    <div class="panel ebiz-on-account">
        <h2>
            <asp:Literal ID="ltlTitle" runat="server" /></h2>
        <asp:Repeater ID="rptOnAccount" runat="server">
            <HeaderTemplate>
                <table class="stack">
                    <thead>
                        <tr>
                            <th scope="col" class="ebiz-account"><%=AccountHeader%></th>
                            <th scope="col" class="ebiz-ammount"><%=AmountHeader%></th>
                            <th scope="col" class="ebiz-options"><%=OptionsHeader %></th>
                        </tr>
                    </thead>
                    <tbody>
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td class="ebiz-account" data-title="<%=AccountHeader%>">
                        <asp:Literal ID="ltlAccount" runat="server" /></td>
                    <td class="ebiz-ammount" data-title="<%=AmountHeader%>">
                        <asp:Literal ID="ltlAmount" runat="server" /></td>
                    <td class="ebiz-options" data-title="<%=OptionsHeader %>">
                        <asp:Button ID="btnRefund" runat="server" OnClick="btnRefund_OnClick" CssClass="button" />
                        <asp:HiddenField ID="hdfPartPaymentId" runat="server" />
                        <asp:HiddenField ID="hdfCardNumber" runat="server" />
                        <asp:HiddenField ID="hdfPaymentAmount" runat="server" />
                        <asp:HiddenField ID="hdfPaymentMethod" runat="server" />
                    </td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
                </tbody> </table>
            </FooterTemplate>
        </asp:Repeater>
    </div>
</asp:PlaceHolder>

<asp:PlaceHolder ID="plhOnAccountSummary" runat="server" Visible="false">
    <div class="row ebiz-on-account-summary">
        <div class="large-12 columns">
            <asp:Repeater ID="rptOnAccountSummary" runat="server">
                <HeaderTemplate>
                    <div class="header">
                        <div class="ebiz-on-account-summary-header">
                            <asp:Literal ID="ltlAccountHeader1" runat="server" />
                        </div>
                    </div>
                </HeaderTemplate>
                <ItemTemplate>
                    <div class="row">
                        <div class="medium-6 columns">
                            <asp:Literal ID="ltlAccount1" runat="server" />
                        </div>
                        <div class="medium-6 columns ebiz-values">
                            <asp:Literal ID="ltlAmount1" runat="server" />
                            <asp:HiddenField ID="hdfPartPaymentId" runat="server" />
                            <asp:HiddenField ID="hdfCardNumber" runat="server" />
                            <asp:HiddenField ID="hdfPaymentAmount" runat="server" />
                            <asp:HiddenField ID="hdfPaymentMethod" runat="server" />
                            <div class="ebiz-remove-fee-wrap">
                                <asp:PlaceHolder ID="plhRefund" runat="server" Visible="true">
                                    <asp:LinkButton ID="lnkButtonForRefund" Visible="true" runat="server" CommandName="ProcessRefund" CssClass="fa-input ebiz-fa-input ebiz-remove">
                                        <i id="iconForIncludeButton" runat="server" class="fa fa-times"></i>
                                    </asp:LinkButton>
                                </asp:PlaceHolder>
                            </div>
                        </div>    
                    </div>
                </ItemTemplate>
                <FooterTemplate>
                </FooterTemplate>
            </asp:Repeater>
        </div>
    </div>
</asp:PlaceHolder>

<asp:HiddenField ID="hdfOnAccountTotal" runat="server" />
