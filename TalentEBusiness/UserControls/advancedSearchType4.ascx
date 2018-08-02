<%@ Control Language="VB" AutoEventWireup="false" CodeFile="advancedSearchType4.ascx.vb" Inherits="UserControls_advancedSearchType4" %>
<div class="advanced-search4">
    <asp:Label runat="server" ID="lblDropDownListGroup1" AssociatedControlID="DropDownListGroup1" />
    <asp:DropDownList ID="DropDownListGroup1" runat="server" AutoPostBack="true"></asp:DropDownList>
    <asp:Label runat="server" ID="lblDropDownListGroup2" AssociatedControlID="DropDownListGroup2" />
    <asp:DropDownList ID="DropDownListGroup2" runat="server" AutoPostBack="true"></asp:DropDownList>
</div>
<asp:Repeater ID="Repeater1" runat="server">
    <ItemTemplate>
        <div class="advanced-search4-dropdownlist3">
            <asp:Label runat="server" ID="lblDropDownList3" AssociatedControlID="DropDownList3" />
            <asp:DropDownList ID="DropDownList3" runat="server"></asp:DropDownList>
        </div>
    </ItemTemplate>
</asp:Repeater>
