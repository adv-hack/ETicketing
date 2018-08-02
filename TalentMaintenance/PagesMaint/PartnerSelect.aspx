<%@ Page Language="VB" MasterPageFile="~/MasterPages/MasterPage.master" AutoEventWireup="false" CodeFile="PartnerSelect.aspx.vb" Inherits="PagesMaint_PartnerSelect" title="PagesMaint Partner Select" %>

<%@ Register Src="../UserControls/PartnerList.ascx" TagName="PartnerList" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Content1" Runat="Server">
    &nbsp;<asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/Default.aspx">Return to Menu</asp:HyperLink>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content2" Runat="Server">
    <uc1:PartnerList ID="PartnerList1" runat="server" />
</asp:Content>

