<%@ Page Language="VB" MasterPageFile="~/MasterPages/MasterPage.master" AutoEventWireup="false" CodeFile="UserAddress.aspx.vb" Inherits="PagesMaint_UserAddress" title="Untitled Page" %>

<%@ Register Src="../UserControls/UserAddressEdit.ascx" TagName="UserAddressEdit"
    TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Content1" Runat="Server">
    <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="<% = NavigateUrl %>">Return to Users Address Selection Screen</asp:HyperLink><br />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content2" Runat="Server">
    <uc1:UserAddressEdit ID="UserAddressEdit1" runat="server" />
</asp:Content>

