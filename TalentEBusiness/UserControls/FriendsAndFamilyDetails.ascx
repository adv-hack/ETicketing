<%@ Control Language="VB" AutoEventWireup="false" CodeFile="FriendsAndFamilyDetails.ascx.vb" Inherits="UserControls_FriendsAndFamilyDetails" ViewStateMode="Disabled" %>

<asp:PlaceHolder ID="plhErrorMessage" runat="server">
    <p class="alert-box alert"><asp:Literal ID="ltlErrorLabel" runat="server" /></p>
</asp:PlaceHolder>

<asp:PlaceHolder ID="plhNoFFInList" runat="server">
    <p class="alert-box warning"><asp:Literal ID="ltlNoFFInList" runat="server" /></p>
</asp:PlaceHolder>

<asp:PlaceHolder ID="plhFFList" runat="server">
    <asp:Repeater ID="rptFriendsAndFamily" runat="server" ViewStateMode="Enabled">
        <HeaderTemplate>
            <div class="panel ebiz-friends-and-family-wrap">
                <table class="ebiz-responsive-table">
                    <thead>
                        <tr>
                            <th scope="col" class="ebiz-customer-number"><%#getText("ColumnsCustomer") %></th>
                            <th scope="col" class="ebiz-customer-name"><%#getText("ColumnsName") %></th>
                            <th scope="col" class="ebiz-customer-address"><%#getText("ColumnsAddress") %></th>
                            <asp:PlaceHolder ID="plhPostCodeColumn" runat="server">
                            <th scope="col" class="ebiz-customer-postcode"><%#getText("ColumnsPostCode") %></th>      
                            </asp:PlaceHolder>
                            <th scope="col" class="ebiz-delete-button"><%#getText("ColumnsDelete")%></th>
                        </tr>
                    </thead>
                <tbody>
        </HeaderTemplate>
        <ItemTemplate>
                <tr>
                    <td data-title="<%#getText("ColumnsCustomer") %>" class="ebiz-customer-number"><asp:Literal ID="ltlCustomerNumber" runat="server" ></asp:Literal></td>
                    <td data-title="<%#getText("ColumnsName") %>" class="ebiz-customer-name"><%#Eval("Forename")%> <%#Eval("Surname")%></td>
                    <td data-title="<%#getText("ColumnsAddress") %>" class="ebiz-customer-address"><%#Eval("AddressLine1")%></td>
                    <asp:PlaceHolder ID="plhPostCodeColumn" runat="server">
                    <td data-title="<%#getText("ColumnsPostCode") %>" class="ebiz-customer-postcode"><%#Eval("PostCode")%></td>
                    </asp:PlaceHolder>
                    <td><asp:LinkButton ID="lbtnDelete" runat="server"><i class="fa fa-times" /></asp:LinkButton></td>
                </tr>
        </ItemTemplate>
        <FooterTemplate>
                    </tbody>
                </table>
            </div>
        </FooterTemplate>
    </asp:Repeater>
</asp:PlaceHolder>