<%@ Page Title="" Language="VB" MasterPageFile="~/MasterPages/PageMaintenanceMasterPage.master"
    AutoEventWireup="false" CodeFile="EmailTemplateDetail.aspx.vb" Inherits="EmailTemplates_EmailTemplateDetail"
    ValidateRequest="false" %>

<%@ Register Src="../UserControls/EmailTemplateEdit.ascx" TagName="EmailTemplateEdit" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Content1" runat="Server">
    <div class="EmailTemplateDetail_aspx error">
        <asp:BulletedList ID="blErrorList" runat="server">
        </asp:BulletedList>
    </div>
    <div class="EmailTemplateDetail_aspx alert-detail">
        <p class="title"> <asp:Label ID="pagetitleLabel" runat="server" Text="Label"></asp:Label> </p>
        <p class="instructions"> <asp:Label ID="pageinstructionsLabel" runat="server" Text="Label"></asp:Label> </p>
        <uc1:EmailTemplateEdit ID="EmailTemplateEdit1" runat="server" />
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true">
        </asp:ScriptManager>
    </div>
</asp:Content>
