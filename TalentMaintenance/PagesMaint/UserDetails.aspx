<%@ Page Language="VB" MasterPageFile="~/MasterPages/MasterPage.master" AutoEventWireup="false"
    CodeFile="UserDetails.aspx.vb" Inherits="PagesMaint_UserDetails" Title="PagesMaint User Details" %>

<%@ Register Src="../UserControls/UserAddressEdit.ascx" TagName="UserAddressEdit"
    TagPrefix="uc2" %>
<%@ Register Src="../UserControls/UserEdit.ascx" TagName="UserDetails" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Content1" runat="Server">
    <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/PagesMaint/UserSelect.aspx">Return to Users Selection Screen</asp:HyperLink><br />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content2" runat="Server">
    <uc1:UserDetails ID="UserDetails1" runat="server" />
</asp:Content>
