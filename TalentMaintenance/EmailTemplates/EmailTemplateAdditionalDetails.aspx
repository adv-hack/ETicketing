<%@ Page Title="" Language="VB" MasterPageFile="~/MasterPages/PageMaintenanceMasterShadowboxPage.master" AutoEventWireup="false" CodeFile="EmailTemplateAdditionalDetails.aspx.vb" Inherits="EmailTemplates_EmailTemplateAdditionalDetails" ValidateRequest="false" %>
<%@ Register Src="../UserControls/EmailTemplateAdditionalDetails.ascx" TagName="emailTemplateAdditionalDetails" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Content1" runat="Server">
    <div class="error">
        <asp:BulletedList ID="blErrorList" runat="server"></asp:BulletedList>
    </div>
    <div class="emailtemplate-additionnaldetails">
        <p class="title"><asp:Label ID="pagetitleLabel" runat="server" Text="Label"></asp:Label></p>
        <p class="instructions"><asp:Label ID="pageinstructionsLabel" runat="server" Text="Label"></asp:Label></p>
        <uc1:emailTemplateAdditionalDetails ID="emailTemplateAdditionalDetails" runat="server" />
    </div>
</asp:Content>
