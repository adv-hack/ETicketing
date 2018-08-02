<%@ Control Language="VB" AutoEventWireup="false" CodeFile="advancedSearchType3.ascx.vb"
    Inherits="UserControls_advancedSearchType3" %>
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
<div class="manufacture">
    <asp:Label ID="manufactureLabel" runat="server" AssociatedControlID="manufacture"></asp:Label>
    <asp:DropDownList ID="manufacture" CssClass="select" runat="server">
    </asp:DropDownList></div>
