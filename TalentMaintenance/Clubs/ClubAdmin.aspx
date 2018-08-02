<%@ Page Language="VB" MasterPageFile="~/MasterPages/MasterPage.master" AutoEventWireup="false" CodeFile="ClubAdmin.aspx.vb" Inherits="Clubs_ClubAdmin" %>
<%@ Register Src="../UserControls/ClubsMaintenance.ascx" TagName="ClubsMaintenance" TagPrefix="uc1" %>  

<asp:Content ID="Content1" ContentPlaceHolderID="Content1" Runat="Server">
    <div id="pageMaintenanceTopNavigation">
        <ul>
            <li><a href="../Default.aspx"><asp:Literal ID="ltlHomeLink" runat="server" /></a></li>
        </ul>
    </div>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Content2" Runat="Server">
    <uc1:ClubsMaintenance ID="ClubsMaintenanceControl" runat="server" />
</asp:Content>