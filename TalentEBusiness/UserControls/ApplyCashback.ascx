<%@ Control Language="VB" AutoEventWireup="false" CodeFile="ApplyCashback.ascx.vb" Inherits="UserControls_ApplyCashback" %>

<div class="ebiz-applycashback">
    <asp:Repeater ID="rptApplyCashback" runat="server" Visible="false" ViewStateMode="Enabled">
        <HeaderTemplate>
            <table class="stack">
                <thead>
                    <tr>
                        <th scope="col" class="supporter"><%=SupporterColumnHeader%></th>
                        <th scope="col" class="total-reward"><%=TotalRewardColumnHeader%></th>
                        <th scope="col" class="avail-reward"><%=AvailableRewardColumnHeader%></th>
                        <th scope="col" class="reward-value"><%=SelectThisRewardColumnHeader%></th>                          
                        <th scope="col" class="apply-amount"><%=ApplyAmountColumnHeader%></th>
                    </tr>
                </thead>
                <tbody>
        </HeaderTemplate>
        <ItemTemplate>
                    <tr>
                        <td class="supporter" data-title="<%=SupporterColumnHeader%>">
                            <span class="supporter-number"><%# DataBinder.Eval(Container.DataItem, "CustomerNumber").ToString().TrimStart(GlobalConstants.LEADING_ZEROS)%></span>
                            <asp:Literal ID="ltlSupporterNumberNameSeparator" runat="server" />
                            <span class="supporter-name"><%# GetCustomerName(DataBinder.Eval(Container.DataItem, "CustomerNumber").ToString)%></span>
                        </td>
                        <td class="total-reward" data-title="<%=TotalRewardColumnHeader%>"><%# FormatCurrency(DataBinder.Eval(Container.DataItem, "TotalReward"))%></td>
                        <td class="avail-reward" data-title="<%=AvailableRewardColumnHeader%>"><%# FormatCurrency(DataBinder.Eval(Container.DataItem, "AvailableReward"))%></td>
                        <td class="select-reward" data-title="<%=SelectThisRewardColumnHeader%>">
                            <asp:CheckBox ID="chkSelectThisReward" runat="server" Checked='<%# DataBinder.Eval(Container.DataItem, "RewardSelected").ToString%>' ViewStateMode="Enabled" />
                            <asp:HiddenField ID="hdfCustomerNumber" runat="server" Value='<%# DataBinder.Eval(Container.DataItem, "CustomerNumber").ToString%>' ViewStateMode="Enabled" />
                            <asp:HiddenField ID="hdfTotalReward" runat="server" Value='<%# DataBinder.Eval(Container.DataItem, "TotalReward").ToString %>' ViewStateMode="Enabled" />
                            <asp:HiddenField ID="hdfAvailableReward" runat="server" Value='<%# DataBinder.Eval(Container.DataItem, "AvailableReward").ToString %>' ViewStateMode="Enabled" />
                            <asp:HiddenField ID="hdfApplyMax" runat="server" Value='<%# Decimal.Round(DataBinder.Eval(Container.DataItem, "ApplyMax"),2).ToString %>' ViewStateMode="Enabled" />
                        </td>
                        <td class="apply-amount" data-title="<%=ApplyAmountColumnHeader%>">
                            <asp:TextBox runat="server" ID="txtApplyAmount" min="0" />
                            <asp:RegularExpressionValidator ID="rgxAmount" runat="server" ValidationGroup="Checkout" CssClass="error" 
                                        ControlToValidate="txtApplyAmount" OnLoad="SetRegex" />
                        </td>
                    </tr>
        </ItemTemplate>
        <FooterTemplate>
                </tbody> 
            </table>
        </FooterTemplate>
    </asp:Repeater>

    <asp:HiddenField runat="server" ID="hdfOnaccountEnabled" />

    <asp:PlaceHolder ID="plhTotalAvailable" runat="server">
        <asp:PlaceHolder ID="plhCashBackAvailable" runat="server">
        <div class="row ebiz-total-cashback-available">
            <div class="large-4 columns">
                <asp:Literal ID="ltlCashbackAvailableLabel" runat="server" />
            </div>
            <div class="large-8 columns">
                <asp:Literal ID="ltlCashbackAvailable" runat="server" />
            </div>
        </div>
        </asp:PlaceHolder>
        <div class="row ebiz-total-tickets-cost">
            <div class="large-4 columns">
                <asp:Literal ID="ltlTicketsCostLabel" runat="server" />
            </div>
            <div class="large-8 columns">
                <asp:Literal ID="ltlTicketsCost" runat="server" /><asp:HiddenField ID="hdfTicketsCost" runat="server" />
            </div>
        </div>
        <div class="row ebiz-total-balance">
            <div class="large-4 columns">
                <asp:Literal ID="ltlBalanceLabel" runat="server" />
            </div>
            <div class="large-8 columns">
                <asp:Literal ID="ltlBalance" runat="server" />
            </div>
        </div>
    </asp:PlaceHolder>

    <asp:PlaceHolder ID="plhTotalApplied" runat="server" Visible="false">
        <div class="row ebiz-cashback-applied">
            <div class="large-4 columns">
                <asp:Literal ID="ltlCashbackAppliedLabel" runat="server" />
            </div>
            <div class="large-8 columns">
                <asp:Literal ID="ltlCashbackApplied" runat="server" />
            </div>
        </div>
    </asp:PlaceHolder>
    
    <asp:Button ID="btnApplyNow" runat="server" CssClass="button" ValidationGroup="Checkout" />
    <asp:Button ID="btnClearAppliedCashback" runat="server" CssClass="button" />

</div>