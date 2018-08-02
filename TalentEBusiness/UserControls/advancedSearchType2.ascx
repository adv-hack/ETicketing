<%@ Control Language="VB" AutoEventWireup="false" CodeFile="advancedSearchType2.ascx.vb"
    Inherits="UserControls_advancedSearchType2" %>
<div class="keyword">
    <label for="keyword">
        <asp:Label ID="keywordLabel" runat="server"></asp:Label>
    </label>
    <asp:TextBox ID="keyword" CssClass="input-l" runat="server"></asp:TextBox>
</div>
<div class="price">
    <asp:Label ID="priceLabel" runat="server" AssociatedControlID="priceFrom"></asp:Label>
    <asp:TextBox ID="priceFrom" CssClass="input-s" runat="server" MaxLength="0"></asp:TextBox>
    <asp:CustomValidator ID="checkPositiveNumericFrom" Display="Static" ControlToValidate="priceFrom"
        runat="server" ValidationGroup="AdvancedSearch" Text="*"></asp:CustomValidator>
    <asp:Label ID="toLabel" runat="server" AssociatedControlID="priceTo" CssClass="price-to"></asp:Label>
    <asp:TextBox ID="priceTo" CssClass="input-s" runat="server" MaxLength="0"></asp:TextBox>
    <asp:CustomValidator ID="checkPositiveNumericTo" Display="Static" ControlToValidate="priceTo"
        runat="server" ValidationGroup="AdvancedSearch" Text="*"></asp:CustomValidator>
    <asp:CustomValidator ID="checkRangeOK" Display="Static" ControlToValidate="priceTo"
        runat="server" ValidationGroup="AdvancedSearch" Text="*"></asp:CustomValidator>
</div>
<div class="type">
    <asp:Label ID="typeLabel" runat="server" AssociatedControlID="type"></asp:Label>
    <asp:DropDownList ID="type" CssClass="select" runat="server">
    </asp:DropDownList></div>
<div class="country">
    <asp:Label ID="countryLabel" runat="server" AssociatedControlID="country"></asp:Label>
    <asp:DropDownList ID="country" CssClass="select" runat="server">
    </asp:DropDownList></div>
<div class="region">
    <asp:Label ID="regionLabel" runat="server" AssociatedControlID="region"></asp:Label>
    <asp:DropDownList ID="region" CssClass="select" runat="server">
    </asp:DropDownList></div>
<div class="area">
    <asp:Label ID="areaLabel" runat="server" AssociatedControlID="area"></asp:Label>
    <asp:DropDownList ID="area" CssClass="select" runat="server">
    </asp:DropDownList></div>
<div class="grape">
    <asp:Label ID="grapeLabel" runat="server" AssociatedControlID="grape"></asp:Label>
    <asp:DropDownList ID="grape" CssClass="select" runat="server">
    </asp:DropDownList></div>
<div class="abv">
    <asp:Label ID="ABVLabel" runat="server" AssociatedControlID="abv"></asp:Label>
    <asp:DropDownList ID="abv" CssClass="select" runat="server">
    </asp:DropDownList></div>
<div class="producer">
    <asp:Label ID="producerLabel" runat="server" AssociatedControlID="producer"></asp:Label>
    <asp:DropDownList ID="producer" CssClass="select" runat="server">
    </asp:DropDownList></div>
