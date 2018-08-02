<%@ Page Language="VB" MasterPageFile="~/MasterPages/masterpage.master" AutoEventWireup="false" CodeFile="NavigationMaintenance.aspx.vb" Inherits="Navigation_NavigationMaintenance" ValidateRequest="false" %>
<%@ Register Src="../UserControls/PageLeftGroupNav.ascx" TagName="PageLeftGroupNav" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Content1" Runat="Server">
    <div id="pageMaintenanceTopNavigation">
       <asp:BulletedList ID="navigationOptions" runat="server" DisplayMode="HyperLink">
            <asp:listItem Text="Maintenance Portal" Value="../MaintenancePortal.aspx" />
       </asp:BulletedList>
    </div>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Content2" Runat="Server">
    <p class="title">
        <asp:Label ID="titleLabel" runat="server" Text="Group Navigation Maintenance" />
    </p>
   <uc1:PageLeftGroupNav runat="server" display="true" ID="PageLeftGroupNav"/>
</asp:Content>