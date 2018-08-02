<%@ Page Language="VB" MasterPageFile="~/MasterPages/MasterPage.master" AutoEventWireup="false" CodeFile="DatabaseAdmin.aspx.vb" Inherits="Client_WebSite_DatabaseAdmin" title="Untitled Page" %>

<%@ Register Src="../../UserControls/DBAdmin.ascx" TagName="DBAdmin" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Content1" Runat="Server">
    <uc1:DBAdmin ID="DBAdmin1" runat="server" />
</asp:Content>

