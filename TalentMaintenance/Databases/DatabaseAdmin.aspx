<%@ Page Language="VB" MasterPageFile="~/MasterPages/MasterPage.master" AutoEventWireup="false" CodeFile="DatabaseAdmin.aspx.vb" Inherits="Databases_DatabaseAdmin" %>
<%@ Register Src="../UserControls/DatabaseMaintenance.ascx" TagName="DatabaseMaintenance" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Content1" runat="Server">
    <div id="pageMaintenanceTopNavigation">
        <ul>
            <li><a href="../Default.aspx"><asp:Literal ID="ltlHomeLink" runat="server" /></a></li>
        </ul>
    </div>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Content2" Runat="Server">
    <uc1:DatabaseMaintenance ID="DatabaseMaintenance1" runat="server" />
</asp:Content>