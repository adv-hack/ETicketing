<%@ Page Language="VB" MasterPageFile="~/MasterPages/MasterPage.master" AutoEventWireup="false" CodeFile="BusinessUnitSelect.aspx.vb" Inherits="PagesMaint_BusinessUnitList" title="Untitled Page" %>

<%@ Register Src="../UserControls/BusinessUnitList.ascx" TagName="BusinessUnitList"
    TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Content1" Runat="Server">
    <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/Default.aspx">Return to Menu</asp:HyperLink>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content2" Runat="Server">
    <uc1:BusinessUnitList ID="BusinessUnitList1" runat="server" />
</asp:Content>

