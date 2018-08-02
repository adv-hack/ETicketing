<%@ Page Title="" Language="VB" MasterPageFile="~/MasterPages/PageMaintenanceMasterPage.master" AutoEventWireup="false" CodeFile="URLBUSelect.aspx.vb" Inherits="PagesMaint_URLBUSelect" ValidateRequest="false" %>

<%@ Register Src="../UserControls/URLBUList.ascx" TagName="URLBUList" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Content1" runat="Server">
    <div class="URLBUSelect_aspx error">
        <asp:BulletedList ID="blErrorList" runat="server">
        </asp:BulletedList>
    </div>
    <div class="URLBUSelect_aspx url-bu-selection">
        <p class="title"><asp:Label ID="pagetitleLabel" runat="server" Text="Label"></asp:Label></p>
        <p class="instructions"><asp:Label ID="pageinstructionsLabel" runat="server" Text="Label"></asp:Label></p>
       <uc1:URLBUList ID="URLBUList1" runat="server" />
    </div>
</asp:Content>
