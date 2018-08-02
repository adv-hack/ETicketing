<%@ Page Title="" Language="VB" MasterPageFile="~/MasterPages/PageMaintenanceMasterShadowboxPage.master" AutoEventWireup="false" CodeFile="EmailTemplateEditEmailContent.aspx.vb" Inherits="EmailTemplates_EmailTemplateEditEmailContent" validateRequest="false" %>
<%@ Register Src="../UserControls/EmailTemplateEditEmailContent.ascx" TagName="EmailTemplateEditEmailContent" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Content1" runat="Server">
    <div class="error">
        <asp:BulletedList ID="blErrorList" runat="server"></asp:BulletedList>
    </div>
    <div class="emailtemplate-sendtestemail">
        <p class="title"><asp:Label ID="pagetitleLabel" runat="server" Text="Label"></asp:Label></p>
        <p class="instructions"><asp:Label ID="pageinstructionsLabel" runat="server" Text="Label"></asp:Label></p>
        <uc1:EmailTemplateEditEmailContent ID="EmailTemplateEditEmailContent" runat="server" />
    </div>
</asp:Content>
