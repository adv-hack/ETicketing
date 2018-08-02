<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/MasterPages/MasterPage.master" CodeFile="PartnerAdmin.aspx.vb" Inherits="Partners_PartnerAdmin" %>
<%@ Register Src="../UserControls/PartnerMaintenance.ascx" TagName="PartnerMaintenance" TagPrefix="uc1" %>

<asp:Content ID="Content3" ContentPlaceHolderID="Content1" runat="Server">
    <div id="pageMaintenanceTopNavigation">
        <ul>
            <li><a href="../Default.aspx"><asp:Literal ID="ltlHomeLink" runat="server" /></a></li>
        </ul>
    </div>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Content2" runat="Server">
    <uc1:PartnerMaintenance ID="PartnerMaintenance1" runat="server" />
</asp:Content>