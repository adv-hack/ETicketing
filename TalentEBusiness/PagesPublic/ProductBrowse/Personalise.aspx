<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Personalise.aspx.vb" Inherits="PagesPublic_ProductBrowse_Personalise" %>
<%@ Register Src="~/UserControls/Flash.ascx" TagName="Flash" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/PageHeaderText.ascx" TagName="PageHeaderText" TagPrefix="Talent" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <Talent:PageHeaderText ID="PageHeaderText1" runat="server" />
    <p class="error"><asp:BulletedList ID="ErrorList" runat="server" CssClass="error"></asp:BulletedList></p>
    <div class="flash-wrapper">
    <Talent:Flash ID="Flash1" runat="server" visible = "true" />
    </div>
</asp:Content>