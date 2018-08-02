<%@ Control Language="VB" AutoEventWireup="false" CodeFile="AccountSelector.ascx.vb"
    Inherits="UserControls_AccountSelector" %>
<div id="switch-account" class="default-table">
    <asp:Repeater ID="AccountsRepeater" runat="server">
        <HeaderTemplate>
            <table cellspacing="0">
                <tr>
                    <th class="acc-no">
                        <%#ucr.Content("AccountNumberColumnHeader", _languageCode, True)%>
                    </th>
                    <th class="login">
                        <%#ucr.Content("LoginIDColumnHeader", _languageCode, True)%>
                    </th>
                    <th class="email">
                        <%#ucr.Content("EmailAddressColumnHeader", _languageCode, True)%>
                    </th>
                    <th class="company">
                        <%#ucr.Content("CompanyColumnHeader", _languageCode, True)%>
                    </th>
                    <th class="address">
                        <%#ucr.Content("AddressColumnHeader", _languageCode, True)%>
                    </th>
                    <th class="postcode">
                        <%#ucr.Content("PostcodeColumnHeader", _languageCode, True)%>
                    </th>
                    <th class="switch">
                        &nbsp;
                    </th>
                </tr>
        </HeaderTemplate>
        <ItemTemplate>
            <tr>
                <td class="acc-no">
                    <asp:Label ID="AccountNumber" runat="server"></asp:Label></td>
                <td class="login">
                    <asp:Label ID="LoginID" runat="server"></asp:Label></td>
                <td class="email">
                    <asp:Label ID="Email" runat="server"></asp:Label></td>
                <td class="company">
                    <asp:Label ID="Company" runat="server"></asp:Label></td>
                <td class="address">
                    <asp:Label ID="Address" runat="server"></asp:Label></td>
                <td class="postcode">
                    <asp:Label ID="Postcode" runat="server"></asp:Label></td>
                <td class="switch">
                    <asp:Button ID="LoginButton" runat="server" OnClick="LoginAccount" CssClass="button" /></td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
            </table>
        </FooterTemplate>
    </asp:Repeater>
</div>
