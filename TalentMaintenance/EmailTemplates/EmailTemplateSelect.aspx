<%@ Page Title="" Language="VB" MasterPageFile="~/MasterPages/PageMaintenanceMasterPage.master"
    AutoEventWireup="false" CodeFile="EmailTemplateSelect.aspx.vb" Inherits="EmailTemplates_EmailTemplateSelect" ValidateRequest="false" %>

<%@ Register Src="../UserControls/emailtemplateList.ascx" TagName="emailTemplateList" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Content1" runat="Server">
    <div class="EmailTemplateSelect_aspx error">
        <asp:BulletedList ID="blErrorList" runat="server">
        </asp:BulletedList>
    </div>
    <div class="EmailTemplateSelect_aspx email-template-selection">
        <p class="title"> <asp:Label ID="pagetitleLabel" runat="server" Text="Label"></asp:Label> </p>
        <p class="instructions"> <asp:Label ID="pageinstructionsLabel" runat="server" Text="Label"></asp:Label> </p>
        <uc1:emailTemplateList ID="emailTemplateList1" runat="server" />
    </div>
</asp:Content>
