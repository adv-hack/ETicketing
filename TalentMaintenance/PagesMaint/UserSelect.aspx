<%@ Page Language="VB" MasterPageFile="~/MasterPages/MasterPage.master" AutoEventWireup="false"
    CodeFile="UserSelect.aspx.vb" Inherits="PagesMaint_UserSelect" Title="PagesMaint User Select" %>

<%@ Register Src="../UserControls/UserList.ascx" TagName="UserList" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Content1" runat="Server">
    <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/Default.aspx">Return to Menu</asp:HyperLink>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content2" runat="Server">
    <uc1:UserList ID="UserList1" runat="server" />
</asp:Content>
