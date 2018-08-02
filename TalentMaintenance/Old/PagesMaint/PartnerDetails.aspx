<%@ Page Language="VB" MasterPageFile="~/MasterPages/MasterPage.master" AutoEventWireup="false"
    CodeFile="PartnerDetails.aspx.vb" Inherits="PagesMaint_PartnerDetail"  Title="PagesMaint Partner Detail"  %>
<%@ Register Src="../UserControls/PartnerEdit.ascx" TagName="PartnerEdit" TagPrefix="uc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Content1" runat="Server">
    <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/PagesMaint/PartnerSelect.aspx">Return to Partners Selection Screen</asp:HyperLink>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content2" runat="Server">
    <uc2:PartnerEdit ID="PartnerEdit1" runat="server" />
</asp:Content>
