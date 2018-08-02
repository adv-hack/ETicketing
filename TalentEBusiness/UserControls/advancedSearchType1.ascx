<%@ Control Language="VB" AutoEventWireup="false" CodeFile="advancedSearchType1.ascx.vb"
    Inherits="UserControls_advancedSearchType1" %>
<div class="keyword">
    <asp:Label ID="keywordLabel" runat="server" AssociatedControlID="keyword"></asp:Label>
    <asp:TextBox ID="keyword" CssClass="input-l" runat="server"></asp:TextBox>
</div>
<div class="price">
    <asp:Label ID="priceLabel" runat="server" AssociatedControlID="priceFrom"></asp:Label>
    <asp:TextBox ID="priceFrom" CssClass="input-s" runat="server" MaxLength="0"></asp:TextBox>
    <asp:CustomValidator ID="checkPositiveNumericFrom" Display="Static" ControlToValidate="priceFrom"
        runat="server" ValidationGroup="AdvancedSearch" Text="*"></asp:CustomValidator>
    <asp:Label ID="toLabel" runat="server" AssociatedControlID="priceTo" CssClass="price-to"></asp:Label>
    <asp:TextBox ID="priceTo" CssClass="input-s" runat="server" MaxLength="0">    </asp:TextBox>
    <asp:CustomValidator ID="checkPositiveNumericTo" Display="Static" ControlToValidate="priceTo"
        runat="server" ValidationGroup="AdvancedSearch" Text="*"></asp:CustomValidator>
    <asp:CustomValidator ID="checkRangeOK" Display="Static" ControlToValidate="priceTo"
        runat="server" ValidationGroup="AdvancedSearch" Text="*"></asp:CustomValidator>
</div>
