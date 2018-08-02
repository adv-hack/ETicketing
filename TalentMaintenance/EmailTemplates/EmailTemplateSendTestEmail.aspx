<%@ Page Title="" Language="VB" MasterPageFile="~/MasterPages/PageMaintenanceMasterShadowboxPage.master" AutoEventWireup="false" CodeFile="EmailTemplateSendTestEmail.aspx.vb" Inherits="EmailTemplates_EmailTemplateSendTestEmail" %>
<%@ Register Src="../UserControls/EmailTemplateSendTestEmail.ascx" TagName="emailTemplateSendTestEmail" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Content1" runat="Server">
    <div class="error">
        <asp:BulletedList ID="blErrorList" runat="server"></asp:BulletedList>
    </div>
    <div class="emailtemplate-sendtestemail">
        <p class="title"><asp:Label ID="pagetitleLabel" runat="server" Text="Label"></asp:Label></p>
        <p class="instructions"><asp:Label ID="pageinstructionsLabel" runat="server" Text="Label"></asp:Label></p>
        <uc1:emailTemplateSendTestEmail ID="emailTemplateSendTestEmail" runat="server" />
    </div>
</asp:Content>
