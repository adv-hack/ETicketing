<%@ Page Language="VB" MasterPageFile="~/MasterPages/PageMaintenanceMasterPage.master"
    AutoEventWireup="false" CodeFile="MaintenancePortal.aspx.vb" Inherits="PagesMaint_MaintenancePortal"
    Title="Maintenance Portal" %>

<%@ Register Src="~/UserControls/MaintenanceMatrix.ascx" TagName="MaintenanceMatrix"
    TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Content1" runat="Server">
    <p>
        <asp:Label ID="titleLabel" runat="server" Text="Label"></asp:Label>
    </p>
    <p>
        <asp:Label ID="instructionsLabel" runat="server" Text="Label"></asp:Label>
    </p>

    <uc1:MaintenanceMatrix ID="MaintenanceMatrix1" runat="server" />
</asp:Content>
