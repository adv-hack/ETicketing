<%@ Page Language="VB" MasterPageFile="~/MasterPages/MasterPage.master" AutoEventWireup="false" CodeFile="BusinessUnitDetails.aspx.vb" Inherits="PagesMaint_BusinessUnitDetails" title="Untitled Page" %>

<%@ Register Src="../UserControls/BusinessUnitEdit.ascx" TagName="BusinessUnitEdit"
    TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Content1" runat="Server">
    <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/PagesMaint/BusinessUnitSelect.aspx">Return to Partners Selection Screen</asp:HyperLink>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content2" Runat="Server">
    <uc1:BusinessUnitEdit ID="BusinessUnitEdit1" runat="server" />
</asp:Content>

