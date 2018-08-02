<%@ Page Title="" Language="VB" MasterPageFile="~/MasterPages/PageMaintenanceMasterPage.master"
    AutoEventWireup="false" CodeFile="URLBUDetail.aspx.vb" Inherits="PagesMaint_URLBUDetail"
    ValidateRequest="false" %>

<%@ Register Src="../UserControls/URLBUEdit.ascx" TagName="URLBUEdit" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Content1" runat="Server">
    <div class="URLBUDetail_aspx error">
        <asp:BulletedList ID="blErrorList" runat="server">
        </asp:BulletedList>
    </div>
    <div class="URLBUDetail_aspx url-bu-detail">
        <p class="title">
            <asp:Label ID="pagetitleLabel" runat="server" Text="Label"></asp:Label></p>
        <p class="instructions">
            <asp:Label ID="pageinstructionsLabel" runat="server" Text="Label"></asp:Label></p>
        <uc1:URLBUedit id="URLBUEdit1" runat="server" />
    </div>
</asp:Content>
