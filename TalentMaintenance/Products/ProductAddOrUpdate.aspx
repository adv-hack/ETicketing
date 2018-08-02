<%@ Page Language="VB" MasterPageFile="~/MasterPages/MasterPage.master" AutoEventWireup="false" CodeFile="ProductAddOrUpdate.aspx.vb" Inherits="Products_ProductAddOrUpdate" ValidateRequest="false" %>
<%@ Register Src="../UserControls/ProductAddOrUpdate.ascx" TagName="ProductAddOrUpdate" TagPrefix="uc1" %>
<%@ Register Src="../UserControls/ProductMaintenanceTopNavigation.ascx" TagName="ProductMaintenanceTopNavigation" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Content1" runat="Server">
    
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Content2" runat="Server">
    <uc1:ProductMaintenanceTopNavigation ID="ProductMaintenanceTopNavigation1" runat="server" />
    <p class="title"><asp:Label ID="titleLabel" runat="server" /></p>
    <uc1:ProductAddOrUpdate ID="ProductAddOrUpdate1" runat="server" />
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true" />
</asp:Content>