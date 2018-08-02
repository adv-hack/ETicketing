<%@ Control Language="VB" AutoEventWireup="false" CodeFile="CountryOfDelivery.ascx.vb"
    Inherits="UserControls_CountryOfDelivery" %>
<div id="country-of-delivery">
    <%--<label for="country">
        Country of delivery:</label>
    <select name="select" id="country">
        <option value="United Kingdom">United Kingdom</option>
    </select>--%>
    <asp:Label ID="lblCountry" runat="server" Text="Label"></asp:Label>
    <asp:DropDownList ID="ddlCountry" runat="server" CssClass="select" AutoPostBack="True">
    </asp:DropDownList>
      <asp:Label ID="lblLanguage" runat="server" Text="Label"></asp:Label>
    <asp:DropDownList ID="ddlLanguage" runat="server" CssClass="select" AutoPostBack="True">
    </asp:DropDownList>
</div>
