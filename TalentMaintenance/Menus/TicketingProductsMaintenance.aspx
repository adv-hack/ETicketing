<%@ Page Language="VB" MasterPageFile="~/MasterPages/MasterPage.master" AutoEventWireup="false" CodeFile="TicketingProductsMaintenance.aspx.vb" Inherits="Menus_TicketingProductsMaintenance" %>
<%@ Register Src="../UserControls/TicketingProducts.ascx" TagName="TicketingProducts" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Content1" runat="server">
    <div id="pageMaintenanceTopNavigation">
        <ul>
            <li><a href="../Default.aspx"><asp:Literal ID="ltlHomeLink" runat="server" /></a></li>
        </ul>
    </div>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Content2" Runat="Server">
    <uc1:TicketingProducts ID="TicketingProducts1" runat="server" />
</asp:Content>